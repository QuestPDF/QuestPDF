using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class AlignmentTests
    {
        [Test]
        public void Measure() => SimpleContainerTests.Measure<Alignment>();
        
        [Test]
        public void Draw_HorizontalCenter_VerticalCenter()
        {
            TestPlan
                .For(x => new Alignment
                {
                    Horizontal = HorizontalAlignment.Center,
                    Vertical = VerticalAlignment.Middle,
                    
                    Child = x.CreateChild()
                })
                .DrawElement(new Size(1000, 500))
                .ExpectChildMeasure(expectedInput: new Size(1000, 500), returns: SpacePlan.PartialRender(new Size(400, 200)))
                .ExpectCanvasTranslate(new Position(300, 150))
                .ExpectChildDraw(new Size(400, 200))
                .ExpectCanvasTranslate(new Position(-300, -150))
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_HorizontalLeft_VerticalCenter()
        {
            TestPlan
                .For(x => new Alignment
                {
                    Horizontal = HorizontalAlignment.Left,
                    Vertical = VerticalAlignment.Middle,
                    
                    Child = x.CreateChild()
                })
                .DrawElement(new Size(400, 300))
                .ExpectChildMeasure(expectedInput: new Size(400, 300), returns: SpacePlan.FullRender(new Size(100, 50)))
                .ExpectCanvasTranslate(new Position(0, 125))
                .ExpectChildDraw(new Size(100, 50))
                .ExpectCanvasTranslate(new Position(0, -125))
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_HorizontalCenter_VerticalBottom()
        {
            TestPlan
                .For(x => new Alignment
                {
                    Horizontal = HorizontalAlignment.Center,
                    Vertical = VerticalAlignment.Bottom,
                    
                    Child = x.CreateChild()
                })
                .DrawElement(new Size(400, 300))
                .ExpectChildMeasure(expectedInput: new Size(400, 300), returns: SpacePlan.FullRender(new Size(100, 50)))
                .ExpectCanvasTranslate(new Position(150, 250))
                .ExpectChildDraw(new Size(100, 50))
                .ExpectCanvasTranslate(new Position(-150, -250))
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_HorizontalRight_VerticalTop()
        {
            TestPlan
                .For(x => new Alignment
                {
                    Horizontal = HorizontalAlignment.Right,
                    Vertical = VerticalAlignment.Top,
                    
                    Child = x.CreateChild()
                })
                .DrawElement(new Size(400, 300))
                .ExpectChildMeasure(expectedInput: new Size(400, 300), returns: SpacePlan.FullRender(new Size(100, 50)))
                .ExpectCanvasTranslate(new Position(300, 0))
                .ExpectChildDraw(new Size(100, 50))
                .ExpectCanvasTranslate(new Position(-300, 0))
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_HorizontalCenter_VerticalNone()
        {
            TestPlan
                .For(x => new Alignment
                {
                    Horizontal = HorizontalAlignment.Center,
                    Vertical = null,
                    
                    Child = x.CreateChild()
                })
                .DrawElement(new Size(400, 300))
                .ExpectChildMeasure(expectedInput: new Size(400, 300), returns: SpacePlan.FullRender(new Size(100, 50)))
                .ExpectCanvasTranslate(new Position(150, 0))
                .ExpectChildDraw(new Size(100, 300))
                .ExpectCanvasTranslate(new Position(-150, 0))
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_HorizontalNone_VerticalMiddle()
        {
            TestPlan
                .For(x => new Alignment
                {
                    Horizontal = null,
                    Vertical = VerticalAlignment.Middle,
                    
                    Child = x.CreateChild()
                })
                .DrawElement(new Size(400, 300))
                .ExpectChildMeasure(expectedInput: new Size(400, 300), returns: SpacePlan.FullRender(new Size(100, 50)))
                .ExpectCanvasTranslate(new Position(0, 125))
                .ExpectChildDraw(new Size(400, 50))
                .ExpectCanvasTranslate(new Position(0, -125))
                .CheckDrawResult();
        }
    }
}