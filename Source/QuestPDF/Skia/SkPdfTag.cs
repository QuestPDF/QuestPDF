using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia;

internal sealed class SkPdfTag : IDisposable
{
    public IntPtr Instance { get; private set; }
    public int NodeId { get; set; }
    public string Type { get; set; } = "";
    public string? Alt { get; set; }
    public string? Lang { get; set; }
    private ICollection<SkPdfTag>? Children { get; set; }
    
    private SkPdfTag(IntPtr instance)
    {
        Instance = instance;
        SkiaAPI.EnsureNotNull(Instance);
    }
    
    public static SkPdfTag Create(int nodeId, string? type, string? alt, string? lang)
    {
        var instance = API.pdf_structure_element_create(nodeId, type, alt, lang);
        return new SkPdfTag(instance) { NodeId = nodeId, Type = type ?? "", Alt = alt, Lang = lang };
    }
    
    public void SetChildren(ICollection<SkPdfTag> children)
    {
        Children = children;
        
        var childrenArray = children.ToArray();
        var childrenPointers = childrenArray.Select(c => c.Instance).ToArray();
        var unmanagedArray = Marshal.AllocHGlobal(IntPtr.Size * childrenPointers.Length);
        Marshal.Copy(childrenPointers, 0, unmanagedArray, childrenPointers.Length);
        
        API.pdf_structure_element_set_children(Instance, unmanagedArray, childrenPointers.Length);
        Marshal.FreeHGlobal(unmanagedArray);
    }

    ~SkPdfTag()
    {
        this.WarnThatFinalizerIsReached();
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        foreach (var child in Children ?? [])
            child.Instance = IntPtr.Zero;
        
        API.pdf_structure_element_delete(Instance);
        Instance = IntPtr.Zero;
        GC.SuppressFinalize(this);
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr pdf_structure_element_create(
            int nodeId,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaller))] string type,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaller))] string alt,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaller))] string lang);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pdf_structure_element_set_children(IntPtr element, IntPtr children, int count);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pdf_structure_element_delete(IntPtr element);
    }
}