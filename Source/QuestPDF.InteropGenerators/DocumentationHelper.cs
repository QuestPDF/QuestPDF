using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace QuestPDF.InteropGenerators;

/// <summary>
/// Helper class for extracting and processing XML documentation comments
/// </summary>
public static class DocumentationHelper
{
    /// <summary>
    /// Extracts documentation from XML documentation comments.
    /// Processes summary and remarks sections, stripping XML tags and formatting.
    /// </summary>
    /// <param name="xmlDocumentation">Raw XML documentation string from Roslyn symbol</param>
    /// <returns>Cleaned documentation text, or empty string if no documentation exists</returns>
    public static string ExtractDocumentation(string? xmlDocumentation)
    {
        if (string.IsNullOrWhiteSpace(xmlDocumentation))
            return string.Empty;

        var doc = new StringBuilder();

        try
        {
            var xml = XDocument.Parse(xmlDocumentation);

            // Extract summary
            var summary = xml.Descendants("summary").FirstOrDefault();
            if (summary != null)
            {
                var summaryText = CleanXmlContent(summary);
                if (!string.IsNullOrWhiteSpace(summaryText))
                    doc.AppendLine(summaryText);
            }

            // Extract remarks
            var remarks = xml.Descendants("remarks").FirstOrDefault();
            if (remarks != null)
            {
                var remarksText = CleanXmlContent(remarks);
                if (!string.IsNullOrWhiteSpace(remarksText))
                {
                    if (doc.Length > 0)
                        doc.AppendLine();
                    doc.Append(remarksText);
                }
            }
        }
        catch
        {
            // If XML parsing fails, return empty string
            return string.Empty;
        }

        return doc.ToString().Trim();
    }

    /// <summary>
    /// Cleans XML content by removing tags like &lt;see cref="..." /&gt; and normalizing whitespace
    /// </summary>
    /// <param name="element">XML element to clean</param>
    /// <returns>Cleaned text content</returns>
    private static string CleanXmlContent(XElement element)
    {
        var text = new StringBuilder();

        foreach (var node in element.Nodes())
        {
            if (node is XText textNode)
            {
                text.Append(textNode.Value);
            }
            else if (node is XElement childElement)
            {
                // Handle specific XML elements
                switch (childElement.Name.LocalName)
                {
                    case "see":
                    case "seealso":
                        // Extract the referenced name from cref attribute
                        var cref = childElement.Attribute("cref")?.Value;
                        if (!string.IsNullOrEmpty(cref))
                        {
                            // Extract just the member name (e.g., "Height" from "ConstrainedExtensions.Height")
                            var parts = cref.Split('.');
                            var memberName = parts.Length > 0 ? parts[parts.Length - 1] : cref;
                            text.Append(memberName);
                        }
                        else
                        {
                            // Fallback to inner text if no cref
                            text.Append(childElement.Value);
                        }
                        break;

                    case "paramref":
                    case "typeparamref":
                        // Extract parameter name
                        var name = childElement.Attribute("name")?.Value ?? childElement.Value;
                        text.Append(name);
                        break;

                    case "c":
                    case "code":
                        // Inline code or code blocks
                        text.Append(childElement.Value);
                        break;

                    default:
                        // For other elements, just get the text content
                        text.Append(childElement.Value);
                        break;
                }
            }
        }

        // Normalize whitespace: collapse multiple spaces/newlines into single spaces
        var result = text.ToString();
        result = System.Text.RegularExpressions.Regex.Replace(result, @"\s+", " ");
        return result.Trim();
    }
}
