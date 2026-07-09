using System;
using System.Runtime.InteropServices;
using QuestPDF.Helpers;
using QuestPDF.Skia.Text;

namespace QuestPDF.Skia;

internal sealed class SkResourceProvider
{
    public IntPtr Instance { get; private set; }
    
    public static SkResourceProvider Local { get; } = new(SkFontManager.Local);
    public static SkResourceProvider Global { get; } = new(SkFontManager.Global);
    
    internal static SkResourceProvider CurrentResourceProvider => Settings.UseEnvironmentFonts ? Global : Local;

    private SkResourceProvider(SkFontManager fontManager)
    {
        Instance = API.questpdf_skia_resource_provider_create(PathHelpers.ApplicationFilesPath, fontManager.Instance);
        SkiaAPI.EnsureNotNull(Instance);
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr questpdf_skia_resource_provider_create(string resourcesPath, IntPtr fontManager);
    }
}