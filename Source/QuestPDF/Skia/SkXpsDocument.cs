using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia;

internal static class SkXpsDocument
{
    public static SkDocument Create(SkWriteStream stream, float dpi)
    {
        var instance = API.xps_document_create(stream.Instance, dpi);
        return new SkDocument(instance);
    }

    private static class API
    {
        [DllImport(SkiaAPI.LibraryName)]
        public static extern IntPtr xps_document_create(IntPtr stream, float dpi);
    }
}