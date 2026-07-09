using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class EnsureSpaceTests
    {
        [Test]
        public void Measure_ReturnsPartialRenderWithZeroSize_WhenChildReturnsWrap()
        {
            TestPlan
                .For(x => new EnsureSpace
                {
                    Child = x.CreateChild(),
                    MinHeight = 200
                })
                .MeasureElement(new Size(400, 100))
                .ExpectChildMeasure(new Size(400, 100), SpacePlan.Wrap("Mock"))
                .CheckMeasureResult(SpacePlan.PartialRender(Size.Zero));
        }
        
        [Test]
        public void Measure_ReturnsPartialRenderWithZeroSize_WhenChildReturnsPartialRender_AndNotEnoughSpace()
        {
            TestPlan
                .For(x => new EnsureSpace
                {
                    Child = x.CreateChild(),
                    MinHeight = 200
                })
                .MeasureElement(new Size(400, 100))
                .ExpectChildMeasure(new Size(400, 100), SpacePlan.PartialRender(300, 50))
                .CheckMeasureResult(SpacePlan.PartialRender(Size.Zero));
        }
        
        [Test]
        public void Measure_ReturnsPartialRender_WhenChildReturnsPartialRender_AndEnoughSpace()
        {
            TestPlan
                .For(x => new EnsureSpace
                {
                    Child = x.CreateChild(),
                    MinHeight = 200
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure(new Size(400, 300), SpacePlan.PartialRender(300, 250))
                .CheckMeasureResult(SpacePlan.PartialRender(300, 250));
        }
        
        [Test]
        public void Measure_ReturnsFullRender_WhenChildReturnsFullRender_AndNotEnoughSpace()
        {
            TestPlan
                .For(x => new EnsureSpace
                {
                    Child = x.CreateChild(),
                    MinHeight = 200
                })
                .MeasureElement(new Size(400, 100))
                .ExpectChildMeasure(new Size(400, 100), SpacePlan.FullRender(300, 50))
                .CheckMeasureResult(SpacePlan.FullRender(300, 50));
        }
        
        [Test]
        public void Measure_ReturnsFullRender_WhenChildReturnsFullRender_AndEnoughSpace()
        {
            TestPlan
                .For(x => new EnsureSpace
                {
                    Child = x.CreateChild(),
                    MinHeight = 200
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure(new Size(400, 300), SpacePlan.FullRender(300, 250))
                .CheckMeasureResult(SpacePlan.FullRender(300, 250));
        }
    }
}