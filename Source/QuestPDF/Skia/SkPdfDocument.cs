using System;
using System.Runtime.InteropServices;

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

    public SkDateTime CreationDate;
    public SkDateTime ModificationDate;

    [MarshalAs(UnmanagedType.I1)] public bool SupportPDFA;
    [MarshalAs(UnmanagedType.I1)] public bool CompressDocument;
    
    public float RasterDPI;
    public int ImageEncodingQuality;
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