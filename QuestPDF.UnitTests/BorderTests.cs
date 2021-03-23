using NUnit.Framework;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class BorderTests
    {
        [Test]
        public void Measure_ShouldHandleNullChild() => new Border().MeasureWithoutChild();

        [Test]
        public void ComponentShouldNotAffectLayout()
        {
            TestPlan
                .For(x => new Border
                {
                    Top = 10,
                    Right = 20,
                    Bottom = 30,
                    Left = 40,
                    
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure(expectedInput: new Size(400, 300), returns: new FullRender(new Size(100, 50)))
                .CheckMeasureResult( new FullRender(new Size(100, 50)));
        }
        
        [Test]
        public void Draw_HorizontalRight_VerticalTop()
        {
            TestPlan
                .For(x => new Border
                {
                    Top = 10,
                    Right = 20,
                    Bottom = 30,
                    Left = 40,
                    
                    Color = "#FF0000",
                    
                    Child = x.CreateChild()
                })
                .DrawElement(new Size(400, 300))
                .ExpectChildDraw(new Size(400, 300))
                .ExpectCanvasDrawRectangle(new Position(-20, -5), new Size(430, 10), "#FF0000") // top
                .ExpectCanvasDrawRectangle(new Position(-20, -5), new Size(40, 320), "#FF0000") // left
                .ExpectCanvasDrawRectangle(new Position(-20, 285), new Size(430, 30), "#FF0000") // bottom
                .ExpectCanvasDrawRectangle(new Position(390, -5), new Size(20, 320), "#FF0000") // right
                .ExpectChildDraw(new Size(400, 300))
                .CheckDrawResult();
        }
    }
}