using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuestPDF.Infrastructure;

namespace QuestPDF.Interop;

public unsafe partial class Exports
{
    [UnmanagedCallersOnly(EntryPoint = "questpdf__settings__license", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static void Settings_License(int licenseId)
    {
        Settings.License = (LicenseType)licenseId;
    }
    
    [UnmanagedCallersOnly(EntryPoint = "questpdf__settings__enable_debugging", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static void Settings_EnableDebugging(bool value)
    {
        Settings.EnableDebugging = value;
    }
    
    [UnmanagedCallersOnly(EntryPoint = "questpdf__settings__enable_caching", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static void Settings_EnableCaching(bool value)
    {
        Settings.EnableCaching = value;
    }
    
    [UnmanagedCallersOnly(EntryPoint = "questpdf__settings__check_if_all_text_glyphs_are_available", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static void Settings_CheckIfAllTextGlyphsAreAvailable(bool value)
    {
        Settings.CheckIfAllTextGlyphsAreAvailable = value;
    }
    
    [UnmanagedCallersOnly(EntryPoint = "questpdf__settings__use_environment_fonts", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static void Settings_UseEnvironmentFonts(bool value)
    {
        Settings.UseEnvironmentFonts = value;
    }
    
    [UnmanagedCallersOnly(EntryPoint = "questpdf__settings__temporary_storage_path", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static void Settings_TemporaryStoragePath(char* value)
    {
        Settings.TemporaryStoragePath = Marshal.PtrToStringUTF8((IntPtr)value);
    }
    
    [UnmanagedCallersOnly(EntryPoint = "questpdf__settings__font_discovery_paths", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static void Settings_FontDiscoveryPaths(char* delimitedList)
    {
        var value = Marshal.PtrToStringUTF8((IntPtr)delimitedList);
        Settings.FontDiscoveryPaths = value?.Split("__questpdf__") ?? Array.Empty<string>();
    }
}