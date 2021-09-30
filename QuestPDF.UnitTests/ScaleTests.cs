using NUnit.Framework;
using QuestPDF.Drawing.SpacePlan;
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
                .ExpectChildMeasure(new Size(300, 400), new Wrap())
                .CheckMeasureResult(new Wrap());
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
                .ExpectChildMeasure(new Size(300, 400), new PartialRender(200, 350))
                .CheckMeasureResult(new PartialRender(600, 700));
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
                .ExpectChildMeasure(new Size(300, 400), new FullRender(250, 300))
                .CheckMeasureResult(new FullRender(750, 600));
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
                .ExpectChildMeasure(new Size(400, 600), new FullRender(300, 500))
                .CheckMeasureResult(new FullRender(600, 500));
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
                .ExpectChildMeasure(new Size(800, 200), new FullRender(800, 100))
                .CheckMeasureResult(new FullRender(800, 300));
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