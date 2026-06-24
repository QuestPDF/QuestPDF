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
    private const int PixelTolerance = 16;
    private const double MaxToleratedDifferentPixelsPercentage = 1;
    
    public static bool AreImagesSimilar(SKBitmap bitmap1, SKBitmap bitmap2)
    {
        if (bitmap1.Width != bitmap2.Width || bitmap1.Height != bitmap2.Height)
        {
            Assert.Fail("Different image sizes: " +
                        $"Image 1: {bitmap1.Width}x{bitmap1.Height}, " +
                        $"Image 2: {bitmap2.Width}x{bitmap2.Height}");
        }

        var pixels1 = bitmap1.Pixels;
        var pixels2 = bitmap2.Pixels;

        if (pixels1.Length != pixels2.Length)
        {
            Assert.Fail("Different image pixel counts: " +
                        $"Image 1: {pixels1.Length}, " +
                        $"Image 2: {pixels2.Length}");
        }
        
        var differentPixels = 0;
        var toleratedDifferentPixels = 0;
        var significantDifferentPixels = 0;
        var minDifference = int.MaxValue;
        var maxDifference = 0;
        var totalDifference = 0L;

        for (var i = 0; i < pixels1.Length; i++)
        {
            if (pixels1[i] != pixels2[i])
            {
                var difference = GetPixelDifference(pixels1[i], pixels2[i]);
                
                differentPixels++;
                minDifference = Math.Min(minDifference, difference);
                maxDifference = Math.Max(maxDifference, difference);
                totalDifference += difference;

                if (difference <= PixelTolerance)
                    toleratedDifferentPixels++;
                else
                    significantDifferentPixels++;
            }
        }

        if (differentPixels == 0)
            return true;

        var differentPixelsPercentage = differentPixels / (double) pixels1.Length * 100;
        var toleratedDifferentPixelsPercentage = toleratedDifferentPixels / (double) pixels1.Length * 100;
        var averageDifference = totalDifference / (double) differentPixels;

        if (significantDifferentPixels > 0 || toleratedDifferentPixelsPercentage > MaxToleratedDifferentPixelsPercentage)
        {
            var message =
                $"Images differ by {minDifference} (min), {maxDifference} (max), {averageDifference:F2} (avg). " +
                $"Different pixels: {differentPixels} ({differentPixelsPercentage:F4}%). " +
                $"Pixels within tolerance {PixelTolerance}: {toleratedDifferentPixels} ({toleratedDifferentPixelsPercentage:F4}%). " +
                $"Pixels outside tolerance {PixelTolerance}: {significantDifferentPixels}. " +
                $"Allowed pixels within tolerance: {MaxToleratedDifferentPixelsPercentage:F4}%.";
            
            Assert.Fail(message);
        }
        
        return true;
    }
    
    public static bool AreImagesSimilar(byte[] imageData1, byte[] imageData2)
    {
        using var bitmap1 = SKBitmap.Decode(imageData1);
        using var bitmap2 = SKBitmap.Decode(imageData2);
        
        return AreImagesSimilar(bitmap1, bitmap2);
    }

    private static int GetPixelDifference(SKColor pixel1, SKColor pixel2)
    {
        return new[]
        {
            Math.Abs(pixel1.Red - pixel2.Red),
            Math.Abs(pixel1.Green - pixel2.Green),
            Math.Abs(pixel1.Blue - pixel2.Blue),
            Math.Abs(pixel1.Alpha - pixel2.Alpha)
        }.Max();
    }
}

public static class ImageNormalizer
{
    public static byte[] DownsampleFromDoubleResolution(byte[] imageData)
    {
        using var sourceBitmap = SKBitmap.Decode(imageData);
        using var normalizedBitmap = DownsampleFromDoubleResolution(sourceBitmap);
        using var normalizedImage = SKImage.FromBitmap(normalizedBitmap);
        using var normalizedImageData = normalizedImage.Encode(SKEncodedImageFormat.Png, 100);
        
        return normalizedImageData.ToArray();
    }
    
    private static SKBitmap DownsampleFromDoubleResolution(SKBitmap sourceBitmap)
    {
        var targetWidth = (sourceBitmap.Width + 1) / 2;
        var targetHeight = (sourceBitmap.Height + 1) / 2;
        var targetBitmap = new SKBitmap(new SKImageInfo(targetWidth, targetHeight, sourceBitmap.ColorType, sourceBitmap.AlphaType));

        for (var y = 0; y < targetHeight; y++)
        {
            for (var x = 0; x < targetWidth; x++)
            {
                var topLeft = GetSourcePixelOrWhite(sourceBitmap, x * 2, y * 2);
                var topRight = GetSourcePixelOrWhite(sourceBitmap, x * 2 + 1, y * 2);
                var bottomLeft = GetSourcePixelOrWhite(sourceBitmap, x * 2, y * 2 + 1);
                var bottomRight = GetSourcePixelOrWhite(sourceBitmap, x * 2 + 1, y * 2 + 1);
                
                targetBitmap.SetPixel(x, y, Average(topLeft, topRight, bottomLeft, bottomRight));
            }
        }

        return targetBitmap;
    }

    private static SKColor GetSourcePixelOrWhite(SKBitmap bitmap, int x, int y)
    {
        if (x >= bitmap.Width || y >= bitmap.Height)
            return SKColors.White;
        
        return bitmap.GetPixel(x, y);
    }
    
    private static SKColor Average(SKColor pixel1, SKColor pixel2, SKColor pixel3, SKColor pixel4)
    {
        return new SKColor(
            AverageChannel(pixel1.Red, pixel2.Red, pixel3.Red, pixel4.Red),
            AverageChannel(pixel1.Green, pixel2.Green, pixel3.Green, pixel4.Green),
            AverageChannel(pixel1.Blue, pixel2.Blue, pixel3.Blue, pixel4.Blue),
            AverageChannel(pixel1.Alpha, pixel2.Alpha, pixel3.Alpha, pixel4.Alpha));
    }
    
    private static byte AverageChannel(byte value1, byte value2, byte value3, byte value4)
    {
        return (byte) ((value1 + value2 + value3 + value4 + 2) / 4);
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
            RasterDpi = 288
        };
        
        var actualImages = document
            .GenerateImages(imageGenerationSettings)
            .Select(ImageNormalizer.DownsampleFromDoubleResolution)
            .ToList();
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

            var imagesAreSimilar = ImageComparer.AreImagesSimilar(actualImageBytes, expectedImageBytes);

            if (imagesAreSimilar) 
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
