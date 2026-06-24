using SkiaSharp;

namespace QuestPDF.VisualTests;

public class ImageComparerTests
{
    [Test]
    public void ShouldAcceptIdenticalImages()
    {
        using var image1 = CreateBitmap(10, 10);
        using var image2 = CreateBitmap(10, 10);
        
        Assert.That(ImageComparer.AreImagesSimilar(image1, image2), Is.True);
    }
    
    [Test]
    public void ShouldAcceptSmallNumberOfPixelsWithinColorTolerance()
    {
        using var image1 = CreateBitmap(10, 10);
        using var image2 = CreateBitmap(10, 10);
        
        image1.SetPixel(5, 5, new SKColor(100, 100, 100));
        image2.SetPixel(5, 5, new SKColor(108, 108, 108));
        
        Assert.That(ImageComparer.AreImagesSimilar(image1, image2), Is.True);
    }
    
    [Test]
    public void ShouldAcceptManyPixelsWithinColorTolerance()
    {
        using var image1 = CreateBitmap(10, 10);
        using var image2 = CreateBitmap(10, 10);

        for (var x = 0; x < 11; x++)
        {
            image1.SetPixel(x % 10, x / 10, new SKColor(100, 100, 100));
            image2.SetPixel(x % 10, x / 10, new SKColor(108, 108, 108));
        }
        
        Assert.That(ImageComparer.AreImagesSimilar(image1, image2), Is.True);
    }
    
    [Test]
    public void ShouldAcceptSmallNumberOfPixelsMovedByOnePixel()
    {
        using var image1 = CreateBitmap(30, 30);
        using var image2 = CreateBitmap(30, 30);
        
        image1.SetPixel(15, 15, SKColors.Black);
        image2.SetPixel(16, 15, SKColors.Black);
        
        Assert.That(ImageComparer.AreImagesSimilar(image1, image2), Is.True);
    }
    
    [Test]
    public void ShouldAcceptPixelWithinFuzzyNeighborhoodRange()
    {
        using var image1 = CreateBitmap(30, 30);
        using var image2 = CreateBitmap(30, 30);
        
        image1.SetPixel(15, 15, new SKColor(128, 128, 128));
        image1.SetPixel(14, 15, new SKColor(92, 92, 92));
        image2.SetPixel(15, 15, new SKColor(100, 100, 100));
        
        Assert.That(ImageComparer.AreImagesSimilar(image1, image2), Is.True);
    }
    
    [Test]
    public void ShouldAcceptManyPixelsMovedByOnePixel()
    {
        using var image1 = CreateBitmap(20, 20);
        using var image2 = CreateBitmap(20, 20);

        for (var y = 0; y < 20; y++)
        {
            image1.SetPixel(9, y, SKColors.Black);
            image2.SetPixel(10, y, SKColors.Black);
        }
        
        Assert.That(ImageComparer.AreImagesSimilar(image1, image2), Is.True);
    }
    
    [Test]
    public void ShouldAcceptDifferentSizesWhenMissingPixelsAreWhite()
    {
        using var image1 = CreateBitmap(10, 10);
        using var image2 = CreateBitmap(11, 10);
        
        Assert.That(ImageComparer.AreImagesSimilar(image1, image2), Is.True);
    }
    
    [Test]
    public void ShouldRejectDifferentSizesOutsideSizeTolerance()
    {
        using var image1 = CreateBitmap(10, 10);
        using var image2 = CreateBitmap(12, 10);
        
        Assert.That(() => ImageComparer.AreImagesSimilar(image1, image2), Throws.TypeOf<AssertionException>());
    }
    
    [Test]
    public void ShouldRejectPixelWithoutNearbyMatch()
    {
        using var image1 = CreateBitmap(30, 30);
        using var image2 = CreateBitmap(30, 30);

        for (var x = 0; x < 10; x++)
        {
            image1.SetPixel(x, 15, SKColors.Black);
        }
        
        Assert.That(() => ImageComparer.AreImagesSimilar(image1, image2), Throws.TypeOf<AssertionException>());
    }
    
    private static SKBitmap CreateBitmap(int width, int height)
    {
        var bitmap = new SKBitmap(width, height);
        bitmap.Erase(SKColors.White);
        return bitmap;
    }
}
