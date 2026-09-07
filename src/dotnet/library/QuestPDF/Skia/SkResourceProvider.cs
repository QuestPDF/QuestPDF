using System;
using System.Runtime.InteropServices;
using QuestPDF.Helpers;

namespace QuestPDF.Skia;

internal sealed class SkResourceProvider
{
    public IntPtr Instance { get; private set; }
    
    public static SkResourceProvider Local { get; } = new();
    
    private SkResourceProvider()
    {
        Instance = API.questpdf_skia_resource_provider_create(PathHelpers.ApplicationFilesPath);
        SkiaAPI.EnsureNotNull(Instance);
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr questpdf_skia_resource_provider_create(string resourcesPath);
    }
}