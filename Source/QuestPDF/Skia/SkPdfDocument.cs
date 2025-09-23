using System;
using System.Runtime.InteropServices;
using System.Xml;

namespace QuestPDF.Skia;

[StructLayout(LayoutKind.Sequential)]
internal struct SkPdfDocumentMetadata
{
    public IntPtr Title; // string
    public IntPtr Author; // string
    public IntPtr Subject; // string
    public IntPtr Keywords; // string
    public IntPtr Creator; // string
    public IntPtr Producer; // string
    public IntPtr Language; // string

    public SkDateTime CreationDate;
    public SkDateTime ModificationDate;

    public PDFA_Conformance PDFA_Conformance;
    public PDFUA_Conformance PDFUA_Conformance;
    
    [MarshalAs(UnmanagedType.I1)] public bool CompressDocument;
    public float RasterDPI;

    public IntPtr SemanticNodeRoot;
}

internal enum PDFA_Conformance
{
    None = 0,
    PDFA_1A = 1,
    PDFA_1B = 2,
    PDFA_2A = 3,
    PDFA_2B = 4,
    PDFA_2U = 5,
    PDFA_3A = 6,
    PDFA_3B = 7,
    PDFA_3U = 8
}

internal enum PDFUA_Conformance
{
    None = 0,
    PDFUA_1 = 1
}

internal static class SkPdfDocument
{
    public static SkDocument Create(SkWriteStream stream, SkPdfDocumentMetadata metadata)
    {
        var instance = API.pdf_document_create(stream.Instance, metadata);
        return new SkDocument(instance);
    }

    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr pdf_document_create(IntPtr stream, SkPdfDocumentMetadata metadata);
    }
}