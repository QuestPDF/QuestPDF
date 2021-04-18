using NUnit.Framework;
using QuestPDF.Elements;
using QuestPDF.Helpers;
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
                    Color = Colors.Red.Medium
                })
                .DrawElement(new Size(400, 300))
                .ExpectCanvasDrawRectangle(new Position(0, 0), new Size(400, 300), Colors.Red.Medium)
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_ShouldHandleChild()
        {
            TestPlan
                .For(x => new Background
                {
                    Color = Colors.Red.Medium,
                    Child = x.CreateChild()
                })
                .DrawElement(new Size(400, 300))
                .ExpectCanvasDrawRectangle(new Position(0, 0), new Size(400, 300), Colors.Red.Medium)
                .ExpectChildDraw(new Size(400, 300))
                .CheckDrawResult();
        }
    }
}