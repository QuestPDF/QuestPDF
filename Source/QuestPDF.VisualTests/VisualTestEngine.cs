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
    private const int MaxPixelChannelDifference = 8;
    private const double MaxDifferentPixelsRatio = 0.0005;
    
    public static bool AreImagesIdentical(SKBitmap actualImage, SKBitmap expectedImage)
    {
        EnsureComparableImages(actualImage, expectedImage);
        
        var comparison = ComparePixels(actualImage.Pixels, expectedImage.Pixels);
        
        if (!comparison.IsAccepted)
            Assert.Fail(comparison.GetFailureMessage());
        
        return true;
    }
    
    public static bool AreImagesIdentical(byte[] imageData1, byte[] imageData2)
    {
        using var bitmap1 = SKBitmap.Decode(imageData1);
        using var bitmap2 = SKBitmap.Decode(imageData2);
        
        return AreImagesIdentical(bitmap1, bitmap2);
    }
    
    private static void EnsureComparableImages(SKBitmap actualImage, SKBitmap expectedImage)
    {
        if (actualImage.Width != expectedImage.Width || actualImage.Height != expectedImage.Height)
        {
            Assert.Fail("Different image sizes: " +
                        $"Actual image: {actualImage.Width}x{actualImage.Height}, " +
                        $"Expected image: {expectedImage.Width}x{expectedImage.Height}");
        }
        
        if (actualImage.ColorType != expectedImage.ColorType)
        {
            Assert.Fail("Different image color types: " +
                        $"Actual image: {actualImage.ColorType}, " +
                        $"Expected image: {expectedImage.ColorType}");
        }
    }
    
    private static ImageComparisonResult ComparePixels(SKColor[] actualPixels, SKColor[] expectedPixels)
    {
        if (actualPixels.Length != expectedPixels.Length)
        {
            Assert.Fail("Different image pixel counts: " +
                        $"Actual image: {actualPixels.Length}, " +
                        $"Expected image: {expectedPixels.Length}");
        }
        
        var statistics = DifferenceStatistics.Calculate(actualPixels, expectedPixels, MaxPixelChannelDifference);
        return new ImageComparisonResult(statistics);
    }
    
    private static int GetPixelDifference(SKColor actualPixel, SKColor expectedPixel)
    {
        return Math.Max(
            Math.Max(Math.Abs(actualPixel.Red - expectedPixel.Red), Math.Abs(actualPixel.Green - expectedPixel.Green)),
            Math.Max(Math.Abs(actualPixel.Blue - expectedPixel.Blue), Math.Abs(actualPixel.Alpha - expectedPixel.Alpha)));
    }
    
    private static string FormatPercentage(double value)
    {
        return value.ToString("P4", CultureInfo.InvariantCulture);
    }
    
    private sealed record ImageComparisonResult(DifferenceStatistics Statistics)
    {
        public bool IsAccepted => 
            Statistics.MaxDifference <= MaxPixelChannelDifference &&
            Statistics.DifferentPixelsRatio <= MaxDifferentPixelsRatio;
        
        public string GetFailureMessage()
        {
            return "Images differ. " +
                   $"Allowed max pixel channel difference: {MaxPixelChannelDifference}. \n" +
                   $"Allowed different pixels: {FormatPercentage(MaxDifferentPixelsRatio)}. \n" +
                   $"Actual different pixels: {Statistics.DifferentPixels} ({FormatPercentage(Statistics.DifferentPixelsRatio)}). \n" +
                   $"Pixels above channel tolerance: {Statistics.PixelsAboveTolerance} ({FormatPercentage(Statistics.PixelsAboveToleranceRatio)}). \n" +
                   $"Difference: {Statistics.MinDifference} (min), {Statistics.MaxDifference} (max), {Statistics.AverageDifference:F2} (avg).";
        }
    }
    
    private sealed class DifferenceStatistics
    {
        public int TotalPixels { get; private init; }
        public int DifferentPixels { get; private init; }
        public int PixelsAboveTolerance { get; private init; }
        public int MinDifference { get; private init; }
        public int MaxDifference { get; private init; }
        public double AverageDifference { get; private init; }
        
        public double DifferentPixelsRatio => TotalPixels == 0 ? 0 : (double) DifferentPixels / TotalPixels;
        public double PixelsAboveToleranceRatio => TotalPixels == 0 ? 0 : (double) PixelsAboveTolerance / TotalPixels;
        
        public static DifferenceStatistics Calculate(SKColor[] actualPixels, SKColor[] expectedPixels, int maxPixelChannelDifference)
        {
            var differentPixels = 0;
            var pixelsAboveTolerance = 0;
            var minDifference = int.MaxValue;
            var maxDifference = 0;
            var differenceSum = 0L;
            
            for (var i = 0; i < actualPixels.Length; i++)
            {
                var difference = GetPixelDifference(actualPixels[i], expectedPixels[i]);
                
                if (difference == 0)
                    continue;
                
                differentPixels++;
                differenceSum += difference;
                minDifference = Math.Min(minDifference, difference);
                maxDifference = Math.Max(maxDifference, difference);
                
                if (difference > maxPixelChannelDifference)
                    pixelsAboveTolerance++;
            }
            
            return new DifferenceStatistics
            {
                TotalPixels = actualPixels.Length,
                DifferentPixels = differentPixels,
                PixelsAboveTolerance = pixelsAboveTolerance,
                MinDifference = differentPixels == 0 ? 0 : minDifference,
                MaxDifference = maxDifference,
                AverageDifference = differentPixels == 0 ? 0 : (double) differenceSum / differentPixels
            };
        }
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
