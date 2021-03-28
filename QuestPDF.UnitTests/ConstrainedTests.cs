using NUnit.Framework;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class ConstrainedTests
    {
        [Test]
        public void Measure_ShouldHandleNullChild() => new Constrained().MeasureWithoutChild();
        
        [Test]
        public void Draw_ShouldHandleNullChild() => new Constrained().DrawWithoutChild();

        #region Min Height

        [Test]
        public void Measure_MinHeight_Empty()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MinHeight = 100
                })
                .MeasureElement(new Size(400, 150))
                .CheckMeasureResult(new FullRender(0, 100));
        }
        
        [Test]
        public void Measure_MinHeight_NotEnoughHeight()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MinHeight = 100,
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(400, 50))
                .CheckMeasureResult(new Wrap());
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
                .ExpectChildMeasure(new Size(400, 200), new FullRender(400, 50))
                .CheckMeasureResult(new FullRender(400, 100));
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
                .ExpectChildMeasure(new Size(400, 200), new FullRender(400, 150))
                .CheckMeasureResult(new FullRender(400, 150));
        }
        
        #endregion
        
        #region Max Height
        
        [Test]
        public void Measure_MaxHeight_Empty()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MaxHeight = 100
                })
                .MeasureElement(new Size(400, 150))
                .CheckMeasureResult(new FullRender(0, 0));
        }
        
        [Test]
        public void Measure_MaxHeight_PassHeight()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MaxHeight = 100,
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(400, 200))
                .ExpectChildMeasure(new Size(400, 100), new FullRender(400, 75))
                .CheckMeasureResult(new FullRender(400, 75));
        }
        
        #endregion
        
        #region Min Width
        
        [Test]
        public void Measure_MinWidth_Empty()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MinWidth = 100
                })
                .MeasureElement(new Size(150, 400))
                .CheckMeasureResult(new FullRender(100, 0));
        }
        
        [Test]
        public void Measure_MinWidth_NotEnoughWidth()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MinWidth = 100,
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(50, 400))
                .CheckMeasureResult(new Wrap());
        }

        [Test]
        public void Measure_MinWidth_ExtendWidth()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MinWidth = 100,
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(400, 200))
                .ExpectChildMeasure(new Size(400, 200), new FullRender(50, 200))
                .CheckMeasureResult(new FullRender(100, 200));
        }
        
        [Test]
        public void Measure_MinWidth_PassWidth()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MinWidth = 100,
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(400, 200))
                .ExpectChildMeasure(new Size(400, 200), new FullRender(150, 200))
                .CheckMeasureResult(new FullRender(150, 200));
        }
        
        #endregion

        #region Max Width

        [Test]
        public void Measure_MaxWidth_Empty()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MaxWidth = 100
                })
                .MeasureElement(new Size(400, 150))
                .CheckMeasureResult(new FullRender(0, 0));
        }
        
        [Test]
        public void Measure_MaxWidth_PassWidth()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MaxWidth = 100,
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(400, 200))
                .ExpectChildMeasure(new Size(100, 200), new FullRender(75, 200))
                .CheckMeasureResult(new FullRender(75, 200));
        }
        
        #endregion
        
        #region Space Plans
        
        [Test]
        public void Measure_AcceptsFullRender()
        {
            TestPlan
                .For(x => new Constrained
                {
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(400, 200))
                .ExpectChildMeasure(new Size(400, 200), new FullRender(300, 200))
                .CheckMeasureResult(new FullRender(300, 200));
        }
        
        [Test]
        public void Measure_AcceptsPartialRender()
        {
            TestPlan
                .For(x => new Constrained
                {
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(400, 200))
                .ExpectChildMeasure(new Size(400, 200), new PartialRender(300, 200))
                .CheckMeasureResult(new PartialRender(300, 200));
        }
        
        [Test]
        public void Measure_AcceptsWrap()
        {
            TestPlan
                .For(x => new Constrained
                {
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(400, 200))
                .ExpectChildMeasure(new Size(400, 200), new Wrap())
                .CheckMeasureResult(new Wrap());
        }
        
        #endregion
        
        #region Drawing
        
        [Test]
        public void Draw_Normal()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MaxWidth = 500,
                    MaxHeight = 300,
                    Child = x.CreateChild()
                })
                .DrawElement(new Size(400, 200))
                .ExpectChildDraw(new Size(400, 200))
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_Shrink()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MaxWidth = 300,
                    MaxHeight = 150,
                    Child = x.CreateChild()
                })
                .DrawElement(new Size(400, 200))
                .ExpectChildDraw(new Size(300, 150))
                .CheckDrawResult();
        }

        #endregion
    }
}