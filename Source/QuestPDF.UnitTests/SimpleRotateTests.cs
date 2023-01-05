using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class SimpleRotateTests
    {
        #region measure
        
        [Test]
        public void Measure_Wrap()
        {
            TestPlan
                .For(x => new SimpleRotate
                {
                    Child = x.CreateChild(),
                    TurnCount = 0
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure(new Size(400, 300), SpacePlan.Wrap())
                .CheckMeasureResult(SpacePlan.Wrap());
        }
        
        [Test]
        public void Measure_PartialRender()
        {
            TestPlan
                .For(x => new SimpleRotate
                {
                    Child = x.CreateChild(),
                    TurnCount = 0
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure(new Size(400, 300), SpacePlan.PartialRender(300, 200))
                .CheckMeasureResult(SpacePlan.PartialRender(300, 200));
        }
        
        [Test]
        public void Measure_RotateRight()
        {
            TestPlan
                .For(x => new SimpleRotate
                {
                    Child = x.CreateChild(),
                    TurnCount = 1
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure(new Size(300, 400), SpacePlan.FullRender(200, 300))
                .CheckMeasureResult(SpacePlan.FullRender(300, 200));
        }
        
        [Test]
        public void Measure_RotateFlip()
        {
            TestPlan
                .For(x => new SimpleRotate
                {
                    Child = x.CreateChild(),
                    TurnCount = 2
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure(new Size(400, 300), SpacePlan.FullRender(200, 100))
                .CheckMeasureResult(SpacePlan.FullRender(200, 100));
        }
        
        [Test]
        public void Measure_RotateLeft()
        {
            TestPlan
                .For(x => new SimpleRotate
                {
                    Child = x.CreateChild(),
                    TurnCount = 3 // or -1
                })
                .MeasureElement(new Size(500, 400))
                .ExpectChildMeasure(new Size(400, 500), SpacePlan.FullRender(300, 350))
                .CheckMeasureResult(SpacePlan.FullRender(350, 300));
        }
        
        #endregion
        
        #region draw
        
        [Test]
        public void Draw_Simple()
        {
            TestPlan
                .For(x => new SimpleRotate
                {
                    Child = x.CreateChild(),
                    TurnCount = 0
                })
                .DrawElement(new Size(640, 480))
                .ExpectCanvasTranslate(0, 0)
                .ExpectCanvasRotate(0)
                .ExpectChildDraw(new Size(640, 480))
                .ExpectCanvasRotate(0)
                .ExpectCanvasTranslate(0, 0)
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_RotateRight()
        {
            TestPlan 
                .For(x => new SimpleRotate
                {
                    Child = x.CreateChild(),
                    TurnCount = 1
                })
                .DrawElement(new Size(640, 480))
                .ExpectCanvasTranslate(640, 0)
                .ExpectCanvasRotate(90)
                .ExpectChildDraw(new Size(480, 640))
                .ExpectCanvasRotate(-90)
                .ExpectCanvasTranslate(-640, 0)
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_RotateFlip()
        {
            TestPlan 
                .For(x => new SimpleRotate
                {
                    Child = x.CreateChild(),
                    TurnCount = 2
                })
                .DrawElement(new Size(640, 480))
                .ExpectCanvasTranslate(640, 480)
                .ExpectCanvasRotate(180)
                .ExpectChildDraw(new Size(640, 480))
                .ExpectCanvasRotate(-180)
                .ExpectCanvasTranslate(-640, -480)
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_RotateLeft()
        {
            TestPlan 
                .For(x => new SimpleRotate
                {
                    Child = x.CreateChild(),
                    TurnCount = 3 // or -1
                })
                .DrawElement(new Size(640, 480))
                .ExpectCanvasTranslate(0, 480)
                .ExpectCanvasRotate(270)
                .ExpectChildDraw(new Size(480, 640))
                .ExpectCanvasRotate(-270)
                .ExpectCanvasTranslate(0, -480)
                .CheckDrawResult();
        }
        
        #endregion
    }
}