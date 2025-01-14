﻿using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia.Text;

internal sealed class SkTypeface : IDisposable
{
    public IntPtr Instance { get; private set; }

    public SkTypeface(IntPtr instance)
    {
        Instance = instance;
        SkiaAPI.EnsureNotNull(Instance);
    }
    
    ~SkTypeface()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        API.typeface_unref(Instance);
        Instance = IntPtr.Zero;
        GC.SuppressFinalize(this);
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void typeface_unref(IntPtr typeface);
    }
}