using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

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

    public void AddAttribute(string owner, string name, object value)
    {
        // for some reason, other marshaling approaches do not work 
        var ownerBytes = Encoding.ASCII.GetBytes(owner + "\0");
        var nameBytes = Encoding.ASCII.GetBytes(name + "\0");
        
        if (value is string textValue)
        {
            var valueBytes = Encoding.ASCII.GetBytes(textValue + "\0");
            API.pdf_structure_element_add_attribute_text(Instance, ownerBytes, nameBytes, valueBytes);
        }
        else if (value is int intValue)
        {
            API.pdf_structure_element_add_attribute_integer(Instance, ownerBytes, nameBytes, intValue);
        }
        else if (value is float floatValue)
        {
            API.pdf_structure_element_add_attribute_float(Instance, ownerBytes, nameBytes, floatValue);
        }
        else if (value is float[] floatArray)
        {
            API.pdf_structure_element_add_attribute_float_array(Instance, ownerBytes, nameBytes, floatArray, floatArray.Length);
        }
        else if (value is int[] nodeIds)
        {
            API.pdf_structure_element_add_attribute_node_ids(Instance, ownerBytes, nameBytes, nodeIds, nodeIds.Length);
        }
        else
        {
            throw new ArgumentException($"Unsupported attribute value type: {value.GetType()}");
        }
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

        // to dispose the entire tree, it is enough to invoke the pdf_structure_element_delete method on the root element
        // root's children should be only marked as disposed
        DisposeChildren(this);
        
        API.pdf_structure_element_delete(Instance);
        Instance = IntPtr.Zero;
        GC.SuppressFinalize(this);
        
        static void DisposeChildren(SkPdfTag parent)
        {
            if (parent.Children == null)
                return;

            foreach (var child in parent.Children)
            {
                child.Instance = IntPtr.Zero;
                GC.SuppressFinalize(child);
                DisposeChildren(child);
            }
        }
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
        public static extern void pdf_structure_element_add_attribute_text(IntPtr element, byte[] owner, byte[] name, byte[] value);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pdf_structure_element_add_attribute_integer(IntPtr element, byte[] owner, byte[] name, int value);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pdf_structure_element_add_attribute_float(IntPtr element, byte[] owner, byte[] name, float value);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pdf_structure_element_add_attribute_float_array(IntPtr element, byte[] owner, byte[] name, float[] array, int arrayLength);

        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pdf_structure_element_add_attribute_node_ids(IntPtr element, byte[] owner, byte[] name, int[] array, int arrayLength);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pdf_structure_element_delete(IntPtr element);
    }
}