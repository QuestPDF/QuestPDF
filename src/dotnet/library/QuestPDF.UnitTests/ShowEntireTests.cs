using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class ShowEntireTests
    {
        [Test]
        public void Measure_ReturnsWrap_WhenElementReturnsWrap()
        {
            TestPlan
                .For(x => new ShowEntire
                {
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure(new Size(400, 300), SpacePlan.Wrap("Mock"))
                .CheckMeasureResult(SpacePlan.Wrap("Child element does not fit (even partially) on the page."));
        }
        
        [Test]
        public void Measure_ReturnsWrap_WhenElementReturnsPartialRender()
        {
            TestPlan
                .For(x => new ShowEntire
                {
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure(new Size(400, 300), SpacePlan.PartialRender(300, 200))
                .CheckMeasureResult(SpacePlan.Wrap("Child element fits only partially on the page."));
        }
        
        [Test]
        public void Measure_ReturnsFullRender_WhenElementReturnsFullRender()
        {
            TestPlan
                .For(x => new ShowEntire
                {
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure(new Size(400, 300), SpacePlan.FullRender(300, 200))
                .CheckMeasureResult(SpacePlan.FullRender(300, 200));
        }
        
        [Test]
        public void Draw() => SimpleContainerTests.Draw<ShowEntire>();
    }
}