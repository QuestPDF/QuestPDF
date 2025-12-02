using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct Buffer
{
    public byte* data;
    public nuint length;  // size_t-like
}


public static unsafe partial class Exports
{
    static IntPtr BoxHandle(object obj)
    {
        var gch = GCHandle.Alloc(obj, GCHandleType.Normal);
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
    
    static Buffer HandleBuffer(byte[] byteArray)
    {
        nuint len = (nuint)byteArray.Length;
        byte* ptr = (byte*)NativeMemory.Alloc(len); // caller must free

        // Copy managed -> unmanaged
        fixed (byte* src = byteArray)
        {
            global::System.Buffer.MemoryCopy(src, ptr, len, len);
        }

        return new Buffer { data = ptr, length = len };
    }
    
    static IntPtr HandleText(string text)
    {
        if (text == null) 
            return IntPtr.Zero;

        var length = Encoding.UTF8.GetByteCount(text);
        var nativeArray = Marshal.AllocHGlobal(length + 1);
        
        fixed (char* pText = text)
        {
            var ptr = (byte*)nativeArray;
            Encoding.UTF8.GetBytes(pText, text.Length, ptr, length);
        }
        
        Marshal.WriteByte(nativeArray, length, 0); // null termination
        
        return nativeArray;
    }
    
    static T NoTransformation<T>(T obj)
    {
        return obj;
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
    public static nint Document_Create(delegate* unmanaged[Cdecl]<nint, void> documentContainerHandler) // returns opaque handle
    {
        var thing = Document
            .Create(documentContainer =>
            {
                var pagePointer = BoxHandle(documentContainer);
                documentContainerHandler(pagePointer);
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
    
    
    
    
    
    [UnmanagedCallersOnly(EntryPoint = "questpdf_document_container_add_page", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static void Document_ContainerAddPage(nint documentContainerPointer, delegate* unmanaged[Cdecl]<nint, void> descriptor) // returns opaque handle
    {
        var documentContainer = UnboxHandle<IDocumentContainer>(documentContainerPointer);
        
        documentContainer.Page(page =>
        {
            var pagePointer = BoxHandle(page);
            descriptor(pagePointer);
        });
    }
    
    
    
    
    

    [UnmanagedCallersOnly(EntryPoint = "questpdf_page_set_margin", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static void Page_SetMargins(nint handle, int value)
    {
        var thing = UnboxHandle<PageDescriptor>(handle);
        thing.Margin(value);
    }

    [UnmanagedCallersOnly(EntryPoint = "questpdf_page_set_content", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static IntPtr Page_AddContent(nint handle)
    {
        var thing = UnboxHandle<PageDescriptor>(handle);
        var result = thing.Content();
        return BoxHandle(result);
    }
    
    [UnmanagedCallersOnly(EntryPoint = "questpdf_container_background", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static IntPtr Background(nint containerPointer, uint color)
    {
        var containerObject = UnboxHandle<IContainer>(containerPointer);
        FreeHandle(containerPointer);
        var result = containerObject.Background(color);
        return BoxHandle(result);
    }
    
    [UnmanagedCallersOnly(EntryPoint = "questpdf_free_bytes", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static void FreeBytes(byte* ptr)
    {
        Marshal.FreeHGlobal((IntPtr)ptr);
    }
}