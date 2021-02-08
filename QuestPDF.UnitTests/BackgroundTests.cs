using NUnit.Framework;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class BackgroundTests
    {
        [Test]
        public void Measure_ShouldHandleNullChild() => new Background().MeasureWithoutChild();

        [Test]
        public void Draw_ShouldHandleNullChild()
        {
            TestPlan
                .For(x => new Background
                {
                    Color = "#F00"
                })
                .DrawElement(new Size(400, 300))
                .ExpectCanvasDrawRectangle(new Position(0, 0), new Size(400, 300), "#F00")
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_ShouldHandleChild()
        {
            TestPlan
                .For(x => new Background
                {
                    Color = "#F00",
                    Child = x.CreateChild("a")
                })
                .DrawElement(new Size(400, 300))
                .ExpectCanvasDrawRectangle(new Position(0, 0), new Size(400, 300), "#F00")
                .ExpectChildDraw("a", new Size(400, 300))
                .CheckDrawResult();
        }
    }
}