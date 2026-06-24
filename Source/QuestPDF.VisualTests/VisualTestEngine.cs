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
    private const int FuzzyPixelSearchRadius = 1;
    private const int ImageSizeTolerance = 1;
    private const double MaxFailedDifferentPixelsPercentage = 0;
    
    public static bool AreImagesSimilar(SKBitmap bitmap1, SKBitmap bitmap2)
    {
        if (Math.Abs(bitmap1.Width - bitmap2.Width) > ImageSizeTolerance ||
            Math.Abs(bitmap1.Height - bitmap2.Height) > ImageSizeTolerance)
        {
            Assert.Fail("Different image sizes: " +
                        $"Image 1: {bitmap1.Width}x{bitmap1.Height}, " +
                        $"Image 2: {bitmap2.Width}x{bitmap2.Height}. " +
                        $"Allowed size tolerance: {ImageSizeTolerance}px.");
        }
        
        var pixels1 = bitmap1.Pixels;
        var pixels2 = bitmap2.Pixels;
        var width = Math.Max(bitmap1.Width, bitmap2.Width);
        var height = Math.Max(bitmap1.Height, bitmap2.Height);
        
        var actualToExpected = ComparePixels(pixels1, bitmap1.Width, bitmap1.Height, pixels2, bitmap2.Width, bitmap2.Height, width, height);
        var expectedToActual = ComparePixels(pixels2, bitmap2.Width, bitmap2.Height, pixels1, bitmap1.Width, bitmap1.Height, width, height);
        
        if (actualToExpected.PixelsAreIdentical && expectedToActual.PixelsAreIdentical)
            return true;

        if (!actualToExpected.IsAccepted || !expectedToActual.IsAccepted)
        {
            var message =
                "Images differ. \n" +
                $"Image 1: {bitmap1.Width}x{bitmap1.Height}, image 2: {bitmap2.Width}x{bitmap2.Height}. \n" +
                $"Allowed fuzzy color tolerance: {PixelTolerance},\n" +
                $"allowed fuzzy radius: {FuzzyPixelSearchRadius}px. \n" +
                $"Allowed failed pixels: {MaxFailedDifferentPixelsPercentage:F4}%. \n" +
                $"Actual -> expected: {actualToExpected}. \n" +
                $"Expected -> actual: {expectedToActual}.";
            
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
    
    private static ImageComparisonResult ComparePixels(
        SKColor[] sourcePixels,
        int sourceWidth,
        int sourceHeight,
        SKColor[] targetPixels,
        int targetWidth,
        int targetHeight,
        int width,
        int height)
    {
        var differentPixels = 0;
        var exactPixels = 0;
        var fuzzyPixels = 0;
        var failedDifferentPixels = 0;
        var minDifference = int.MaxValue;
        var maxDifference = 0;
        var totalDifference = 0L;

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var sourcePixel = GetPixelOrWhite(sourcePixels, sourceWidth, sourceHeight, x, y);
                var targetPixel = GetPixelOrWhite(targetPixels, targetWidth, targetHeight, x, y);
                
                var difference = GetPixelDifference(sourcePixel, targetPixel);

                if (difference == 0)
                {
                    exactPixels++;
                    continue;
                }
                
                differentPixels++;
                minDifference = Math.Min(minDifference, difference);
                maxDifference = Math.Max(maxDifference, difference);
                totalDifference += difference;
                
                if (!FitsWithinFuzzyNeighborhoodRange(sourcePixel, targetPixels, targetWidth, targetHeight, width, height, x, y))
                {
                    failedDifferentPixels++;
                    continue;
                }
                
                fuzzyPixels++;
            }
        }

        return new ImageComparisonResult(
            width * height,
            exactPixels,
            differentPixels,
            fuzzyPixels,
            failedDifferentPixels,
            minDifference == int.MaxValue ? 0 : minDifference,
            maxDifference,
            differentPixels == 0 ? 0 : totalDifference / (double) differentPixels);
    }

    private static bool FitsWithinFuzzyNeighborhoodRange(
        SKColor sourcePixel,
        SKColor[] targetPixels,
        int targetWidth,
        int targetHeight,
        int width,
        int height,
        int x,
        int y)
    {
        var left = Math.Max(0, x - FuzzyPixelSearchRadius);
        var top = Math.Max(0, y - FuzzyPixelSearchRadius);
        var right = Math.Min(width - 1, x + FuzzyPixelSearchRadius);
        var bottom = Math.Min(height - 1, y + FuzzyPixelSearchRadius);

        var minRed = byte.MaxValue;
        var minGreen = byte.MaxValue;
        var minBlue = byte.MaxValue;
        var minAlpha = byte.MaxValue;
        
        var maxRed = byte.MinValue;
        var maxGreen = byte.MinValue;
        var maxBlue = byte.MinValue;
        var maxAlpha = byte.MinValue;
        
        for (var fuzzyY = top; fuzzyY <= bottom; fuzzyY++)
        {
            for (var fuzzyX = left; fuzzyX <= right; fuzzyX++)
            {
                var targetPixel = GetPixelOrWhite(targetPixels, targetWidth, targetHeight, fuzzyX, fuzzyY);
                
                minRed = Math.Min(minRed, targetPixel.Red);
                minGreen = Math.Min(minGreen, targetPixel.Green);
                minBlue = Math.Min(minBlue, targetPixel.Blue);
                minAlpha = Math.Min(minAlpha, targetPixel.Alpha);
                
                maxRed = Math.Max(maxRed, targetPixel.Red);
                maxGreen = Math.Max(maxGreen, targetPixel.Green);
                maxBlue = Math.Max(maxBlue, targetPixel.Blue);
                maxAlpha = Math.Max(maxAlpha, targetPixel.Alpha);
            }
        }

        return
            FitsWithinChannelRange(sourcePixel.Red, minRed, maxRed) &&
            FitsWithinChannelRange(sourcePixel.Green, minGreen, maxGreen) &&
            FitsWithinChannelRange(sourcePixel.Blue, minBlue, maxBlue) &&
            FitsWithinChannelRange(sourcePixel.Alpha, minAlpha, maxAlpha);
    }

    private static bool FitsWithinChannelRange(byte value, byte minValue, byte maxValue)
    {
        return value >= minValue - PixelTolerance && value <= maxValue + PixelTolerance;
    }

    private static SKColor GetPixelOrWhite(SKColor[] pixels, int width, int height, int x, int y)
    {
        if (x >= width || y >= height)
            return SKColors.White;
        
        return pixels[y * width + x];
    }
    
    private readonly record struct ImageComparisonResult(
        int AllPixels,
        int ExactPixels,
        int DifferentPixels,
        int FuzzyPixels,
        int FailedDifferentPixels,
        int MinDifference,
        int MaxDifference,
        double AverageDifference)
    {
        public bool PixelsAreIdentical => DifferentPixels == 0;
        
        public bool IsAccepted =>
            FailedDifferentPixelsPercentage <= MaxFailedDifferentPixelsPercentage;

        private double ExactPixelsPercentage => ExactPixels / (double) AllPixels * 100;
        private double DifferentPixelsPercentage => DifferentPixels / (double) AllPixels * 100;
        private double FuzzyPixelsPercentage => FuzzyPixels / (double) AllPixels * 100;
        private double FailedDifferentPixelsPercentage => FailedDifferentPixels / (double) AllPixels * 100;

        public override string ToString()
        {
            return
                $"exact pixels: {ExactPixels} ({ExactPixelsPercentage:F4}%), \n" +
                $"different pixels: {DifferentPixels} ({DifferentPixelsPercentage:F4}%), \n" +
                $"fuzzy pixels: {FuzzyPixels} ({FuzzyPixelsPercentage:F4}%), \n" +
                $"failed pixels: {FailedDifferentPixels} ({FailedDifferentPixelsPercentage:F4}%), \n" +
                $"difference: {MinDifference} (min), {MaxDifference} (max), {AverageDifference:F2} (avg)";
        }
    }

    private static int GetPixelDifference(SKColor pixel1, SKColor pixel2)
    {
        var redDifference = Math.Abs(pixel1.Red - pixel2.Red);
        var greenDifference = Math.Abs(pixel1.Green - pixel2.Green);
        var blueDifference = Math.Abs(pixel1.Blue - pixel2.Blue);
        var alphaDifference = Math.Abs(pixel1.Alpha - pixel2.Alpha);
        
        return Math.Max(
            Math.Max(redDifference, greenDifference),
            Math.Max(blueDifference, alphaDifference));
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
        
        var actualImages = document
            .GenerateImages(imageGenerationSettings)
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
