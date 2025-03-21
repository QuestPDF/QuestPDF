using System.Collections.Generic;

namespace QuestPDF.Qpdf;

using Name = SimpleJsonPropertyNameAttribute;

sealed class JobConfiguration
{
    [Name("inputFile")] public string InputFile { get; set; }
    [Name("password")] public string? Password { get; set; }
    
    [Name("outputFile")] public string OutputFile { get; set; }
    
    [Name("pages")] public ICollection<PageConfiguration>? Pages { get; set; }
    [Name("overlay")] public ICollection<LayerConfiguration>? Overlay { get; set; }
    [Name("underlay")] public ICollection<LayerConfiguration>? Underlay { get; set; }

    [Name("extendMetadata")] public string? ExtendMetadata { get; set; }
    [Name("addAttachment")] public ICollection<AddDocumentAttachment>? AddAttachment { get; set; }
    
    [Name("encrypt")] public EncryptionSettings? Encrypt { get; set; } 
    [Name("linearize")] public string? Linearize { get; set; }
    [Name("newlineBeforeEndstream")] public string? NewlineBeforeEndstream { get; set; } = string.Empty;
    
    internal sealed class PageConfiguration
    {
        [Name("file")] public string File { get; set; }
        [Name("range")] public string Range { get; set; }
    }
    
    internal sealed class LayerConfiguration
    {
        [Name("file")] public string File { get; set; }
        [Name("to")] public string? To { get; set; }
        [Name("from")] public string? From { get; set; }
        [Name("repeat")] public string? Repeat { get; set; }
    }
    
    public sealed class AddDocumentAttachment
    {
        [Name("key")] public string Key { get; set; }
        [Name("file")] public string File { get; set; }
        [Name("filename")] public string? FileName { get; set; }
        [Name("creationdate")] public string? CreationDate { get; set; }
        [Name("moddate")] public string? ModificationDate { get; set; }
        [Name("mimetype")] public string? MimeType { get; set; }
        [Name("description")] public string? Description { get; set; }
        [Name("replace")] public string? Replace { get; set; }
        [Name("relationship")] public string? Relationship { get; set; }
    }

    public sealed class EncryptionSettings
    {
        [Name("userPassword")] public string? UserPassword { get; set; }
        [Name("ownerPassword")] public string OwnerPassword { get; set; }
        
        [Name("40bit")] public Encryption40Bit? Options40Bit { get; set; }
        [Name("128bit")] public Encryption128Bit? Options128Bit { get; set; }
        [Name("256bit")] public Encryption256Bit? Options256Bit { get; set; }
    }
    
    public sealed class Encryption40Bit
    {
        [Name("annotate")] public string Annotate { get; set; }
        [Name("extract")] public string Extract { get; set; }
        [Name("modify")] public string Modify { get; set; }
        [Name("print")] public string Print { get; set; }
    }

    public sealed class Encryption128Bit
    {
        [Name("annotate")] public string Annotate { get; set; }
        [Name("assemble")] public string Assemble { get; set; }
        [Name("extract")] public string Extract { get; set; }
        [Name("form")] public string Form { get; set; }
        [Name("print")] public string? Print { get; set; }
        [Name("cleartextMetadata")] public string? CleartextMetadata { get; set; }
    }

    public sealed class Encryption256Bit
    {
        [Name("annotate")] public string Annotate { get; set; }
        [Name("assemble")] public string Assemble { get; set; }
        [Name("extract")] public string Extract { get; set; }
        [Name("form")] public string Form { get; set; }
        [Name("print")] public string? Print { get; set; }
        [Name("cleartextMetadata")] public string? CleartextMetadata { get; set; }
    }
}