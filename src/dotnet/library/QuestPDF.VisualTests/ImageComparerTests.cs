using SkiaSharp;

namespace QuestPDF.VisualTests;

public class ImageComparerTests
{
    [Test]
    public void ShouldAcceptIdenticalImages()
    {
        using var image1 = CreateImage(100, 100, SKColors.White);
        using var image2 = CreateImage(100, 100, SKColors.White);
        
        Assert.That(ImageComparer.AreImagesIdentical(image1, image2), Is.True);
    }
    
    [Test]
    public void ShouldAcceptSmallNumberOfDifferentPixelsWithinTolerance()
    {
        using var image1 = CreateImage(100, 100, SKColors.White);
        using var image2 = CreateImage(100, 100, SKColors.White);
        
        for (var i = 0; i < 5; i++)
            image2.SetPixel(i, 0, new SKColor(247, 247, 247));
        
        Assert.That(ImageComparer.AreImagesIdentical(image1, image2), Is.True);
    }
    
    [Test]
    public void ShouldRejectPixelsAboveColorTolerance()
    {
        using var image1 = CreateImage(100, 100, SKColors.White);
        using var image2 = CreateImage(100, 100, SKColors.White);
        
        image2.SetPixel(0, 0, new SKColor(246, 246, 246));
        
        Assert.Throws<AssertionException>(() => ImageComparer.AreImagesIdentical(image1, image2));
    }
    
    [Test]
    public void ShouldRejectTooManyDifferentPixels()
    {
        using var image1 = CreateImage(100, 100, SKColors.White);
        using var image2 = CreateImage(100, 100, SKColors.White);
        
        for (var i = 0; i < 6; i++)
            image2.SetPixel(i, 0, new SKColor(247, 247, 247));
        
        Assert.Throws<AssertionException>(() => ImageComparer.AreImagesIdentical(image1, image2));
    }
    
    [Test]
    public void ShouldRejectDifferentImageSizes()
    {
        using var image1 = CreateImage(100, 100, SKColors.White);
        using var image2 = CreateImage(101, 100, SKColors.White);
        
        Assert.Throws<AssertionException>(() => ImageComparer.AreImagesIdentical(image1, image2));
    }
    
    private static SKBitmap CreateImage(int width, int height, SKColor color)
    {
        var bitmap = new SKBitmap(width, height);
        bitmap.Erase(color);
        
        return bitmap;
    }
}
