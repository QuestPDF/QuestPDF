using FluentAssertions;
using NUnit.Framework;
using QuestPDF.Drawing.SpacePlan;
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
                    Source = GenerateImage
                })
                .MeasureElement(new Size(300, 200))
                .CheckMeasureResult(new FullRender(300, 200));
        }
        
        [Test]
        public void Draw_HandlesNull()
        {
            TestPlan
                .For(x => new DynamicImage
                {
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
                    Source = GenerateImage
                })
                .DrawElement(new Size(300, 200))
                .ExpectCanvasDrawImage(Position.Zero, new Size(300, 200))
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_PassesCorrectSizeToSource()
        {
            Size passedSize = null;

            TestPlan
                .For(x => new DynamicImage
                {
                    Source = size =>
                    {
                        passedSize = size;
                        return GenerateImage(size);
                    }
                })
                .DrawElement(new Size(400, 300))
                .ExpectCanvasDrawImage(Position.Zero, new Size(400, 300))
                .CheckDrawResult();
            
            passedSize.Should().BeEquivalentTo(new Size(400, 300));
        }
        
        byte[] GenerateImage(Size size)
        {
            var image = GenerateImage((int) size.Width, (int) size.Height);
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