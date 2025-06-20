using System.Runtime.CompilerServices;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.VisualTests;

public class ImageComparer
{
    public static bool AreImagesIdentical(SKBitmap bitmap1, SKBitmap bitmap2)
    {
        if (bitmap1 == null || bitmap2 == null)
            return false;
            
        if (bitmap1.Width != bitmap2.Width || bitmap1.Height != bitmap2.Height)
            return false;
            
        if (bitmap1.ColorType != bitmap2.ColorType)
            return false;

        var pixels1 = bitmap1.Pixels;
        var pixels2 = bitmap2.Pixels;
        
        if (pixels1.Length != pixels2.Length)
            return false;
            
        for (var i = 0; i < pixels1.Length; i++)
        {
            if (pixels1[i] != pixels2[i])
                return false;
        }
        
        return true;
    }
    
    public static bool AreImagesIdentical(byte[] imageData1, byte[] imageData2)
    {
        using var bitmap1 = SKBitmap.Decode(imageData1);
        using var bitmap2 = SKBitmap.Decode(imageData2);
        
        return AreImagesIdentical(bitmap1, bitmap2);
    }
}

public static class VisualTestEngine
{
    public static void ShouldMatchExpectedImage(this IDocument document, [CallerMemberName] string callerName = null)
    {
        var generatedImageBytes = document.GenerateImages(new ImageGenerationSettings
        {
            ImageFormat = ImageFormat.Png,
            RasterDpi = 144
        }).First();
        
        var expectedImagePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "ExpectedOutput", $"{callerName}.png");
        
        if (!File.Exists(expectedImagePath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(expectedImagePath)!);
            
            File.WriteAllBytes(expectedImagePath, generatedImageBytes);
            Assert.Inconclusive($"Expected image created at: {expectedImagePath}. Please verify the image looks correct and re-run the test.");
        }
        
        var imagesAreIdentical = ImageComparer.AreImagesIdentical(generatedImageBytes, File.ReadAllBytes(expectedImagePath));
        
        if (!imagesAreIdentical)
        {
            var actualImagePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "ActualOutput", $"{callerName}.png");
            Directory.CreateDirectory(Path.GetDirectoryName(actualImagePath)!);
            File.WriteAllBytes(actualImagePath, generatedImageBytes);
            File.SetLastWriteTimeUtc(actualImagePath, DateTime.UtcNow);
            Assert.Fail($"Generated image does not match expected image. Actual image saved to: {actualImagePath}. rider64 diff {expectedImagePath} {actualImagePath}");
        }
        
        Assert.Pass("Images are identical!");
    }
}

public static class VisualTest
{
    public static void Perform(Action<IDocumentContainer> documentBuilder, [CallerMemberName] string callerName = null)
    {
        Document
            .Create(documentBuilder)
            .ShouldMatchExpectedImage(callerName);
    }
    
    public static void PerformWithDefaultPageSettings(Action<IContainer> contentBuilder, [CallerMemberName] string callerName = null)
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(1000, 1000));
                    page.Margin(20);
                    
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(16));
                    
                    page.Content().Element(contentBuilder);
                });
            })
            .ShouldMatchExpectedImage(callerName);
    }
}