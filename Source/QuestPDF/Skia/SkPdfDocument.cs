using System;
using System.Runtime.InteropServices;

namespace NativeSkia;

[StructLayout(LayoutKind.Sequential)]
internal struct SkPdfDocumentMetadata
{
    [MarshalAs(UnmanagedType.LPUTF8Str)] public string Title;
    [MarshalAs(UnmanagedType.LPUTF8Str)] public string Author;
    [MarshalAs(UnmanagedType.LPUTF8Str)] public string Subject;
    [MarshalAs(UnmanagedType.LPUTF8Str)] public string Keywords;
    [MarshalAs(UnmanagedType.LPUTF8Str)] public string Creator;
    [MarshalAs(UnmanagedType.LPUTF8Str)] public string Producer;

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
        [DllImport(SkiaAPI.LibraryName)]
        public static extern IntPtr pdf_document_create(IntPtr stream, SkPdfDocumentMetadata metadata);
    }
}