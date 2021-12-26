using System.Drawing;
using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;
using Size = QuestPDF.Infrastructure.Size;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class BorderTests
    {
        [Test]
        public void Measure() => SimpleContainerTests.Measure<Border>();
        
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
                .ExpectChildMeasure(expectedInput: new Size(400, 300), returns: SpacePlan.FullRender(new Size(100, 50)))
                .CheckMeasureResult( SpacePlan.FullRender(new Size(100, 50)));
        }
        
        [Test]
        public void Draw_SameWidths_Optimized()
        {
            TestPlan
                .For(x => new Border
                {
                    Top = 10,
                    Right = 10,
                    Bottom = 10,
                    Left = 10,
                    
                    Color = Colors.Red.Medium,
                    
                    Child = x.CreateChild()
                })
                .DrawElement(new Size(400, 300))
                .ExpectChildDraw(new Size(400, 300))
                .ExpectCanvasDrawStrokedRectangle(new Size(400, 300), Colors.Red.Medium, 10)
                .ExpectChildDraw(new Size(400, 300))
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_VariousWidths()
        {
            TestPlan
                .For(x => new Border
                {
                    Top = 10,
                    Right = 20,
                    Bottom = 30,
                    Left = 40,
                    
                    Color = Colors.Red.Medium,
                    
                    Child = x.CreateChild()
                })
                .DrawElement(new Size(400, 300))
                .ExpectChildDraw(new Size(400, 300))
                .ExpectCanvasDrawFilledRectangle(new Position(-20, -5), new Size(430, 10), Colors.Red.Medium) // top
                .ExpectCanvasDrawFilledRectangle(new Position(-20, -5), new Size(40, 320), Colors.Red.Medium) // left
                .ExpectCanvasDrawFilledRectangle(new Position(-20, 285), new Size(430, 30), Colors.Red.Medium) // bottom
                .ExpectCanvasDrawFilledRectangle(new Position(390, -5), new Size(20, 320), Colors.Red.Medium) // right
                .ExpectChildDraw(new Size(400, 300))
                .CheckDrawResult();
        }
    }
}