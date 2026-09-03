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
        private static readonly LayoutSpace FlowingHeight = new(new Size(400, 300), LayoutAxisMode.Final, LayoutAxisMode.Flowing);
        private static readonly LayoutSpace FinalHeight = LayoutSpace.Final(new Size(400, 300));
        private static readonly LayoutSpace NaturalSize = LayoutSpace.Query(new Size(400, 300));
        
        [Test]
        public void Measure_ReturnsWrap_WhenElementReturnsWrap()
        {
            TestPlan
                .For(x => new ShowEntire
                {
                    Child = x.CreateChild()
                })
                .MeasureElement(FinalHeight)
                .ExpectChildMeasure(FinalHeight, SpacePlan.Wrap("Mock"))
                .CheckMeasureResult(SpacePlan.Wrap("Child element does not fit (even partially) on the page."));
        }
        
        [Test]
        public void Measure_ReturnsWrap_WhenElementReturnsPartialRender_AndTheHeightIsFlowing()
        {
            // the next page offers the entire height, so moving the content there may let it fit entirely
            TestPlan
                .For(x => new ShowEntire
                {
                    Child = x.CreateChild()
                })
                .MeasureElement(FlowingHeight)
                .ExpectChildMeasure(FlowingHeight, SpacePlan.PartialRender(300, 200))
                .CheckMeasureResult(SpacePlan.Wrap("Child element fits only partially on the page."));
        }
        
        [Test]
        public void Measure_ReturnsPartialRender_WhenElementReturnsPartialRender_AndTheHeightIsFinal()
        {
            // no page is taller than this one, so the content cannot be shown entirely anywhere and is split instead
            TestPlan
                .For(x => new ShowEntire
                {
                    Child = x.CreateChild()
                })
                .MeasureElement(FinalHeight)
                .ExpectChildMeasure(FinalHeight, SpacePlan.PartialRender(300, 200))
                .CheckMeasureResult(SpacePlan.PartialRender(300, 200))
                .CheckState<ShowEntire>(x => x.GetCompanionHint() != null);
        }
        
        [Test]
        public void Measure_ReturnsPartialRender_WhenElementReturnsPartialRender_AndTheNaturalSizeIsAsked()
        {
            TestPlan
                .For(x => new ShowEntire
                {
                    Child = x.CreateChild()
                })
                .MeasureElement(NaturalSize)
                .ExpectChildMeasure(NaturalSize, SpacePlan.PartialRender(300, 200))
                .CheckMeasureResult(SpacePlan.PartialRender(300, 200))
                .CheckState<ShowEntire>(x => x.GetCompanionHint() == null);
        }
        
        [Test]
        public void Measure_ReturnsFullRender_WhenElementReturnsFullRender()
        {
            TestPlan
                .For(x => new ShowEntire
                {
                    Child = x.CreateChild()
                })
                .MeasureElement(FlowingHeight)
                .ExpectChildMeasure(FlowingHeight, SpacePlan.FullRender(300, 200))
                .CheckMeasureResult(SpacePlan.FullRender(300, 200));
        }
        
        [Test]
        public void Draw() => SimpleContainerTests.Draw<ShowEntire>();
    }
}
