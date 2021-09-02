using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;
using SkiaSharp;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class ImageTests
    {
        [Test]
        public void Measure_ShouldHandleNullChild() => new AspectRatio().MeasureWithoutChild();
        
        [Test]
        public void Draw_ShouldHandleNullChild() => new AspectRatio().DrawWithoutChild();
        
        [Test]
        public void Measure_TakesAvailableSpaceRegardlessOfSize()
        {
            TestPlan
                .For(x => new Image
                {
                    InternalImage = GenerateImage(400, 300)
                })
                .MeasureElement(new Size(300, 200))
                .CheckMeasureResult(SpacePlan.FullRender(300, 200));
        }
        
        [Test]
        public void Draw_TakesAvailableSpaceRegardlessOfSize()
        {
            TestPlan
                .For(x => new Image
                {
                    InternalImage = GenerateImage(400, 300)
                })
                .DrawElement(new Size(300, 200))
                .ExpectCanvasDrawImage(new Position(0, 0), new Size(300, 200))
                .CheckDrawResult();
        }
        
        [Test]
        public void Fluent_RecognizesImageProportions()
        {
            var image = GenerateImage(600, 200).Encode(SKEncodedImageFormat.Png, 100).ToArray();
            
            TestPlan
                .For(x =>
                {
                    var container = new Container();
                    container.Image(image);
                    return container;
                })
                .MeasureElement(new Size(300, 200))
                .CheckMeasureResult(SpacePlan.FullRender(300, 100));;
        }
        
        SKImage GenerateImage(int width, int height)
        {
            var imageInfo = new SKImageInfo(width, height);
            using var surface = SKSurface.Create(imageInfo);
            return surface.Snapshot();
        }
    }
}