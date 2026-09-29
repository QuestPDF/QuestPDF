using System;
using System.Collections.Generic;
using System.IO;
using QuestPDF.Helpers;
using QuestPDF.Qpdf;

namespace QuestPDF.Fluent;

/// <summary>
/// Provides functionality for performing various operations on PDF documents, including loading, merging, overlaying, underlaying, selecting specific pages, adding attachments, and applying encryption settings.
/// Documents can be loaded from files or from in-memory data, and saved to a file, a stream, or a byte array.
/// </summary>
public sealed class DocumentOperation
{
    /// <summary>
    /// Represents configuration options for applying an overlay or underlay to a PDF document using qpdf.
    /// </summary>
    public sealed class LayerConfiguration
    {
        /// <summary>
        /// The file path of the overlay or underlay PDF file to be used.
        /// Please provide either this property or <see cref="DocumentData"/>.
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="filePath.remarks"]/*' />
        public string? FilePath { get; set; }

        /// <summary>
        /// The content of the overlay or underlay PDF document, provided as in-memory data.
        /// Please provide either this property or <see cref="FilePath"/>.
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.inMemoryData.remarks"]/*' />
        public byte[]? DocumentData { get; set; }

        /// <summary>
        /// Specifies the range of pages in the output document where the overlay or underlay will be applied.
        /// If not specified, the overlay or underlay is applied to all output pages.
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.pageSelector"]/*' />
        public string? TargetPages { get; set; }

        /// <summary>
        /// Specifies the range of pages in the overlay or underlay file to be used initially.
        /// If not specified, all pages in the overlay or underlay file will be used in sequence.
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.pageSelector"]/*' />
        public string? SourcePages { get; set; }

        /// <summary>
        /// Specifies an optional range of pages in the overlay or underlay file that will repeat after the initial source pages are exhausted.
        /// Useful for repeating certain pages of the overlay or underlay file across multiple pages of the output.
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.pageSelector"]/*' />
        public string? RepeatSourcePages { get; set; }
    }

    public enum DocumentAttachmentRelationship
    {
        /// <summary>
        /// Indicates data files relevant to the document (e.g., supporting datasets or data tables).
        /// </summary>
        Data,
        
        /// <summary>
        /// Represents a source file directly used to create the document.
        /// </summary>
        Source,
        
        /// <summary>
        /// An alternative representation of the document content (e.g., XML, HTML).
        /// </summary>
        Alternative,
        
        /// <summary>
        /// A file supplementing the content, like additional resources.
        /// </summary>
        Supplement,
        
        /// <summary>
        /// No specific relationship is defined.
        /// </summary>
        Unspecified
    }
    
    public sealed class DocumentAttachment
    {
        /// <summary>
        /// Sets the key for the attachment, specific to the PDF format.
        /// Defaults to the file name without its path.
        /// For attachments provided as <see cref="Content"/>, defaults to the <see cref="AttachmentName"/>.
        /// </summary>
        public string? Key { get; set; }
    
        /// <summary>
        /// The file path of the attachment. Ensure that the specified file exists.
        /// Please provide either this property or <see cref="Content"/>.
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="filePath.remarks"]/*' />
        public string? FilePath { get; set; }

        /// <summary>
        /// The content of the attachment, provided as in-memory data.
        /// Please provide either this property or <see cref="FilePath"/>.
        /// As there is no file name to derive defaults from, the <see cref="AttachmentName"/> property is required.
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.inMemoryData.remarks"]/*' />
        public byte[]? Content { get; set; }
    
        /// <summary>
        /// Specifies the display name for the attachment.
        /// This name is typically shown to the user and used by most graphical PDF viewers when saving the file.
        /// Defaults to the file name without its path.
        /// Required for attachments provided as <see cref="Content"/>.
        /// </summary>
        public string? AttachmentName { get; set; }
    
        /// <summary>
        /// Specifies the creation date of the attachment. 
        /// Defaults to the file's creation time, or to the current time for attachments provided as <see cref="Content"/>.
        /// </summary>
        public DateTime? CreationDate { get; set; }
    
        /// <summary>
        /// Specifies the modification date of the attachment.
        /// Defaults to the file's last modified time, or to the current time for attachments provided as <see cref="Content"/>.
        /// </summary>
        public DateTime? ModificationDate { get; set; }
    
        /// <summary>
        /// Specifies the MIME type of the attachment, such as "text/plain", "application/pdf", "image/png", etc.
        /// Defaults to the type matching the extension of the attachment's file name.
        /// </summary>
        public string? MimeType { get; set; }
    
        /// <summary>
        /// Sets a description for the attachment, which may be displayed by some PDF viewers.
        /// </summary>
        public string? Description { get; set; }
    
        /// <summary>
        /// Indicates whether to replace an existing attachment with the same key.
        /// If false, an exception is thrown if an attachment with the same key already exists.
        /// </summary>
        public bool Replace { get; set; } = true;
        
        /// <summary>
        /// Specifies the relationship of the embedded file to the document for PDF/A-3b compliance.
        /// </summary>
        public DocumentAttachmentRelationship? Relationship { get; set; } = null;
    }

    public class EncryptionBase
    {
        /// <summary>
        /// The user password for the PDF, allowing restricted access based on encryption settings. 
        /// May be left null to enable opening the PDF without a password, though this may restrict certain operations.
        /// </summary>
        public string? UserPassword { get; set; }
        
        /// <summary>
        /// The owner password for the PDF, granting full access to all document features.
        /// An empty owner password is considered insecure, as is using the same value for both user and owner passwords.
        /// </summary>
        public string OwnerPassword { get; set; }
    }
    
    public sealed class Encryption40Bit : EncryptionBase
    {
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.encryption.allow.annotation"]/*' />
        public bool AllowAnnotation { get; set; } = true;
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.encryption.allow.contentExtraction"]/*' />
        public bool AllowContentExtraction { get; set; } = true;
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.encryption.allow.modification"]/*' />
        public bool AllowModification { get; set; } = true;

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.encryption.allow.printing"]/*' />
        public bool AllowPrinting { get; set; } = true;
    }

    public sealed class Encryption128Bit : EncryptionBase
    {
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.encryption.allow.annotation"]/*' />
        public bool AllowAnnotation { get; set; } = true;
    
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.encryption.allow.assembly"]/*' />
        public bool AllowAssembly { get; set; } = true;
    
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.encryption.allow.contentExtraction"]/*' />
        public bool AllowContentExtraction { get; set; } = true;
    
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.encryption.allow.fillingForms"]/*' />
        public bool AllowFillingForms { get; set; } = true;

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.encryption.allow.modification"]/*' />
        public bool AllowModification { get; set; } = true;

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.encryption.allow.printing"]/*' />
        public bool AllowPrinting { get; set; } = true;
    
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.encryption.encryptMetadata"]/*' />
        public bool EncryptMetadata { get; set; } = true;
    }

    public sealed class Encryption256Bit : EncryptionBase
    {
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.encryption.allow.annotation"]/*' />
        public bool AllowAnnotation { get; set; } = true;
    
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.encryption.allow.assembly"]/*' />
        public bool AllowAssembly { get; set; } = true;
    
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.encryption.allow.contentExtraction"]/*' />
        public bool AllowContentExtraction { get; set; } = true;
    
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.encryption.allow.fillingForms"]/*' />
        public bool AllowFillingForms { get; set; } = true;

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.encryption.allow.modification"]/*' />
        public bool AllowModification { get; set; } = true;

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.encryption.allow.printing"]/*' />
        public bool AllowPrinting { get; set; } = true;
    
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.encryption.encryptMetadata"]/*' />
        public bool EncryptMetadata { get; set; } = true;
    }
    
    internal JobConfiguration Configuration { get; private set; }
    private List<string> MetadataExtensions { get; } = new();

    /// <summary>
    /// In-memory inputs, keyed by the names under which qpdf reads them.
    /// qpdf error messages quote these names, so they describe the role of each input.
    /// </summary>
    private Dictionary<string, byte[]> InputBuffers { get; } = new();

    private DocumentOperation()
    {
            
    }

    /// <summary>
    /// Loads the specified PDF file for processing, enabling operations such as merging, overlaying or underlaying content, selecting pages, adding attachments, and encrypting.
    /// </summary>
    /// <param name="filePath">The path to the PDF file to be loaded.</param>
    /// <param name="password">The password for the PDF file, if it is password-protected. Optional.</param>
    /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="filePath.remarks"]/*' />
    public static DocumentOperation LoadFile(string filePath, string? password = null)
    {
        return new DocumentOperation
        {
            Configuration = new JobConfiguration
            {
                InputFile = PathHelpers.ResolveResourceFilePath(filePath),
                Password = password
            }
        };
    }
    
    /// <summary>
    /// Loads the specified PDF document from in-memory data for processing, enabling operations such as merging, overlaying or underlaying content, selecting pages, adding attachments, and encrypting.
    /// </summary>
    /// <param name="documentData">The content of the PDF document to be loaded.</param>
    /// <param name="password">The password for the PDF document, if it is password-protected. Optional.</param>
    /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.inMemoryData.remarks"]/*' />
    public static DocumentOperation LoadDocument(byte[] documentData, string? password = null)
    {
        var operation = new DocumentOperation();

        operation.Configuration = new JobConfiguration
        {
            InputFile = operation.AddDocumentBuffer("input", documentData, nameof(documentData)),
            Password = password
        };

        return operation;
    }

    private string AddDocumentBuffer(string role, byte[] documentData, string parameterName)
    {
        if (documentData == null)
            throw new ArgumentNullException(parameterName);

        if (documentData.Length == 0)
            throw new ArgumentException("The document data cannot be empty.", parameterName);

        return AddInputBuffer(role, documentData);
    }

    private string AddInputBuffer(string role, byte[] data)
    {
        var name = role;

        for (var index = 2; InputBuffers.ContainsKey(name); index++)
            name = $"{role}-{index}";

        InputBuffers.Add(name, data);
        return QpdfAPI.BufferReferencePrefix + name;
    }

    private static void EnsureSingleSource(string? filePath, byte[]? data, string dataPropertyName, string parameterName)
    {
        if (filePath != null && data != null)
            throw new ArgumentException($"Please provide either the FilePath or the {dataPropertyName} property, not both.", parameterName);

        if (filePath == null && data == null)
            throw new ArgumentException($"Please provide either the FilePath or the {dataPropertyName} property.", parameterName);
    }

    /// <summary>
    /// Selects specific pages from the current document based on the provided page selector, marking them for further operations.
    /// </summary>
    /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.pageSelector"]/*' />
    public DocumentOperation TakePages(string pageSelector)
    {
        Configuration.Pages ??= new List<JobConfiguration.PageConfiguration>();
        
        Configuration.Pages.Add(new JobConfiguration.PageConfiguration
        {
            File = ".",
            Range = pageSelector
        });
        
        return this;
    }
    
    /// <summary>
    /// Merges pages from the specified PDF file into the current document, according to the provided page selection.
    /// </summary>
    /// <param name="filePath">The path to the PDF file to be merged.</param>
    /// <param name="pageSelector">An optional page selector to specify the range of pages to merge. If not provided, all pages will be merged.</param>
    /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.pageSelector"]/*' />
    /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="filePath.remarks"]/*' />
    public DocumentOperation MergeFile(string filePath, string? pageSelector = null)
    {
        return MergePages(PathHelpers.ResolveResourceFilePath(filePath), pageSelector);
    }

    /// <summary>
    /// Merges pages from the specified in-memory PDF document into the current document, according to the provided page selection.
    /// </summary>
    /// <param name="documentData">The content of the PDF document to be merged.</param>
    /// <param name="pageSelector">An optional page selector to specify the range of pages to merge. If not provided, all pages will be merged.</param>
    /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.pageSelector"]/*' />
    /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.inMemoryData.remarks"]/*' />
    public DocumentOperation MergeDocument(byte[] documentData, string? pageSelector = null)
    {
        return MergePages(AddDocumentBuffer("merged-document", documentData, nameof(documentData)), pageSelector);
    }

    private DocumentOperation MergePages(string file, string? pageSelector)
    {
        if (Configuration.Pages == null)
            TakePages("1-z");

        Configuration.Pages.Add(new JobConfiguration.PageConfiguration
        {
            File = file,
            Range = pageSelector ?? "1-z"
        });
        
        return this;
    }

    /// <summary>
    /// Applies an underlay to the document using the specified configuration.
    /// The underlay pages are drawn beneath the target pages in the output file, potentially obscured by the original content.
    /// The underlay document can be provided either as a file or as in-memory data.
    /// </summary>
    /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="filePath.remarks"]/*' />
    public DocumentOperation UnderlayFile(LayerConfiguration configuration)
    {
        var file = GetLayerFile(configuration, "underlay");

        Configuration.Underlay ??= new List<JobConfiguration.LayerConfiguration>();
        
        Configuration.Underlay.Add(new JobConfiguration.LayerConfiguration
        {
            File = file,
            To = configuration.TargetPages,
            From = configuration.SourcePages,
            Repeat = configuration.RepeatSourcePages
        });
        
        return this;
    }
    
    /// <summary>
    /// Applies an overlay to the document using the specified configuration.
    /// The overlay pages are drawn on top of the target pages in the output file, potentially obscuring the original content.
    /// The overlay document can be provided either as a file or as in-memory data.
    /// </summary>
    /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="filePath.remarks"]/*' />
    public DocumentOperation OverlayFile(LayerConfiguration configuration)
    {
        var file = GetLayerFile(configuration, "overlay");

        Configuration.Overlay ??= new List<JobConfiguration.LayerConfiguration>();
        
        Configuration.Overlay.Add(new JobConfiguration.LayerConfiguration
        {
            File = file,
            To = configuration.TargetPages,
            From = configuration.SourcePages,
            Repeat = configuration.RepeatSourcePages
        });
        
        return this;
    }

    private string GetLayerFile(LayerConfiguration configuration, string role)
    {
        EnsureSingleSource(configuration.FilePath, configuration.DocumentData, nameof(LayerConfiguration.DocumentData), nameof(configuration));

        return configuration.DocumentData != null
            ? AddDocumentBuffer(role, configuration.DocumentData, nameof(configuration))
            : PathHelpers.ResolveResourceFilePath(configuration.FilePath!);
    }

    /// <summary>
    /// Extends the current document's XMP metadata by adding content within the <c>rdf:Description</c> tag.
    /// This allows for adding additional descriptive metadata to the PDF, which is useful for compliance standards
    /// like PDF/A or for industry-specific metadata (e.g., ZUGFeRD).
    /// </summary>
    /// <param name="metadata">
    /// A string containing the metadata to add. This metadata must be valid XML content and conform to the
    /// RDF structure required by the PDF XMP metadata specification.
    /// </param>
    public DocumentOperation ExtendMetadata(string metadata)
    {
        MetadataExtensions.Add(metadata);
        return this;
    }
    
    /// <summary>
    /// Adds an attachment to the document, with specified metadata and configuration options.
    /// The attachment content can be provided either as a file or as in-memory data.
    /// </summary>
    /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="filePath.remarks"]/*' />
    public DocumentOperation AddAttachment(DocumentAttachment attachment)
    {
        EnsureSingleSource(attachment.FilePath, attachment.Content, nameof(DocumentAttachment.Content), nameof(attachment));

        var configuration = attachment.Content != null
            ? CreateFromContent(attachment.Content)
            : CreateFromFile(attachment.FilePath!);

        Configuration.AddAttachment ??= new List<JobConfiguration.AddDocumentAttachment>();
        Configuration.AddAttachment.Add(configuration);
        
        return this;

        JobConfiguration.AddDocumentAttachment CreateFromFile(string filePath)
        {
            filePath = PathHelpers.ResolveResourceFilePath(filePath);
            var file = new FileInfo(filePath);

            return Create(filePath, file.Name, file.CreationTimeUtc, file.LastWriteTime);
        }

        JobConfiguration.AddDocumentAttachment CreateFromContent(byte[] content)
        {
            // the attachment name takes the role of the file name
            if (attachment.AttachmentName == null)
                throw new ArgumentException("Please provide the AttachmentName property (e.g. 'invoice.xml') for attachments provided as Content.", nameof(attachment));

            var now = DateTime.UtcNow;
            return Create(AddInputBuffer("attachment", content), attachment.AttachmentName, now, now);
        }

        JobConfiguration.AddDocumentAttachment Create(string file, string fileName, DateTime defaultCreationDate, DateTime defaultModificationDate)
        {
            return new JobConfiguration.AddDocumentAttachment
            {
                Key = attachment.Key ?? fileName,
                File = file,
                FileName = attachment.AttachmentName ?? fileName,
                CreationDate = GetFormattedDate(attachment.CreationDate, defaultCreationDate),
                ModificationDate = GetFormattedDate(attachment.ModificationDate, defaultModificationDate),
                MimeType = attachment.MimeType ?? GetDefaultMimeType(fileName),
                Description = attachment.Description,
                Replace = attachment.Replace ? string.Empty : null,
                Relationship = GetRelationship(attachment.Relationship)
            };
        }

        string GetDefaultMimeType(string fileName)
        {
            var fileExtension = Path.GetExtension(fileName);
            fileExtension = fileExtension.TrimStart('.').ToLowerInvariant();
            return MimeHelper.FileExtensionToMimeConversionTable.TryGetValue(fileExtension, out var value) ? value : "text/plain";
        }
        
        string GetFormattedDate(DateTime? value, DateTime defaultValue)
        {
            return $"D:{(value ?? defaultValue).ToUniversalTime():yyyyMMddHHmmsss}Z";
        }
        
        string? GetRelationship(DocumentAttachmentRelationship? relationship)
        {
            return relationship switch
            {
                DocumentAttachmentRelationship.Data => "/Data",
                DocumentAttachmentRelationship.Source => "/Source",
                DocumentAttachmentRelationship.Alternative => "/Alternative",
                DocumentAttachmentRelationship.Supplement => "/Supplement",
                DocumentAttachmentRelationship.Unspecified => "/Unspecified",
                null => null,
                _ => throw new ArgumentOutOfRangeException(nameof(relationship), relationship, null)
            };
        }
    }

    /// <summary>
    /// Removes any existing encryption from the current PDF document, effectively making it accessible without a password or encryption restrictions.
    /// </summary>
    public DocumentOperation Decrypt()
    {
        Configuration.Decrypt = string.Empty;
        return this;
    }
    
    /// <summary>
    /// Remove security restrictions associated with digitally signed PDF files.
    /// This may be combined with Decrypt() operation to allow free editing of previously signed/encrypted files.
    /// This option invalidates and disables any digital signatures but leaves their visual appearances intact.
    /// </summary>
    public DocumentOperation RemoveRestrictions()
    {
        Configuration.Decrypt = string.Empty;
        Configuration.RemoveRestrictions = string.Empty;
        return this;
    }
    
    /// <summary>
    /// Encrypts the document using 40-bit encryption, applying specified owner and user passwords along with defined permissions.
    /// </summary>
    public DocumentOperation Encrypt(Encryption40Bit encryption)
    {
        if (Configuration.Encrypt != null)
            throw new InvalidOperationException("Encryption process can be set only once");
        
        Configuration.Encrypt = new JobConfiguration.EncryptionSettings
        {
            UserPassword = encryption.UserPassword ?? string.Empty,
            OwnerPassword = encryption.OwnerPassword,
            Options40Bit = new JobConfiguration.Encryption40Bit
            {
                Annotate = FormatBooleanFlag(encryption.AllowAnnotation),
                Extract = FormatBooleanFlag(encryption.AllowContentExtraction),
                Modify = encryption.AllowModification ? null : "none",
                Print = encryption.AllowPrinting ? null : "none",
            }
        };
        
        return this;
    }
    
    /// <summary>
    /// Encrypts the document using 128-bit encryption, applying specified owner and user passwords along with defined permissions.
    /// </summary>
    public DocumentOperation Encrypt(Encryption128Bit encryption)
    {
        if (Configuration.Encrypt != null)
            throw new InvalidOperationException("Encryption process can be set only once");
        
        Configuration.Encrypt = new JobConfiguration.EncryptionSettings
        {
            UserPassword = encryption.UserPassword ?? string.Empty,
            OwnerPassword = encryption.OwnerPassword,
            Options128Bit = new JobConfiguration.Encryption128Bit
            {
                Annotate = FormatBooleanFlag(encryption.AllowAnnotation),
                Assemble = FormatBooleanFlag(encryption.AllowAssembly),
                Extract = FormatBooleanFlag(encryption.AllowContentExtraction),
                Form = FormatBooleanFlag(encryption.AllowFillingForms),
                ModifyOther = FormatBooleanFlag(encryption.AllowModification),
                Print = encryption.AllowPrinting ? "full" : "none",
                CleartextMetadata = encryption.EncryptMetadata ? null : string.Empty
            }
        };
        
        return this;
    }
    
    /// <summary>
    /// Encrypts the document using 256-bit encryption, applying specified owner and user passwords along with defined permissions.
    /// </summary>
    public DocumentOperation Encrypt(Encryption256Bit encryption)
    {
        if (Configuration.Encrypt != null)
            throw new InvalidOperationException("Encryption process can be set only once");
        
        Configuration.Encrypt = new JobConfiguration.EncryptionSettings
        {
            UserPassword = encryption.UserPassword ?? string.Empty,
            OwnerPassword = encryption.OwnerPassword,
            Options256Bit = new JobConfiguration.Encryption256Bit
            {
                Annotate = FormatBooleanFlag(encryption.AllowAnnotation),
                Assemble = FormatBooleanFlag(encryption.AllowAssembly),
                Extract = FormatBooleanFlag(encryption.AllowContentExtraction),
                Form = FormatBooleanFlag(encryption.AllowFillingForms),
                ModifyOther = FormatBooleanFlag(encryption.AllowModification),
                Print = encryption.AllowPrinting ? "full" : "none",
                CleartextMetadata = encryption.EncryptMetadata ? null : string.Empty
            }
        };
        
        return this;
    }

    private string FormatBooleanFlag(bool value)
    {
        return value ? "y" : "n";
    }
    
    /// <summary>
    /// Creates linearized (web-optimized) output files.
    /// Linearized files are structured to allow compliant PDF readers to begin displaying content before the entire file is downloaded.
    /// Normally, a PDF reader requires the entire file to be present to render content, as essential cross-reference data typically appears at the file’s end.
    /// </summary>
    public DocumentOperation Linearize()
    {
        Configuration.Linearize = string.Empty;
        return this;
    }
    
    /// <summary>
    /// Executes the configured operations on the document and saves the resulting file to the specified path.
    /// </summary>
    /// <param name="filePath">The path where the output file will be saved.</param>
    public void Save(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
        
        Configuration.OutputFile = filePath;
        Execute(outputStream: null);
    }

    /// <summary>
    /// Executes the configured operations on the document and writes the resulting document to the provided stream.
    /// </summary>
    /// <remarks>
    /// The document is written synchronously, in chunks, as it is produced.
    /// If the operation fails, the stream may already contain a part of the document.
    /// </remarks>
    /// <param name="stream">The writable stream to which the resulting document will be written. The stream is left open.</param>
    public void Save(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        if (!stream.CanWrite)
            throw new ArgumentException("The stream must be writable.", nameof(stream));

        Configuration.OutputFile = QpdfAPI.OutputBufferReference;
        Execute(stream);
    }

    /// <summary>
    /// Executes the configured operations on the document and returns the resulting document as a byte array.
    /// </summary>
    public byte[] Save()
    {
        using var stream = new MemoryStream();
        Save(stream);
        return stream.ToArray();
    }

    private void Execute(Stream? outputStream)
    {
        var json = QpdfJobSerializer.Serialize(Configuration);

        Func<byte[], byte[]>? transformMetadata = MetadataExtensions.Count > 0
            ? xmp => XmpMetadataExtension.Extend(xmp, MetadataExtensions)
            : null;

        QpdfAPI.ExecuteJob(json, InputBuffers, outputStream, transformMetadata);
    }
}
