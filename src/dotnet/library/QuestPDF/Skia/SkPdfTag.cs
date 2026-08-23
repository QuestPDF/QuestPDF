using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace QuestPDF.Skia;

internal sealed class SkPdfTag : IDisposable
{
    public IntPtr Instance { get; private set; }

    public SkPdfTag(IntPtr instance)
    {
        Instance = instance;
        SkiaAPI.EnsureNotNull(Instance);
    }

    // strings are marshalled manually: on modern .NET, every P/Invoke argument marshalled
    // with an ICustomMarshaler allocates a RuntimeMethodInfoStub object per call
    [SkipLocalsInit]
    public static IntPtr CreateElement(int nodeId, string? type, string? alt, string? lang)
    {
        var typePointer = SkText.MarshalFromManagedToNative(type);
        var altPointer = SkText.MarshalFromManagedToNative(alt);
        var langPointer = SkText.MarshalFromManagedToNative(lang);
        
        var instance = API.questpdf_skia_pdf_structure_element_create(nodeId, typePointer, altPointer, langPointer);
        SkiaAPI.EnsureNotNull(instance);
        
        Marshal.FreeHGlobal(typePointer);
        Marshal.FreeHGlobal(altPointer);
        Marshal.FreeHGlobal(langPointer);

        return instance;
    }

    public static unsafe void SetChildren(IntPtr element, IntPtr[] childElements, int childCount)
    {
        if (childCount == 0)
            return;

        fixed (IntPtr* childElementsPointer = childElements)
            API.questpdf_skia_pdf_structure_element_set_children(element, (IntPtr)childElementsPointer, childCount);
    }

    public static void AddAttribute(IntPtr element, string owner, string name, object value)
    {
        // for some reason, other marshaling approaches do not work
        var ownerBytes = Encoding.ASCII.GetBytes(owner + "\0");
        var nameBytes = Encoding.ASCII.GetBytes(name + "\0");

        if (value is string textValue)
        {
            var valueBytes = Encoding.ASCII.GetBytes(textValue + "\0");
            API.questpdf_skia_pdf_structure_element_add_attribute_text(element, ownerBytes, nameBytes, valueBytes);
        }
        else if (value is int intValue)
        {
            API.questpdf_skia_pdf_structure_element_add_attribute_integer(element, ownerBytes, nameBytes, intValue);
        }
        else if (value is float floatValue)
        {
            API.questpdf_skia_pdf_structure_element_add_attribute_float(element, ownerBytes, nameBytes, floatValue);
        }
        else if (value is float[] floatArray)
        {
            API.questpdf_skia_pdf_structure_element_add_attribute_float_array(element, ownerBytes, nameBytes, floatArray, floatArray.Length);
        }
        else if (value is int[] nodeIds)
        {
            API.questpdf_skia_pdf_structure_element_add_attribute_node_ids(element, ownerBytes, nameBytes, nodeIds, nodeIds.Length);
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

        // deleting the root element releases the entire native element tree,
        // child elements never get managed wrappers
        API.questpdf_skia_pdf_structure_element_delete(Instance);
        Instance = IntPtr.Zero;
        GC.SuppressFinalize(this);
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr questpdf_skia_pdf_structure_element_create(int nodeId, IntPtr type, IntPtr alt, IntPtr lang);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_pdf_structure_element_set_children(IntPtr element, IntPtr children, int count);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_pdf_structure_element_add_attribute_text(IntPtr element, byte[] owner, byte[] name, byte[] value);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_pdf_structure_element_add_attribute_integer(IntPtr element, byte[] owner, byte[] name, int value);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_pdf_structure_element_add_attribute_float(IntPtr element, byte[] owner, byte[] name, float value);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_pdf_structure_element_add_attribute_float_array(IntPtr element, byte[] owner, byte[] name, float[] array, int arrayLength);

        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_pdf_structure_element_add_attribute_node_ids(IntPtr element, byte[] owner, byte[] name, int[] array, int arrayLength);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_pdf_structure_element_delete(IntPtr element);
    }
}