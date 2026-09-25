using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace QuestPDF.Qpdf;

static class XmpMetadataExtension
{
    private static readonly XNamespace RdfNamespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
    private static readonly XNamespace PdfaExtensionNamespace = "http://www.aiim.org/pdfa/ns/extension/";

    private static readonly XName RdfElementName = RdfNamespace + "RDF";
    private static readonly XName BagElementName = RdfNamespace + "Bag";
    private static readonly XName ExtensionSchemasElementName = PdfaExtensionNamespace + "schemas";

    /// <summary>
    /// Appends each content (usually <c>rdf:Description</c> elements) to the <c>rdf:RDF</c> element of the XMP packet.
    /// XMP allows only one <c>pdfaExtension:schemas</c> container per document, and QuestPDF already declares one for PDF/UA documents.
    /// Therefore, when both the packet and the content declare it, the schemas from the content are moved into the existing container.
    /// </summary>
    public static byte[] Extend(byte[] xmp, ICollection<string> contents)
    {
        var document = LoadMetadata(xmp);

        var rdf = document.Descendants(RdfElementName).FirstOrDefault()
            ?? throw new InvalidOperationException("The XMP metadata of the document does not contain the rdf:RDF element.");

        foreach (var content in contents)
        {
            foreach (var node in ParseContent(content).Nodes().ToList())
            {
                node.Remove();

                // skip descriptions that contained only the extension schemas
                if (node is XElement description && TryMoveExtensionSchemas(rdf, description) && !description.HasElements)
                    continue;

                rdf.Add(node);
            }
        }

        return SaveMetadata(document);
    }

    private static bool TryMoveExtensionSchemas(XElement rdf, XElement description)
    {
        var existingBag = rdf.Descendants(ExtensionSchemasElementName).Elements(BagElementName).FirstOrDefault();
        var schemas = description.Element(ExtensionSchemasElementName);
        var bag = schemas?.Element(BagElementName);

        if (existingBag == null || bag == null)
            return false;

        existingBag.Add(bag.Elements());
        schemas!.Remove();
        return true;
    }

    private static XDocument LoadMetadata(byte[] xmp)
    {
        // the metadata comes from the input document, XMP never uses DTD
        var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit };

        using var stream = new MemoryStream(xmp);
        using var reader = XmlReader.Create(stream, settings);
        return XDocument.Load(reader, LoadOptions.PreserveWhitespace);
    }

    private static XElement ParseContent(string content)
    {
        // the content uses the rdf prefix declared by the packet
        var wrapper = $"<rdf:RDF xmlns:rdf=\"{RdfNamespace}\">{content}</rdf:RDF>";
        return XElement.Parse(wrapper, LoadOptions.PreserveWhitespace);
    }

    private static byte[] SaveMetadata(XDocument document)
    {
        var settings = new XmlWriterSettings
        {
            Encoding = new UTF8Encoding(false),
            OmitXmlDeclaration = true,
            NewLineHandling = NewLineHandling.None
        };

        using var stream = new MemoryStream();

        using (var writer = XmlWriter.Create(stream, settings))
            document.Save(writer);

        return stream.ToArray();
    }
}
