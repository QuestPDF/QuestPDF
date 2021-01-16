using NUnit.Framework;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.MeasureTest;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class ConstrainedTests
    {
        [Test]
        public void Measure_ShouldHandleNullChild() => new Constrained().MeasureWithoutChild();
        
        [Test]
        public void Draw_ShouldHandleNullChild() => new Constrained().DrawWithoutChild();

        [Test]
        public void Measure_MinHeight_ExpectWrap()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MinHeight = 100
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
                    Child = x.CreateChild("a")
                })
                .MeasureElement(new Size(400, 200))
                .ExpectChildMeasure("a", new Size(400, 200), new FullRender(400, 50))
                .CheckMeasureResult(new FullRender(400, 100));
        }
        
        [Test]
        public void Measure_MinHeight_PassHeight()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MinHeight = 100,
                    Child = x.CreateChild("a")
                })
                .MeasureElement(new Size(400, 200))
                .ExpectChildMeasure("a", new Size(400, 200), new FullRender(400, 150))
                .CheckMeasureResult(new FullRender(400, 150));
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
                .CheckMeasureResult(new FullRender(0, 0));
        }
        
        [Test]
        public void Measure_MaxHeight_PartialRender()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MaxHeight = 100,
                    Child = x.CreateChild("a")
                })
                .MeasureElement(new Size(400, 200))
                .ExpectChildMeasure("a", new Size(400, 100), new PartialRender(400, 75))
                .CheckMeasureResult(new PartialRender(400, 75));
        }
        
        [Test]
        public void Measure_MaxHeight_ExpectWrap()
        {
            TestPlan
                .For(x => new Constrained
                {
                    MaxHeight = 100,
                    Child = x.CreateChild("a")
                })
                .MeasureElement(new Size(400, 200))
                .ExpectChildMeasure("a", new Size(400, 100), new Wrap())
                .CheckMeasureResult(new Wrap());
        }
    }
}