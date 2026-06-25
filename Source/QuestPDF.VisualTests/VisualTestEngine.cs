using System.Globalization;
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

public static class ImageComparer
{
    private const int PixelTolerance = 8;
    private const double MaxDifferentPixelsRatio = 0.0005;
    
    public static bool AreImagesIdentical(SKBitmap bitmap1, SKBitmap bitmap2)
    {
        if (bitmap1.Width != bitmap2.Width || bitmap1.Height != bitmap2.Height)
        {
            Assert.Fail("Different image sizes: " +
                        $"Image 1: {bitmap1.Width}x{bitmap1.Height}, " +
                        $"Image 2: {bitmap2.Width}x{bitmap2.Height}");
        }

        if (bitmap1.ColorType != bitmap2.ColorType)
        {
            Assert.Fail("Different image color types: " +
                        $"Image 1: {bitmap1.ColorType}, " +
                        $"Image 2: {bitmap2.ColorType}");
        }

        var pixels1 = bitmap1.Pixels;
        var pixels2 = bitmap2.Pixels;

        if (pixels1.Length != pixels2.Length)
        {
            Assert.Fail("Different image pixel counts: " +
                        $"Image 1: {pixels1.Length}, " +
                        $"Image 2: {pixels2.Length}");
        }
        
        var statistics = CalculateDifferenceStatistics(pixels1, pixels2);
        
        if (!statistics.IsAccepted)
        {
            var message = "Images differ. " +
                          $"Allowed max pixel channel difference: {PixelTolerance}. " +
                          $"Allowed different pixels: {FormatPercentage(MaxDifferentPixelsRatio)}. " +
                          $"Actual different pixels: {statistics.DifferentPixels} ({FormatPercentage(statistics.DifferentPixelsRatio)}). " +
                          $"Pixels above channel tolerance: {statistics.PixelsAboveTolerance} ({FormatPercentage(statistics.PixelsAboveToleranceRatio)}). " +
                          $"Difference: {statistics.MinDifference} (min), {statistics.MaxDifference} (max), {statistics.AverageDifference:F2} (avg).";
            
            Assert.Fail(message);
        }
        
        return true;
    }
    
    public static bool AreImagesIdentical(byte[] imageData1, byte[] imageData2)
    {
        using var bitmap1 = SKBitmap.Decode(imageData1);
        using var bitmap2 = SKBitmap.Decode(imageData2);
        
        return AreImagesIdentical(bitmap1, bitmap2);
    }
    
    private static DifferenceStatistics CalculateDifferenceStatistics(SKColor[] pixels1, SKColor[] pixels2)
    {
        var differentPixels = 0;
        var pixelsAboveTolerance = 0;
        var minDifference = int.MaxValue;
        var maxDifference = 0;
        var differenceSum = 0L;
        
        for (var i = 0; i < pixels1.Length; i++)
        {
            var difference = GetPixelDifference(pixels1[i], pixels2[i]);
            
            if (difference == 0)
                continue;
            
            differentPixels++;
            differenceSum += difference;
            minDifference = Math.Min(minDifference, difference);
            maxDifference = Math.Max(maxDifference, difference);
            
            if (difference > PixelTolerance)
                pixelsAboveTolerance++;
        }
        
        return new DifferenceStatistics
        {
            TotalPixels = pixels1.Length,
            DifferentPixels = differentPixels,
            PixelsAboveTolerance = pixelsAboveTolerance,
            MinDifference = differentPixels == 0 ? 0 : minDifference,
            MaxDifference = maxDifference,
            AverageDifference = differentPixels == 0 ? 0 : (double) differenceSum / differentPixels
        };
    }
    
    private static int GetPixelDifference(SKColor pixel1, SKColor pixel2)
    {
        return Math.Max(
            Math.Max(Math.Abs(pixel1.Red - pixel2.Red), Math.Abs(pixel1.Green - pixel2.Green)),
            Math.Max(Math.Abs(pixel1.Blue - pixel2.Blue), Math.Abs(pixel1.Alpha - pixel2.Alpha)));
    }
    
    private static string FormatPercentage(double value)
    {
        return value.ToString("P4", CultureInfo.InvariantCulture);
    }
    
    private sealed class DifferenceStatistics
    {
        public int TotalPixels { get; init; }
        public int DifferentPixels { get; init; }
        public int PixelsAboveTolerance { get; init; }
        public int MinDifference { get; init; }
        public int MaxDifference { get; init; }
        public double AverageDifference { get; init; }
        
        public double DifferentPixelsRatio => TotalPixels == 0 ? 0 : (double) DifferentPixels / TotalPixels;
        public double PixelsAboveToleranceRatio => TotalPixels == 0 ? 0 : (double) PixelsAboveTolerance / TotalPixels;
        
        public bool IsAccepted => 
            MaxDifference <= PixelTolerance &&
            DifferentPixelsRatio <= MaxDifferentPixelsRatio;
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
        
        var testName = TestContext.CurrentContext.Test.Name;
        
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
        
        if (!Directory.Exists(expectedOutputPath))
            Assert.Inconclusive("Cannot find the expected output folder");
        
        var expectedOutputFileCount = Directory.EnumerateFiles(expectedOutputPath, $"{testName}*.png").Count();
        
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
        SetUpCultureInfoToInvariant();
        
        Document
            .Create(documentBuilder)
            .ShouldMatchExpectedImage();
    }
    
    public static void PerformWithDefaultPageSettings(Action<IContainer> contentBuilder)
    {
        SetUpCultureInfoToInvariant();
        
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

    private static void SetUpCultureInfoToInvariant()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
    }
}
