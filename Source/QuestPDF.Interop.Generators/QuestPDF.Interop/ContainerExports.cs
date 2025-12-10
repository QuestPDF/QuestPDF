using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct DynamicImageSourcePayload
{
    public float AvailableSpaceWidth;
    public float AvailableSpaceHeight;
    public int ImageSizeWidth;
    public int ImageSizeHeight;
    public int Dpi;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct DynamicSvgSourcePayload
{
    public float Width;
    public float Height;
}

public unsafe partial class Exports
{
    [UnmanagedCallersOnly(EntryPoint = "questpdf__container__image_bytes", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static IntPtr Container_Image_Bytes(IntPtr target, Buffer data)
    {
        var targetObject = UnboxHandle<IContainer>(target);
        var bytes = HandleBuffer(data);
        var result = targetObject.Image(bytes);
        return BoxHandle(result);
    }
    
    [UnmanagedCallersOnly(EntryPoint = "questpdf__container__image_dynamic", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static IntPtr Container_Image_Dynamic(IntPtr target, delegate* unmanaged[Cdecl]<DynamicImageSourcePayload, Buffer> source)
    {
        var targetObject = UnboxHandle<IContainer>(target);

        var result = targetObject.Image(payload =>
        {
            var interopPayload = new DynamicImageSourcePayload
            {
                AvailableSpaceWidth = payload.AvailableSpace.Width,
                AvailableSpaceHeight = payload.AvailableSpace.Height,
                ImageSizeWidth = payload.ImageSize.Width,
                ImageSizeHeight = payload.ImageSize.Height,
                Dpi = payload.Dpi
            };
            
            var resultInterop = source(interopPayload);
            var bytes = new byte[(int)resultInterop.length];

            fixed (byte* ptr = bytes)
                global::System.Buffer.MemoryCopy(resultInterop.data, ptr, bytes.Length, bytes.Length);
            
            return bytes;
        });
        
        return BoxHandle(result);
    }
    
    [UnmanagedCallersOnly(EntryPoint = "questpdf__container__svg_dynamic", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static void Container_Svg_Dynamic(IntPtr target, delegate* unmanaged[Cdecl]<DynamicSvgSourcePayload, char*> source)
    {
        var targetObject = UnboxHandle<IContainer>(target);

        targetObject.Svg(payload =>
        {
            var interopPayload = new DynamicSvgSourcePayload
            {
                Width = payload.Width,
                Height = payload.Height
            };
            
            var resultInterop = source(interopPayload);
            return Marshal.PtrToStringUTF8((IntPtr)resultInterop);
        });
    }
}