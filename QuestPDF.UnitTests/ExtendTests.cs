using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class ExtendTests
    {
        [Test]
        public void Measure_ShouldHandleNullChild() => new Extend().MeasureWithoutChild();
        
        [Test]
        public void Draw_ShouldHandleNullChild() => new Extend().DrawWithoutChild();
        
        [Test]
        public void Measure_ReturnsWrap_WhenChildReturnsWrap()
        {
            TestPlan
                .For(x => new Extend
                {
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(400, 200))
                .ExpectChildMeasure(new Size(400, 200), SpacePlan.Wrap())
                .CheckMeasureResult(SpacePlan.Wrap());
        }
        
        [Test]
        public void Measure_ReturnsPartialRender_WhenChildReturnsPartialRender()
        {
            TestPlan
                .For(x => new Extend
                {
                    Child = x.CreateChild(),
                    ExtendHorizontal = true,
                    ExtendVertical = true
                })
                .MeasureElement(new Size(400, 200))
                .ExpectChildMeasure(new Size(400, 200), SpacePlan.PartialRender(300, 100))
                .CheckMeasureResult(SpacePlan.PartialRender(400, 200));
        }
        
        [Test]
        public void Measure_ReturnsFullRender_WhenChildReturnsFullRender()
        {
            TestPlan
                .For(x => new Extend
                {
                    Child = x.CreateChild(),
                    ExtendHorizontal = true,
                    ExtendVertical = true
                })
                .MeasureElement(new Size(400, 200))
                .ExpectChildMeasure(new Size(400, 200), SpacePlan.FullRender(300, 100))
                .CheckMeasureResult(SpacePlan.FullRender(400, 200));
        }
        
        [Test]
        public void Measure_ExtendHorizontal()
        {
            TestPlan
                .For(x => new Extend
                {
                    Child = x.CreateChild(),
                    ExtendHorizontal = true,
                    ExtendVertical = false
                })
                .MeasureElement(new Size(400, 200))
                .ExpectChildMeasure(new Size(400, 200), SpacePlan.FullRender(100, 100))
                .CheckMeasureResult(SpacePlan.FullRender(400, 100));
        }
        
        [Test]
        public void Measure_ExtendVertical()
        {
            TestPlan
                .For(x => new Extend
                {
                    Child = x.CreateChild(),
                    ExtendHorizontal = false,
                    ExtendVertical = true
                })
                .MeasureElement(new Size(400, 200))
                .ExpectChildMeasure(new Size(400, 200), SpacePlan.FullRender(100, 100))
                .CheckMeasureResult(SpacePlan.FullRender(100, 200));
        }
    }
}