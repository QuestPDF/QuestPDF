using System;
using System.Runtime.InteropServices;
using System.Text;
using QuestPDF.Infrastructure;

namespace QuestPDF.Skia.Text;

internal sealed class SkFontManager
{
    public IntPtr Instance { get; }
    
    public static SkFontManager System { get; } = new(API.questpdf_skia_font_manager_create_system());

    internal SkFontManager(IntPtr instance)
    {
        Instance = instance;
        SkiaAPI.EnsureNotNull(Instance);
    }
    
    public FontInfo[] GetTypefaces()
    {
        API.questpdf_skia_font_manager_get_typefaces(Instance, out var array, out var arrayLength);
        
        try
        {
            var result = new FontInfo[arrayLength];
            var size = Marshal.SizeOf<API.SkFontInfo>();

            for (var i = 0; i < arrayLength; i++)
            {
                var fontInfo = Marshal.PtrToStructure<API.SkFontInfo>(IntPtr.Add(array, i * size));

                result[i] = new FontInfo
                {
                    FamilyName = DecodeString(fontInfo.FamilyName),
                    PostScriptName = DecodeString(fontInfo.PostScriptName),
                    Weight = fontInfo.Weight,
                    IsItalic = fontInfo.IsItalic,
                    IsVariable = fontInfo.IsVariable,
                };
            }

            return result;
        }
        finally
        {
            API.questpdf_skia_font_manager_delete_typefaces(array);
        }
        
        // decodes a NUL-terminated UTF-8 string stored in a fixed-size buffer
        static string DecodeString(byte[] buffer)
        {
            var length = Array.IndexOf(buffer, (byte)0);

            if (length < 0)
                length = buffer.Length;

            return Encoding.UTF8.GetString(buffer, 0, length);
        }
    }
    
    private static class API
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SkFontInfo
        {
            public const int StringBufferLength = 256;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = StringBufferLength)] public byte[] FamilyName;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = StringBufferLength)] public byte[] PostScriptName;
            public int Weight;
            [MarshalAs(UnmanagedType.U1)] public bool IsItalic;
            [MarshalAs(UnmanagedType.U1)] public bool IsVariable;
        }
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr questpdf_skia_font_manager_create_system();
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_font_manager_get_typefaces(IntPtr fontManager, out IntPtr array, out int arrayLength);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_font_manager_delete_typefaces(IntPtr array);
    }
}