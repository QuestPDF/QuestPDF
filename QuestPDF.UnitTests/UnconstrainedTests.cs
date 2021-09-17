using NUnit.Framework;
using QuestPDF.Drawing.SpacePlan;
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
                .ExpectChildMeasure(Size.Max, new Wrap())
                .CheckMeasureResult(new Wrap());
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
                .ExpectChildMeasure(Size.Max, new PartialRender(1200, 1600))
                .CheckMeasureResult(new PartialRender(Size.Zero));
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
                .ExpectChildMeasure(Size.Max, new FullRender(1200, 1600))
                .CheckMeasureResult(new FullRender(Size.Zero));
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
                .ExpectChildMeasure(Size.Max, new Wrap())
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
                .ExpectChildMeasure(Size.Max, new PartialRender(1200, 1600))
                .ExpectChildDraw(new Size(1200, 1600))
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
                .ExpectChildMeasure(Size.Max, new FullRender(1600, 1000))
                .ExpectChildDraw(new Size(1600, 1000))
                .CheckDrawResult();
        }
        
        #endregion
    }
}