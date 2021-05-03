using NUnit.Framework;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class EnsureSpaceTests
    {
        [Test]
        public void Measure_ReturnsWrap_WhenChildReturnsWrap()
        {
            TestPlan
                .For(x => new EnsureSpace
                {
                    Child = x.CreateChild(),
                    MinHeight = 200
                })
                .MeasureElement(new Size(400, 100))
                .ExpectChildMeasure(new Size(400, 100), new Wrap())
                .CheckMeasureResult(new Wrap());
        }
        
        [Test]
        public void Measure_ReturnsWrap_WhenChildReturnsPartialRender_AndNotEnoughSpace()
        {
            TestPlan
                .For(x => new EnsureSpace
                {
                    Child = x.CreateChild(),
                    MinHeight = 200
                })
                .MeasureElement(new Size(400, 100))
                .ExpectChildMeasure(new Size(400, 100), new PartialRender(300, 50))
                .CheckMeasureResult(new Wrap());
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
                .ExpectChildMeasure(new Size(400, 300), new PartialRender(300, 250))
                .CheckMeasureResult(new PartialRender(300, 250));
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
                .ExpectChildMeasure(new Size(400, 100), new FullRender(300, 50))
                .CheckMeasureResult(new FullRender(300, 50));
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
                .ExpectChildMeasure(new Size(400, 300), new FullRender(300, 250))
                .CheckMeasureResult(new FullRender(300, 250));
        }
    }
}