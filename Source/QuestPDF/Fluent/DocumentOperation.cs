using System;
using System.Collections.Generic;
using System.IO;
using QuestPDF.Qpdf;

namespace NativeQpdf.Qpdf;

/// <summary>
/// Provides functionality for performing various operations on PDF documents, including loading, merging, overlaying, underlaying, selecting specific pages, adding attachments, and applying encryption settings.
/// </summary>
public class DocumentOperation
{
    /// <summary>
    /// Represents configuration options for applying an overlay or underlay to a PDF document using qpdf.
    /// </summary>
    public class LayerConfiguration
    {
        /// <summary>
        /// The file path of the overlay or underlay PDF file to be used.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Specifies the range of pages in the output document where the overlay or underlay will be applied.
        /// If not specified, the overlay or underlay is applied to all output pages.
        /// </summary>
        /// <remarks>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Syntax</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term>1, 2, 3</term>
        ///         <description>Plain numbers indicate pages numbered from the start</description>
        ///     </item>
        ///     <item>
        ///         <term>r1, r2</term>
        ///         <description>Numbers with 'r' prefix count from the end (r1 = last page)</description>
        ///     </item>
        ///     <item>
        ///         <term>z</term>
        ///         <description>Represents the last page (equivalent to r1)</description>
        ///     </item>
        ///     <item>
        ///         <term>1-5</term>
        ///         <description>Dash-separated ranges are inclusive</description>
        ///     </item>
        ///     <item>
        ///         <term>5-1</term>
        ///         <description>Reversed ranges list pages in descending order</description>
        ///     </item>
        ///     <item>
        ///         <term>x1-3</term>
        ///         <description>Excludes specified pages from previous range</description>
        ///     </item>
        ///     <item>
        ///         <term>:odd</term>
        ///         <description>Selects odd-positioned pages from the resulting page-set</description>
        ///     </item>
        ///     <item>
        ///         <term>:even</term>
        ///         <description>Selects even-positioned pages from the resulting page-set</description>
        ///     </item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Expression</term>
        ///         <description>Result</description>
        ///     </listheader>
        ///     <item>
        ///         <term>1,6,4</term>
        ///         <description>pages 1, 6, and 4 in that order</description>
        ///     </item>
        ///     <item>
        ///         <term>3-7</term>
        ///         <description>pages 3 through 7 inclusive</description>
        ///     </item>
        ///     <item>
        ///         <term>7-3</term>
        ///         <description>pages 7, 6, 5, 4, and 3 in that order</description>
        ///     </item>
        ///     <item>
        ///         <term>1-z</term>
        ///         <description>all pages in order</description>
        ///     </item>
        ///     <item>
        ///         <term>z-1</term>
        ///         <description>all pages in reverse order</description>
        ///     </item>
        ///     <item>
        ///         <term>r3-r1</term>
        ///         <description>the last three pages of the document</description>
        ///     </item>
        ///     <item>
        ///         <term>r1-r3</term>
        ///         <description>the last three pages of the document in reverse order</description>
        ///     </item>
        ///     <item>
        ///         <term>1-20:even</term>
        ///         <description>even pages from 2 to 20</description>
        ///     </item>
        ///     <item>
        ///         <term>5,7-9,12</term>
        ///         <description>pages 5, 7, 8, 9, and 12</description>
        ///     </item>
        ///     <item>
        ///         <term>5,7-9,12:odd</term>
        ///         <description>pages 5, 8, and 12 (pages in odd positions from the original set of 5, 7, 8, 9, 12)</description>
        ///     </item>
        ///     <item>
        ///         <term>5,7-9,12:even</term>
        ///         <description>pages 7 and 9 (pages in even positions from the original set of 5, 7, 8, 9, 12)</description>
        ///     </item>
        ///     <item>
        ///         <term>1-10,x3-4</term>
        ///         <description>pages 1 through 10 except pages 3 and 4 (1, 2, and 5 through 10)</description>
        ///     </item>
        ///     <item>
        ///         <term>4-10,x7-9,12-8,xr5</term>
        ///         <description>In a 15-page file: pages 4, 5, 6, 10, 12, 10, 9, and 8 in that order (pages 4 through 10 except 7 through 9, followed by 12 through 8 descending, except 11 which is the fifth page from the end)</description>
        ///     </item>
        /// </list>
        /// </example>
        public string? TargetPages { get; set; }

