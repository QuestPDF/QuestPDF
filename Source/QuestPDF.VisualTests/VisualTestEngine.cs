using System.Text.RegularExpressions;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.VisualTests;

public static class Helpers
{
    public static TOutput Apply<TInput, TOutput>(this TInput input, Func<TInput, TOutput> func)
    {
        return func(input);
    }
}

public class ImageComparer
{
    public static bool AreImagesIdentical(SKBitmap bitmap1, SKBitmap bitmap2)
    {
        if (bitmap1.Width != bitmap2.Width || bitmap1.Height != bitmap2.Height)
            return false;
            
        if (bitmap1.ColorType != bitmap2.ColorType)
            return false;

        var pixels1 = bitmap1.Pixels;
        var pixels2 = bitmap2.Pixels;
        
        if (pixels1.Length != pixels2.Length)
            return false;
            
        var differences = pixels1.Zip(pixels2, (p1, p2) => new[] {p1.Red - p2.Red, p1.Green - p2.Green, p1.Blue - p2.Blue, p1.Alpha - p2.Alpha })
            .Select(x => x.Max(Math.Abs))
            .Where(diff => diff > 0)
            .ToArray();
        
        var min = differences.Min();
        var max = differences.Max();
        var average = differences.Average(x => x);

        if (differences.Length > 0)
        {
            Console.WriteLine($"Image differences: Count={differences.Length}, Min={min}, Max={max}, Average={average}");
        }
        
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
    private static string ActualOutputDirectoryName => Path.Combine(TestContext.CurrentContext.TestDirectory, "ActualOutput");
    private static string ExpectedOutputDirectoryName => Path.Combine(TestContext.CurrentContext.TestDirectory, "ExpectedOutput");
    
    private static readonly Regex TestNameRegex = new(@"QuestPDF\.VisualTests\.(?<name>.*)Tests");
    
    public static void ClearActualOutputDirectories()
    {
        if (Directory.Exists(ActualOutputDirectoryName))
            Directory.Delete(ActualOutputDirectoryName, true);
    }
    
    public static void ShouldMatchExpectedImage(this IDocument document)
    {
        if (TestContext.CurrentContext.Test.ClassName == null)
            throw new Exception("Test class name is not set.");
        
        var match = TestNameRegex.Match(TestContext.CurrentContext.Test.ClassName);
        var testCategory = match.Groups["name"].Value;
        
        var imageGenerationSettings = new ImageGenerationSettings
        {
            ImageFormat = ImageFormat.Png,
            RasterDpi = 144
        };
        
        var actualImages = document.GenerateImages(imageGenerationSettings).ToList();
        var hasMultipleImages = actualImages.Count > 1;
        
        var actualOutputPath = Path.Combine(ActualOutputDirectoryName, testCategory);
        var expectedOutputPath = Path.Combine(ExpectedOutputDirectoryName, testCategory);
        
        if (!Directory.Exists(expectedOutputPath))
            Assert.Inconclusive("Cannot find the expected output folder");
        
        var testName = TestContext.CurrentContext.Test.Name;
        var expectedOutputFileCount = Directory.EnumerateFiles(expectedOutputPath, $"{testName}*.png").Count();
        
        Directory.CreateDirectory(actualOutputPath);

        string GetFileName(int index)
        {
            return hasMultipleImages ? $"{testName}_{index}.png" : $"{testName}.png";
        }
        
        foreach (var i in Enumerable.Range(0, actualImages.Count))
        {
            var actualImagePath = Path.Combine(actualOutputPath, GetFileName(i));
            File.WriteAllBytes(actualImagePath, actualImages[i]);
        }
        
        if (actualImages.Count != expectedOutputFileCount)
            Assert.Fail($"Generated {actualImages.Count} images but expected {expectedOutputFileCount}");

        foreach (var i in Enumerable.Range(0, actualImages.Count))
        {
            var expectedImagePath = Path.Combine(expectedOutputPath, GetFileName(i));
            
            if (!File.Exists(expectedImagePath))
                Assert.Fail($"Cannot find expected image file {expectedImagePath}");
            
            var expectedImageBytes = File.ReadAllBytes(expectedImagePath);
            var actualImageBytes = actualImages[i];

            var imagesAreIdentical = ImageComparer.AreImagesIdentical(actualImageBytes, expectedImageBytes);

            if (imagesAreIdentical) 
                continue;
            
            var pageText = actualImages.Count > 1 ? $" (page {i})" : string.Empty;
            Assert.Fail($"Generated image does not match expected image{pageText}.");
        }
    }
}

public static class VisualTest
{
    public static void Perform(Action<IDocumentContainer> documentBuilder)
    {
        Document
            .Create(documentBuilder)
            .ShouldMatchExpectedImage();
    }
    
    public static void PerformWithDefaultPageSettings(Action<IContainer> contentBuilder)
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
            .ShouldMatchExpectedImage();
    }
}