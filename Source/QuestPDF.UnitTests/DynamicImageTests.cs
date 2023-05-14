using FluentAssertions;
using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;
using SkiaSharp;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class DynamicImageTests
    {
        [Test]
        public void Measure_TakesAvailableSpaceRegardlessOfSize()
        {
            TestPlan
                .For(x => new DynamicImage
                {
                    TargetDpi = DocumentSettings.DefaultRasterDpi,
                    CompressionQuality = ImageCompressionQuality.High,
                    Source = GenerateImage
                })
                .MeasureElement(new Size(300, 200))
                .CheckMeasureResult(SpacePlan.FullRender(300, 200));
        }
        
        [Test]
        public void Draw_HandlesNull()
        {
            TestPlan
                .For(x => new DynamicImage
                {
                    TargetDpi = DocumentSettings.DefaultRasterDpi,
                    CompressionQuality = ImageCompressionQuality.High,
                    Source = size => null
                })
                .DrawElement(new Size(300, 200))
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_PreservesSize()
        {
            TestPlan
                .For(x => new DynamicImage
                {
                    TargetDpi = DocumentSettings.DefaultRasterDpi,
                    CompressionQuality = ImageCompressionQuality.High,
                    Source = GenerateImage
                })
                .DrawElement(new Size(300, 200))
                .ExpectCanvasDrawImage(Position.Zero, new Size(300, 200))
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_PassesCorrectSizeToSource()
        {
            ImageSize passedSize = default;

            TestPlan
                .For(x => new DynamicImage
                {
                    TargetDpi = DocumentSettings.DefaultRasterDpi * 3,
                    CompressionQuality = ImageCompressionQuality.High,
                    Source = size =>
                    {
                        passedSize = size;
                        return GenerateImage(size);
                    }
                })
                .DrawElement(new Size(400, 300))
                .ExpectCanvasDrawImage(Position.Zero, new Size(400, 300))
                .CheckDrawResult();
            
            passedSize.Width.Should().Be(1200);
            passedSize.Height.Should().Be(900);
        }
        
        byte[] GenerateImage(ImageSize size)
        {
            var image = GenerateImage(size.Width, size.Height);
            return image.Encode(SKEncodedImageFormat.Png, 100).ToArray();
        }
        
        SKImage GenerateImage(int width, int height)
        {
            var imageInfo = new SKImageInfo(width, height);
            using var surface = SKSurface.Create(imageInfo);
            return surface.Snapshot();
        }
    }
}