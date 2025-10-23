using ImageMagick;

namespace QuestPDF.ConformanceTests;

public static class ImageHelpers
{
    public static void ConvertImageIccColorSpaceProfileToVersion2(Stream inputStream, Stream outputStream)
    {
        using var image = new MagickImage(inputStream);
        var iccVersion = GetIccProfileVersion();

        if (iccVersion == 2)
        {
            image.Write(outputStream);
            return;
        }

        if (iccVersion != null)
            image.RemoveProfile("icc");
        
        image.ColorSpace = ColorSpace.sRGB;
        image.SetProfile(ColorProfile.SRGB);
        
        image.Write(outputStream);

        int? GetIccProfileVersion()
        {
            var imageProfile = image.GetProfile("icc");
 
            if (imageProfile == null)
                return null;
            
            var imageProfileRaw = imageProfile.ToByteArray();

            if (imageProfileRaw.Length < 12)
                return null;
            
            return imageProfileRaw[8];
        }
    }

    public static void ConvertImageIccColorSpaceProfileToVersion2(string inputPath, string outputPath)
    {
        using var inputStream = File.OpenRead(inputPath);
        using var outputStream = File.OpenWrite(outputPath);
        ConvertImageIccColorSpaceProfileToVersion2(inputStream, outputStream);
    }
}