using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class ConstrainedTests
    {
        #region Height
        
        [Test]
        public void Measure_MinHeight_ExpectWrap()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MinHeight = 100
                })
                .MeasureElement(new Size(400, 50))
                .CheckMeasureResult(SpacePlan.Wrap());
        }
        
        [Test]
        public void Measure_MinHeight_ExtendHeight()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MinHeight = 100,
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(400, 200))
                .ExpectChildMeasure(new Size(400, 200), SpacePlan.FullRender(400, 50))
                .CheckMeasureResult(SpacePlan.FullRender(400, 100));
        }
        
        [Test]
        public void Measure_MinHeight_PassHeight()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MinHeight = 100,
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(400, 200))
                .ExpectChildMeasure(new Size(400, 200), SpacePlan.FullRender(400, 150))
                .CheckMeasureResult(SpacePlan.FullRender(400, 150));
        }
        
        [Test]
        public void Measure_MaxHeight_Empty()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MaxHeight = 100
                })
                .MeasureElement(new Size(400, 150))
                .CheckMeasureResult(SpacePlan.FullRender(0, 0));
        }
        
        [Test]
        public void Measure_MaxHeight_PartialRender()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MaxHeight = 100,
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(400, 200))
                .ExpectChildMeasure(new Size(400, 100), SpacePlan.PartialRender(400, 75))
                .CheckMeasureResult(SpacePlan.PartialRender(400, 75));
        }
        
        [Test]
        public void Measure_MaxHeight_ExpectWrap()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MaxHeight = 100,
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(400, 200))
                .ExpectChildMeasure(new Size(400, 100), SpacePlan.Wrap())
                .CheckMeasureResult(SpacePlan.Wrap());
        }
        
        #endregion
        
        #region Width
        
        [Test]
        public void Measure_MinWidth_ExpectWrap()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MinWidth = 100
                })
                .MeasureElement(new Size(50, 400))
                .CheckMeasureResult(SpacePlan.Wrap());
        }
        
        [Test]
        public void Measure_MinWidth_ExtendHeight()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MinWidth = 100,
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(200, 400))
                .ExpectChildMeasure(new Size(200, 400), SpacePlan.FullRender(50, 400))
                .CheckMeasureResult(SpacePlan.FullRender(100, 400));
        }
        
        [Test]
        public void Measure_MinWidth_PassHeight()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MinWidth = 100,
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(200, 400))
                .ExpectChildMeasure(new Size(200, 400), SpacePlan.FullRender(150, 400))
                .CheckMeasureResult(SpacePlan.FullRender(150, 400));
        }
        
        [Test]
        public void Measure_MaxWidth_Empty()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MaxWidth = 100
                })
                .MeasureElement(new Size(150, 400))
                .CheckMeasureResult(SpacePlan.FullRender(0, 0));
        }
        
        [Test]
        public void Measure_MaxWidth_PartialRender()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MaxWidth = 100,
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(200, 400))
                .ExpectChildMeasure(new Size(100, 400), SpacePlan.PartialRender(75, 400))
                .CheckMeasureResult(SpacePlan.PartialRender(75, 400));
        }
        
        [Test]
        public void Measure_MaxWidth_ExpectWrap()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MaxWidth = 100,
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(200, 400))
                .ExpectChildMeasure(new Size(100, 400), SpacePlan.Wrap())
                .CheckMeasureResult(SpacePlan.Wrap());
        }
        
        #endregion
    }
}