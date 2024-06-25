using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class UnconstrainedTests
    {
        #region measure
        
        [Test]
        public void Measure_Wrap()
        {
            TestPlan
                .For(x => new Unconstrained
                {
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(900, 800))
                .ExpectChildMeasure(Size.Max, SpacePlan.Wrap("Mock"))
                .CheckMeasureResult(SpacePlan.Wrap("Forwarded from child"));
        }
        
        [Test]
        public void Measure_PartialRender()
        {
            TestPlan
                .For(x => new Unconstrained
                {
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(900, 800))
                .ExpectChildMeasure(Size.Max, SpacePlan.PartialRender(1200, 1600))
                .CheckMeasureResult(SpacePlan.PartialRender(Size.Zero));
        }
        
        [Test]
        public void Measure_FullRender()
        {
            TestPlan
                .For(x => new Unconstrained
                {
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(900, 800))
                .ExpectChildMeasure(Size.Max, SpacePlan.FullRender(1200, 1600))
                .CheckMeasureResult(SpacePlan.FullRender(Size.Zero));
        }
        
        #endregion
        
        #region draw
        
        [Test]
        public void Draw_SkipWhenChildWraps()
        {
            TestPlan
                .For(x => new Unconstrained
                {
                    Child = x.CreateChild()
                })
                .DrawElement(new Size(900, 800))
                .ExpectChildMeasure(Size.Max, SpacePlan.Wrap("Mock"))
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_WhenChildPartiallyRenders()
        {
            TestPlan
                .For(x => new Unconstrained
                {
                    Child = x.CreateChild()
                })
                .DrawElement(new Size(900, 800))
                .ExpectChildMeasure(Size.Max, SpacePlan.PartialRender(1200, 1600))
                .ExpectCanvasTranslate(0, 0)
                .ExpectChildDraw(new Size(1200, 1600))
                .ExpectCanvasTranslate(0, 0)
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_WhenChildFullyRenders()
        {
            TestPlan
                .For(x => new Unconstrained
                {
                    Child = x.CreateChild()
                })
                .DrawElement(new Size(900, 800))
                .ExpectChildMeasure(Size.Max, SpacePlan.FullRender(1600, 1000))
                .ExpectCanvasTranslate(0, 0)
                .ExpectChildDraw(new Size(1600, 1000))
                .ExpectCanvasTranslate(0, 0)
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_WhenChildPartiallyRenders_RightToLeft()
        {
            TestPlan
                .For(x => new Unconstrained
                {
                    Child = x.CreateChild(),
                    ContentDirection = ContentDirection.RightToLeft
                })
                .DrawElement(new Size(900, 800))
                .ExpectChildMeasure(Size.Max, SpacePlan.PartialRender(1200, 1600))
                .ExpectCanvasTranslate(-1200, 0)
                .ExpectChildDraw(new Size(1200, 1600))
                .ExpectCanvasTranslate(1200, 0)
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_WhenChildFullyRenders_RightToLeft()
        {
            TestPlan
                .For(x => new Unconstrained
                {
                    Child = x.CreateChild(),
                    ContentDirection = ContentDirection.RightToLeft
                })
                .DrawElement(new Size(900, 800))
                .ExpectChildMeasure(Size.Max, SpacePlan.FullRender(1600, 1000))
                .ExpectCanvasTranslate(-1600, 0)
                .ExpectChildDraw(new Size(1600, 1000))
                .ExpectCanvasTranslate(1600, 0)
                .CheckDrawResult();
        }
        
        #endregion
    }
}