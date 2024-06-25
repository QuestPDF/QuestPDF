using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class ScaleTests
    {
        #region measure
        
        [Test]
        public void Measure_Wrap()
        {
            TestPlan
                .For(x => new Scale
                {
                    Child = x.CreateChild(),
                    ScaleX = 3,
                    ScaleY = 2
                })
                .MeasureElement(new Size(900, 800))
                .ExpectChildMeasure(new Size(300, 400), SpacePlan.Wrap("Mock"))
                .CheckMeasureResult(SpacePlan.Wrap("Forwarded from child"));
        }
        
        [Test]
        public void Measure_PartialRender()
        {
            TestPlan
                .For(x => new Scale
                {
                    Child = x.CreateChild(),
                    ScaleX = 3,
                    ScaleY = 2
                })
                .MeasureElement(new Size(900, 800))
                .ExpectChildMeasure(new Size(300, 400), SpacePlan.PartialRender(200, 350))
                .CheckMeasureResult(SpacePlan.PartialRender(600, 700));
        }
        
        [Test]
        public void Measure_FullRender()
        {
            TestPlan
                .For(x => new Scale
                {
                    Child = x.CreateChild(),
                    ScaleX = 3,
                    ScaleY = 2
                })
                .MeasureElement(new Size(900, 800))
                .ExpectChildMeasure(new Size(300, 400), SpacePlan.FullRender(250, 300))
                .CheckMeasureResult(SpacePlan.FullRender(750, 600));
        }
        
        [Test]
        public void Measure_NegativeScaleX()
        {
            TestPlan
                .For(x => new Scale
                {
                    Child = x.CreateChild(),
                    ScaleX = -2,
                    ScaleY = 1
                })
                .MeasureElement(new Size(800, 600))
                .ExpectChildMeasure(new Size(400, 600), SpacePlan.FullRender(300, 500))
                .CheckMeasureResult(SpacePlan.FullRender(600, 500));
        }
        
        [Test]
        public void Measure_NegativeScaleY()
        {
            TestPlan
                .For(x => new Scale
                {
                    Child = x.CreateChild(),
                    ScaleX = 1,
                    ScaleY = -3
                })
                .MeasureElement(new Size(800, 600))
                .ExpectChildMeasure(new Size(800, 200), SpacePlan.FullRender(800, 100))
                .CheckMeasureResult(SpacePlan.FullRender(800, 300));
        }
        
        #endregion
        
        #region draw
        
        [Test]
        public void Draw_Simple()
        {
            TestPlan
                .For(x => new Scale
                {
                    Child = x.CreateChild(),
                    ScaleX = 3,
                    ScaleY = 2
                })
                .DrawElement(new Size(900, 800))
                .ExpectCanvasTranslate(0, 0)
                .ExpectCanvasScale(3, 2)
                .ExpectChildDraw(new Size(300, 400))
                .ExpectCanvasScale(1/3f, 1/2f)
                .ExpectCanvasTranslate(0, 0)
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_NegativeScaleX()
        {
            TestPlan
                .For(x => new Scale
                {
                    Child = x.CreateChild(),
                    ScaleX = -3,
                    ScaleY = 2
                })
                .DrawElement(new Size(900, 800))
                .ExpectCanvasTranslate(900, 0)
                .ExpectCanvasScale(-3, 2)
                .ExpectChildDraw(new Size(300, 400))
                .ExpectCanvasScale(-1/3f, 1/2f)
                .ExpectCanvasTranslate(-900, 0)
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_NegativeScaleY()
        {
            TestPlan
                .For(x => new Scale
                {
                    Child = x.CreateChild(),
                    ScaleX = 3,
                    ScaleY = -2
                })
                .DrawElement(new Size(900, 800))
                .ExpectCanvasTranslate(0, 800)
                .ExpectCanvasScale(3, -2)
                .ExpectChildDraw(new Size(300, 400))
                .ExpectCanvasScale(1/3f, -1/2f)
                .ExpectCanvasTranslate(0, -800)
                .CheckDrawResult();
        }
        
        #endregion
    }
}