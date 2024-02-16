using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia;

internal sealed class Utf8StringMarshaller : ICustomMarshaler
{
    private static readonly Utf8StringMarshaller Instance = new();

    public static ICustomMarshaler GetInstance(string? cookie) => Instance;
    
    public void CleanUpManagedData(object managedObj)
    {
        
    }

    public void CleanUpNativeData(IntPtr pNativeData)
    {
        Marshal.FreeHGlobal(pNativeData);
    }

    public int GetNativeDataSize()
    {
        return -1;
    }

    public IntPtr MarshalManagedToNative(object managedObject)
    {
        return SkText.MarshalFromManagedToNative(managedObject as string);
    }

    public object MarshalNativeToManaged(IntPtr pNativeData)
    {
        throw new NotImplementedException();
    }
}