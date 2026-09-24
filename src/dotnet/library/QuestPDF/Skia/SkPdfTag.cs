using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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

    // Skia keeps the owner, name and text value pointers until the document is closed (SkPDFUnion::Name(const char*)),
    // so the strings must not be moved or released in between. The attribute vocabulary is small and fixed,
    // therefore each distinct string is marshalled once and kept for the process lifetime.
    private static readonly ConcurrentDictionary<string, IntPtr> AttributeStrings = new();

    private static IntPtr GetAttributeString(string text)
    {
        Debug.Assert(text.Length <= 24, "Semantic attribute strings are expected to be short PDF names.");
        Debug.Assert(AttributeStrings.Count <= 32, "The number of distinct semantic attribute strings is unexpectedly large.");
        
        return AttributeStrings.GetOrAdd(text, SkText.MarshalFromManagedToNative);
    }

    public static void AddAttribute(IntPtr element, string owner, string name, object value)
    {
        var ownerPointer = GetAttributeString(owner);
        var namePointer = GetAttributeString(name);

        if (value is string textValue)
        {
            API.questpdf_skia_pdf_structure_element_add_attribute_text(element, ownerPointer, namePointer, GetAttributeString(textValue));
        }
        else if (value is int intValue)
        {
            API.questpdf_skia_pdf_structure_element_add_attribute_integer(element, ownerPointer, namePointer, intValue);
        }
        else if (value is float floatValue)
        {
            API.questpdf_skia_pdf_structure_element_add_attribute_float(element, ownerPointer, namePointer, floatValue);
        }
        else if (value is float[] floatArray)
        {
            API.questpdf_skia_pdf_structure_element_add_attribute_float_array(element, ownerPointer, namePointer, floatArray, floatArray.Length);
        }
        else if (value is int[] nodeIds)
        {
            API.questpdf_skia_pdf_structure_element_add_attribute_node_ids(element, ownerPointer, namePointer, nodeIds, nodeIds.Length);
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
        public static extern void questpdf_skia_pdf_structure_element_add_attribute_text(IntPtr element, IntPtr owner, IntPtr name, IntPtr value);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_pdf_structure_element_add_attribute_integer(IntPtr element, IntPtr owner, IntPtr name, int value);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_pdf_structure_element_add_attribute_float(IntPtr element, IntPtr owner, IntPtr name, float value);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_pdf_structure_element_add_attribute_float_array(IntPtr element, IntPtr owner, IntPtr name, float[] array, int arrayLength);

        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_pdf_structure_element_add_attribute_node_ids(IntPtr element, IntPtr owner, IntPtr name, int[] array, int arrayLength);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_pdf_structure_element_delete(IntPtr element);
    }
}