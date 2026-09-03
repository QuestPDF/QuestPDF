using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class PreventPageBreakTests
    {
        private static readonly LayoutSpace FlowingHeight = new(new Size(400, 300), LayoutAxisMode.Final, LayoutAxisMode.Flowing);
        private static readonly LayoutSpace FinalHeight = LayoutSpace.Final(new Size(400, 300));
        
        [Test]
        public void Measure_MovesToTheNextPage_WhenChildReturnsPartialRender_AndTheHeightIsFlowing()
        {
            TestPlan
                .For(x => new PreventPageBreak { Child = x.CreateChild() })
                .MeasureElement(FlowingHeight)
                .ExpectChildMeasure(FlowingHeight, SpacePlan.PartialRender(300, 200))
                .CheckMeasureResult(SpacePlan.PartialRender(Size.Zero));
        }
        
        [Test]
        public void Measure_MovesToTheNextPage_WhenChildReturnsWrap_AndTheHeightIsFlowing()
        {
            TestPlan
                .For(x => new PreventPageBreak { Child = x.CreateChild() })
                .MeasureElement(FlowingHeight)
                .ExpectChildMeasure(FlowingHeight, SpacePlan.Wrap("Mock"))
                .CheckMeasureResult(SpacePlan.PartialRender(Size.Zero));
        }
        
        [Test]
        public void Measure_ReturnsPartialRender_WhenChildReturnsPartialRender_AndTheHeightIsFinal()
        {
            // no page is taller than this one, so the content is split here instead of costing an empty page
            TestPlan
                .For(x => new PreventPageBreak { Child = x.CreateChild() })
                .MeasureElement(FinalHeight)
                .ExpectChildMeasure(FinalHeight, SpacePlan.PartialRender(300, 200))
                .CheckMeasureResult(SpacePlan.PartialRender(300, 200))
                .CheckState<PreventPageBreak>(x => x.GetCompanionHint() != null);
        }
        
        [Test]
        public void Measure_ReturnsFullRender_WhenChildReturnsFullRender()
        {
            TestPlan
                .For(x => new PreventPageBreak { Child = x.CreateChild() })
                .MeasureElement(FlowingHeight)
                .ExpectChildMeasure(FlowingHeight, SpacePlan.FullRender(300, 200))
                .CheckMeasureResult(SpacePlan.FullRender(300, 200));
        }
        
        [Test]
        public void Draw_SkipsTheChild_WhenItMovesToTheNextPage()
        {
            TestPlan
                .For(x => new PreventPageBreak { Child = x.CreateChild() })
                .DrawElement(FlowingHeight)
                .ExpectChildMeasure(FlowingHeight, SpacePlan.PartialRender(300, 200))
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_DrawsTheChild_WhenTheHeightIsFinal()
        {
            TestPlan
                .For(x => new PreventPageBreak { Child = x.CreateChild() })
                .DrawElement(FinalHeight)
                .ExpectChildMeasure(FinalHeight, SpacePlan.PartialRender(300, 200))
                .ExpectChildDraw(FinalHeight)
                .CheckDrawResult();
        }
    }
}