        /// <summary>
        /// Specifies the range of pages in the overlay or underlay file to be used initially.
        /// If not specified, all pages in the overlay or underlay file will be used in sequence.
        /// </summary>
        public string? SourcePages { get; set; }

        /// <summary>
        /// Specifies an optional range of pages in the overlay or underlay file that will repeat after the initial source pages are exhausted.
        /// Useful for repeating certain pages of the overlay or underlay file across multiple pages of the output.
        /// </summary>
        public string? RepeatSourcePages { get; set; }
    }
    
    public class DocumentAttachment
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
    
    public class Encryption40Bit : EncryptionBase
    {
        /// <summary>
        /// Specifies whether the user is permitted to add signatures and annotations to the document.
        /// </summary>
        public bool AllowAnnotation { get; set; } = true;
        
        /// <summary>
        /// Specifies whether the user is allowed to copy text and graphics from the document.
        /// </summary>
        public bool AllowContentExtraction { get; set; } = true;
        
        /// <summary>
        /// Specifies whether the user is permitted to insert, rotate, or delete pages within the document.
        /// </summary>
        public bool AllowModification { get; set; } = true;

        /// <summary>
        /// Specifies whether the user can print the document.
        /// </summary>
        public bool AllowPrinting { get; set; } = true;
    }

    public class Encryption128Bit : EncryptionBase
    {
        /// <summary>
        /// Specifies whether the user is permitted to add signatures and annotations to the document.
        /// </summary>
        public bool AllowAnnotation { get; set; } = true;
    
        /// <summary>
        /// Specifies whether the user is permitted to insert, rotate, or delete pages within the document.
        /// </summary>
        public bool AllowAssembly { get; set; } = true;
    
        /// <summary>
        /// Specifies whether the user is allowed to copy text and graphics from the document.
        /// </summary>
        public bool AllowContentExtraction { get; set; } = true;
    
        /// <summary>
        /// Specifies whether the user is allowed to fill out existing form fields in the document.
        /// </summary>
        public bool AllowFillingForms { get; set; } = true;

        /// <summary>
        /// Specifies whether the user can print the document.
        /// </summary>
        public bool AllowPrinting { get; set; } = true;
    
        /// <summary>
        /// Determines whether the document's metadata is included in encryption.
        /// </summary>
        public bool EncryptMetadata { get; set; } = true;
    }

    public class Encryption256Bit : EncryptionBase
    {
        /// <summary>
        /// Specifies whether the user is permitted to add signatures and annotations to the document.
        /// </summary>
        public bool AllowAnnotation { get; set; } = true;
    
        /// <summary>
        /// Specifies whether the user is permitted to insert, rotate, or delete pages within the document.
        /// </summary>
        public bool AllowAssembly { get; set; } = true;
    
        /// <summary>
        /// Specifies whether the user is allowed to copy text and graphics from the document.
        /// </summary>
        public bool AllowContentExtraction { get; set; } = true;
    
        /// <summary>
        /// Specifies whether the user is allowed to fill out existing form fields in the document.
        /// </summary>
        public bool AllowFillingForms { get; set; } = true;

        /// <summary>
        /// Specifies whether the user can print the document.
        /// </summary>
        public bool AllowPrinting { get; set; } = true;
    
        /// <summary>
        /// Determines whether the document's metadata is included in encryption.
        /// </summary>
        public bool EncryptMetadata { get; set; } = true;
    }
    
    internal JobConfiguration Configuration { get; private set; }
    
    private DocumentOperation()
    {
            
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
    /// Selects specific pages from the current document based on the provided page selector, marking them for further operations.
    /// </summary>
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
            Replace = attachment.Replace ? string.Empty : null
        });
        
        return this;

        string GetDefaultMimeType()
        {
            var fileExtension = Path.GetExtension(attachment.FilePath);
            return MimeHelper.FileExtensionToMimeConversionTable.TryGetValue(fileExtension, out var value) ? value : "text/plain";
        }
        
        string GetFormattedDate(DateTime? value, DateTime defaultValue)
        {
            return $"D:{(value ?? defaultValue).ToUniversalTime():yyyyMMddHHmmsss}Z";
        }
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
                Modify = FormatBooleanFlag(encryption.AllowModification),
                Print = FormatBooleanFlag(encryption.AllowPrinting)
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
        Configuration.OutputFile = filePath;
        var json = SimpleJsonSerializer.Serialize(Configuration);
        QpdfAPI.ExecuteJob(json);
    }
}