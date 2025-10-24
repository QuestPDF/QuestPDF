using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct Buffer
{
    public byte* data;
    public nuint length;  // size_t-like
}


public unsafe class Interop
{
    static Interop()
    {
        Settings.License = LicenseType.Community;
    }
    
    
    static IntPtr BoxHandle(object obj)
    {
        var gch = GCHandle.Alloc(obj, GCHandleType.Normal);         // keep object alive
        return GCHandle.ToIntPtr(gch);
    }

    static T UnboxHandle<T>(nint handle) where T : class
    {
        var gch = GCHandle.FromIntPtr(handle);
        return (T)gch.Target!;
    }

    static void FreeHandle(nint handle)
    {
        if (handle == 0) return;
        var gch = GCHandle.FromIntPtr(handle);
        if (gch.IsAllocated) gch.Free();
    }
    
    
    
    
    [UnmanagedCallersOnly(EntryPoint = "questpdf_sum", CallConvs = new[] { typeof(CallConvCdecl) })]
    [SuppressGCTransition]
    public static int Sum(int a, int b) => a + b;
    
    [UnmanagedCallersOnly(EntryPoint = "apply_binary_op", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static int ApplyBinaryOp(delegate* unmanaged[Cdecl]<int, int, int> op, int a, int b)
    {
        // Invoke the Python-provided callback
        return op(a, b);
    }
    
    
    
    
    
    
    
    
    
    
    
    [UnmanagedCallersOnly(EntryPoint = "questpdf_document_create", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static nint Document_Create(delegate* unmanaged[Cdecl]<nint, void> pageHandler) // returns opaque handle
    {
        var thing = Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    var pagePointer = BoxHandle(page);
                    pageHandler(pagePointer);
                });
            });
        
        return BoxHandle(thing);
    }

    [UnmanagedCallersOnly(EntryPoint = "questpdf_document_generate_pdf", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static Buffer Document_GeneratePdf(nint handle)
    {
        var thing = UnboxHandle<Document>(handle);
        var byteArray = thing.GeneratePdf();
        
        
        nuint len = (nuint)byteArray.Length;
        byte* ptr = (byte*)NativeMemory.Alloc(len); // caller must free

        // Copy managed -> unmanaged
        fixed (byte* src = byteArray)
        {
            global::System.Buffer.MemoryCopy(src, ptr, len, len);
        }

        return new Buffer { data = ptr, length = len };
    }

    [UnmanagedCallersOnly(EntryPoint = "questpdf_document_destroy", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static void Document_Destroy(nint handle)
    {
        FreeHandle(handle);
    }
    
    
    
    
    
    
    
    
    

    [UnmanagedCallersOnly(EntryPoint = "questpdf_page_set_margin", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static void Page_GeneratePdf(nint handle, int value)
    {
        var thing = UnboxHandle<PageDescriptor>(handle);
        thing.Margin(value);
    }

    [UnmanagedCallersOnly(EntryPoint = "questpdf_page_set_content", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static void Page_GeneratePdf(nint handle, byte* textPtr)
    {
        var thing = UnboxHandle<PageDescriptor>(handle);
        var textFromOutside = Marshal.PtrToStringUTF8((IntPtr)textPtr) ?? "";
        
        thing.Content().Text(text =>
        {
            text.DefaultTextStyle(x => x.FontSize(20));

            text.Span("Hello World... from ");
            text.Span(textFromOutside).FontColor(Colors.Blue.Darken1);
            text.Span("!");
        });
    }
    
    
    
    
    

    [UnmanagedCallersOnly(EntryPoint = "questpdf_free_bytes", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static void FreeBytes(byte* ptr)
    {
        Marshal.FreeHGlobal((IntPtr)ptr);
    }
}