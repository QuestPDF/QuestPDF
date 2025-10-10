using System;
using System.Collections.Generic;
using System.IO;
using QuestPDF.Qpdf;

namespace QuestPDF.Fluent;

/// <summary>
/// Provides functionality for performing various operations on PDF documents, including loading, merging, overlaying, underlaying, selecting specific pages, adding attachments, and applying encryption settings.
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
        /// </summary>
        public string FilePath { get; set; }

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

    /// <summary>
    /// Represents configuration options for applying an overlay or underlay to a PDF document using qpdf with stream-based input.
    /// </summary>
    public sealed class LayerStreamConfiguration
    {
        /// <summary>
        /// The stream containing the overlay or underlay PDF data to be used.
        /// </summary>
        public Stream Stream { get; set; }

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
        /// </summary>
        public string? Key { get; set; }
    
        /// <summary>
        /// The file path of the attachment. Ensure that the specified file exists.
        /// </summary>
        public string FilePath { get; set; }
    
        /// <summary>
        /// Specifies the display name for the attachment.
        /// This name is typically shown to the user and used by most graphical PDF viewers when saving the file.
        /// Defaults to the file name without its path.
        /// </summary>
        public string? AttachmentName { get; set; }
    
        /// <summary>
        /// Specifies the creation date of the attachment. 
        /// Defaults to the file's creation time.
        /// </summary>
        public DateTime? CreationDate { get; set; }
    
        /// <summary>
        /// Specifies the modification date of the attachment.
        /// Defaults to the file's last modified time.
        /// </summary>
        public DateTime? ModificationDate { get; set; }
    
        /// <summary>
        /// Specifies the MIME type of the attachment, such as "text/plain", "application/pdf", "image/png", etc.
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

    public sealed class DocumentAttachmentStream
    {
        /// <summary>
        /// Sets the key for the attachment, specific to the PDF format.
        /// Defaults to the AttachmentName if provided, otherwise a generated key.
        /// </summary>
        public string? Key { get; set; }
    
        /// <summary>
        /// The stream containing the attachment data.
        /// </summary>
        public Stream Stream { get; set; }
    
        /// <summary>
        /// Specifies the display name for the attachment.
        /// This name is typically shown to the user and used by most graphical PDF viewers when saving the file.
        /// Required for stream-based attachments.
        /// </summary>
        public string AttachmentName { get; set; }
    
        /// <summary>
        /// Specifies the creation date of the attachment. 
        /// Defaults to the current date and time.
        /// </summary>
        public DateTime? CreationDate { get; set; }
    
        /// <summary>
        /// Specifies the modification date of the attachment.
        /// Defaults to the current date and time.
        /// </summary>
        public DateTime? ModificationDate { get; set; }
    
        /// <summary>
        /// Specifies the MIME type of the attachment, such as "text/plain", "application/pdf", "image/png", etc.
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

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.encryption.allow.printing"]/*' />
        public bool AllowPrinting { get; set; } = true;
    
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.encryption.encryptMetadata"]/*' />
        public bool EncryptMetadata { get; set; } = true;
    }
    
    internal JobConfiguration Configuration { get; private set; }
    private List<string> TemporaryFiles { get; } = new List<string>();
    
    private DocumentOperation()
    {
            
    }

    /// <summary>
    /// Creates a temporary file from a stream and tracks it for cleanup.
    /// </summary>
    private string CreateTemporaryFileFromStream(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        var tempFilePath = Path.GetTempFileName();
        TemporaryFiles.Add(tempFilePath);

        using var fileStream = File.Create(tempFilePath);
        stream.CopyTo(fileStream);
        
        return tempFilePath;
    }

    /// <summary>
    /// Cleans up temporary files created during stream operations.
    /// </summary>
    private void CleanupTemporaryFiles()
    {
        foreach (var tempFile in TemporaryFiles)
        {
            try
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
            catch
            {
                // Ignore cleanup errors - files might be in use or already deleted
            }
        }
        TemporaryFiles.Clear();
    }

    /// <summary>
    /// Loads the specified PDF file for processing, enabling operations such as merging, overlaying or underlaying content, selecting pages, adding attachments, and encrypting.
    /// </summary>
    /// <param name="filepath">The full path to the PDF file to be loaded.</param>
    /// <param name="password">The password for the PDF file, if it is password-protected. Optional.</param>
    public static DocumentOperation LoadFile(string filepath, string? password = null)
    {
        if (!File.Exists(filepath))
            throw new Exception("The file could not be found");
        
        return new DocumentOperation
        {
            Configuration = new JobConfiguration
            {
                InputFile = filepath,
                Password = password
            }
        };
    }
    
    /// <summary>
    /// Loads a PDF from the specified stream for processing, enabling operations such as merging, overlaying or underlaying content, selecting pages, adding attachments, and encrypting.
    /// </summary>
    /// <param name="stream">The stream containing the PDF data to be loaded.</param>
    /// <param name="password">The password for the PDF file, if it is password-protected. Optional.</param>
    public static DocumentOperation LoadStream(Stream stream, string? password = null)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        var operation = new DocumentOperation();
        var tempFilePath = operation.CreateTemporaryFileFromStream(stream);
        
        operation.Configuration = new JobConfiguration
        {
            InputFile = tempFilePath,
            Password = password
        };
        
        return operation;
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
    /// <param name="pageSelector">An optional <see cref="DocumentPageSelector"/> to specify the range of pages to merge. If not provided, all pages will be merged.</param>
    /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.pageSelector"]/*' />
    public DocumentOperation MergeFile(string filePath, string? pageSelector = null)
    {
        if (!File.Exists(filePath))
            throw new Exception("The file could not be found");
        
        if (Configuration.Pages == null)
            TakePages("1-z");
        
        Configuration.Pages.Add(new JobConfiguration.PageConfiguration
        {
            File = filePath,
            Range = pageSelector ?? "1-z"
        });
        
        return this;
    }

    /// <summary>
    /// Merges pages from the specified PDF stream into the current document, according to the provided page selection.
    /// </summary>
    /// <param name="stream">The stream containing the PDF data to be merged.</param>
    /// <param name="pageSelector">An optional <see cref="DocumentPageSelector"/> to specify the range of pages to merge. If not provided, all pages will be merged.</param>
    /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="documentOperation.pageSelector"]/*' />
    public DocumentOperation MergeStream(Stream stream, string? pageSelector = null)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));
        
        var tempFilePath = CreateTemporaryFileFromStream(stream);
        
        if (Configuration.Pages == null)
            TakePages("1-z");
        
        Configuration.Pages.Add(new JobConfiguration.PageConfiguration
        {
            File = tempFilePath,
            Range = pageSelector ?? "1-z"
        });
        
        return this;
    }

    /// <summary>
    /// Applies an underlay to the document using the specified configuration.
    /// The underlay pages are drawn beneath the target pages in the output file, potentially obscured by the original content.
    /// </summary>    
    public DocumentOperation UnderlayFile(LayerConfiguration configuration)
    {
        if (!File.Exists(configuration.FilePath))
            throw new Exception("The file could not be found");
        
        Configuration.Underlay ??= new List<JobConfiguration.LayerConfiguration>();
        
        Configuration.Underlay.Add(new JobConfiguration.LayerConfiguration
        {
            File = configuration.FilePath,
            To = configuration.TargetPages,
            From = configuration.SourcePages,
            Repeat = configuration.RepeatSourcePages
        });
        
        return this;
    }

    /// <summary>
    /// Applies an underlay to the document using the specified stream-based configuration.
    /// The underlay pages are drawn beneath the target pages in the output file, potentially obscured by the original content.
    /// </summary>    
    public DocumentOperation UnderlayStream(LayerStreamConfiguration configuration)
    {
        if (configuration?.Stream == null)
            throw new ArgumentNullException(nameof(configuration));
        
        var tempFilePath = CreateTemporaryFileFromStream(configuration.Stream);
        
        Configuration.Underlay ??= new List<JobConfiguration.LayerConfiguration>();
        
        Configuration.Underlay.Add(new JobConfiguration.LayerConfiguration
        {
            File = tempFilePath,
            To = configuration.TargetPages,
            From = configuration.SourcePages,
            Repeat = configuration.RepeatSourcePages
        });
        
        return this;
    }
    
    /// <summary>
    /// Applies an overlay to the document using the specified configuration.
    /// The overlay pages are drawn on top of the target pages in the output file, potentially obscuring the original content.
    /// </summary>
    public DocumentOperation OverlayFile(LayerConfiguration configuration)
    {
        if (!File.Exists(configuration.FilePath))
            throw new Exception("The file could not be found");
        
        Configuration.Overlay ??= new List<JobConfiguration.LayerConfiguration>();
        
        Configuration.Overlay.Add(new JobConfiguration.LayerConfiguration
        {
            File = configuration.FilePath,
            To = configuration.TargetPages,
            From = configuration.SourcePages,
            Repeat = configuration.RepeatSourcePages
        });
        
        return this;
    }

    /// <summary>
    /// Applies an overlay to the document using the specified stream-based configuration.
    /// The overlay pages are drawn on top of the target pages in the output file, potentially obscuring the original content.
    /// </summary>
    public DocumentOperation OverlayStream(LayerStreamConfiguration configuration)
    {
        if (configuration?.Stream == null)
            throw new ArgumentNullException(nameof(configuration));
        
        var tempFilePath = CreateTemporaryFileFromStream(configuration.Stream);
        
        Configuration.Overlay ??= new List<JobConfiguration.LayerConfiguration>();
        
        Configuration.Overlay.Add(new JobConfiguration.LayerConfiguration
        {
            File = tempFilePath,
            To = configuration.TargetPages,
            From = configuration.SourcePages,
            Repeat = configuration.RepeatSourcePages
        });
        
        return this;
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
        Configuration.ExtendMetadata = metadata;
        return this;
    }
    
    /// <summary>
    /// Adds an attachment to the document, with specified metadata and configuration options.
    /// </summary>
    public DocumentOperation AddAttachment(DocumentAttachment attachment)
    {
        Configuration.AddAttachment ??= new List<JobConfiguration.AddDocumentAttachment>();

        if (!File.Exists(attachment.FilePath))
            throw new Exception("The file could not be found");
        
        var file = new FileInfo(attachment.FilePath);
        
        Configuration.AddAttachment.Add(new JobConfiguration.AddDocumentAttachment
        {
            Key = attachment.Key ?? Path.GetFileName(attachment.FilePath),
            File = attachment.FilePath,
            FileName = attachment.AttachmentName ?? file.Name,
            CreationDate = GetFormattedDate(attachment.CreationDate, File.GetCreationTimeUtc(attachment.FilePath)),
            ModificationDate = GetFormattedDate(attachment.ModificationDate, File.GetLastWriteTime(attachment.FilePath)),
            MimeType = attachment.MimeType ?? GetDefaultMimeType(),
            Description = attachment.Description,
            Replace = attachment.Replace ? string.Empty : null,
            Relationship = GetRelationship(attachment.Relationship)
        });
        
        return this;

        string GetDefaultMimeType()
        {
            var fileExtension = Path.GetExtension(attachment.FilePath);
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
                DocumentAttachmentRelationship.Supplement => "/Alternative",
                DocumentAttachmentRelationship.Unspecified => "/Unspecified",
                null => null,
                _ => throw new ArgumentOutOfRangeException(nameof(relationship), relationship, null)
            };
        }
    }

    /// <summary>
    /// Adds an attachment to the document from a stream, with specified metadata and configuration options.
    /// </summary>
    public DocumentOperation AddAttachmentStream(DocumentAttachmentStream attachment)
    {
        if (attachment?.Stream == null)
            throw new ArgumentNullException(nameof(attachment));
        if (string.IsNullOrEmpty(attachment.AttachmentName))
            throw new ArgumentException("AttachmentName is required for stream-based attachments.", nameof(attachment));

        Configuration.AddAttachment ??= new List<JobConfiguration.AddDocumentAttachment>();

        var tempFilePath = CreateTemporaryFileFromStream(attachment.Stream);
        var now = DateTime.UtcNow;
        
        Configuration.AddAttachment.Add(new JobConfiguration.AddDocumentAttachment
        {
            Key = attachment.Key ?? attachment.AttachmentName,
            File = tempFilePath,
            FileName = attachment.AttachmentName,
            CreationDate = GetFormattedDate(attachment.CreationDate, now),
            ModificationDate = GetFormattedDate(attachment.ModificationDate, now),
            MimeType = attachment.MimeType ?? GetDefaultMimeType(),
            Description = attachment.Description,
            Replace = attachment.Replace ? string.Empty : null,
            Relationship = GetRelationship(attachment.Relationship)
        });
        
        return this;

        string GetDefaultMimeType()
        {
            var fileExtension = Path.GetExtension(attachment.AttachmentName);
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
                DocumentAttachmentRelationship.Supplement => "/Alternative",
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
            UserPassword = encryption.UserPassword,
            OwnerPassword = encryption.OwnerPassword,
            Options40Bit = new JobConfiguration.Encryption40Bit
            {
                Annotate = FormatBooleanFlag(encryption.AllowAnnotation),
                Extract = FormatBooleanFlag(encryption.AllowContentExtraction),
                Modify = encryption.AllowModification ? "all" : "none",
                Print = encryption.AllowPrinting ? "full" : "none",
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
            UserPassword = encryption.UserPassword,
            OwnerPassword = encryption.OwnerPassword,
            Options128Bit = new JobConfiguration.Encryption128Bit
            {
                Annotate = FormatBooleanFlag(encryption.AllowAnnotation),
                Assemble = FormatBooleanFlag(encryption.AllowAssembly),
                Extract = FormatBooleanFlag(encryption.AllowContentExtraction),
                Form = FormatBooleanFlag(encryption.AllowFillingForms),
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
            UserPassword = encryption.UserPassword,
            OwnerPassword = encryption.OwnerPassword,
            Options256Bit = new JobConfiguration.Encryption256Bit
            {
                Annotate = FormatBooleanFlag(encryption.AllowAnnotation),
                Assemble = FormatBooleanFlag(encryption.AllowAssembly),
                Extract = FormatBooleanFlag(encryption.AllowContentExtraction),
                Form = FormatBooleanFlag(encryption.AllowFillingForms),
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
        try
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
            
            Configuration.OutputFile = filePath;
            var json = SimpleJsonSerializer.Serialize(Configuration);
            QpdfAPI.ExecuteJob(json);
        }
        finally
        {
            CleanupTemporaryFiles();
        }
    }

    /// <summary>
    /// Executes the configured operations on the document and writes the resulting PDF to the specified stream.
    /// </summary>
    /// <param name="stream">The stream where the output PDF data will be written.</param>
    public void SaveToStream(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        var tempOutputFile = Path.GetTempFileName();
        
        try
        {
            Configuration.OutputFile = tempOutputFile;
            var json = SimpleJsonSerializer.Serialize(Configuration);
            QpdfAPI.ExecuteJob(json);
            
            using var fileStream = File.OpenRead(tempOutputFile);
            fileStream.CopyTo(stream);
        }
        finally
        {
            try
            {
                if (File.Exists(tempOutputFile))
                    File.Delete(tempOutputFile);
            }
            catch
            {
                // Ignore cleanup errors
            }
            
            CleanupTemporaryFiles();
        }
    }
}