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
        private static LayoutSpace FlowingHeight(float width, float height) => new(new Size(width, height), LayoutAxisMode.Final, LayoutAxisMode.Flowing);
        private static LayoutSpace FinalHeight(float width, float height) => LayoutSpace.Final(new Size(width, height));
        private static LayoutSpace NaturalSize(float width, float height) => LayoutSpace.Query(new Size(width, height));
        
        [Test]
        public void Measure_MovesToTheNextPage_WhenChildReturnsWrap_AndTheHeightIsFlowing()
        {
            TestPlan
                .For(x => new EnsureSpace
                {
                    Child = x.CreateChild(),
                    MinHeight = 200
                })
                .MeasureElement(FlowingHeight(400, 100))
                .ExpectChildMeasure(FlowingHeight(400, 100), SpacePlan.Wrap("Mock"))
                .CheckMeasureResult(SpacePlan.PartialRender(Size.Zero));
        }
        
        [Test]
        public void Measure_ReturnsWrap_WhenChildReturnsWrap_AndTheHeightIsFinal()
        {
            // no page offers more, so moving the content cannot help and the shortfall is reported
            TestPlan
                .For(x => new EnsureSpace
                {
                    Child = x.CreateChild(),
                    MinHeight = 200
                })
                .MeasureElement(FinalHeight(400, 100))
                .ExpectChildMeasure(FinalHeight(400, 100), SpacePlan.Wrap("Mock"))
                .CheckMeasureResult(SpacePlan.Wrap("Forwarded from child"));
        }
        
        [Test]
        public void Measure_MovesToTheNextPage_WhenChildReturnsPartialRender_AndNotEnoughSpace_AndTheHeightIsFlowing()
        {
            TestPlan
                .For(x => new EnsureSpace
                {
                    Child = x.CreateChild(),
                    MinHeight = 200
                })
                .MeasureElement(FlowingHeight(400, 100))
                .ExpectChildMeasure(FlowingHeight(400, 100), SpacePlan.PartialRender(300, 50))
                .CheckMeasureResult(SpacePlan.PartialRender(Size.Zero));
        }
        
        [Test]
        public void Measure_ReturnsPartialRender_WhenChildReturnsPartialRender_AndNotEnoughSpace_AndTheHeightIsFinal()
        {
            // the element already has the entire height of a page, so a move would only cost an empty page
            TestPlan
                .For(x => new EnsureSpace
                {
                    Child = x.CreateChild(),
                    MinHeight = 200
                })
                .MeasureElement(FinalHeight(400, 100))
                .ExpectChildMeasure(FinalHeight(400, 100), SpacePlan.PartialRender(300, 50))
                .CheckMeasureResult(SpacePlan.PartialRender(300, 50));
        }
        
        [Test]
        public void Measure_ReturnsPartialRender_WhenChildReturnsPartialRender_AndNotEnoughSpace_AndTheNaturalSizeIsAsked()
        {
            TestPlan
                .For(x => new EnsureSpace
                {
                    Child = x.CreateChild(),
                    MinHeight = 200
                })
                .MeasureElement(NaturalSize(400, 100))
                .ExpectChildMeasure(NaturalSize(400, 100), SpacePlan.PartialRender(300, 50))
                .CheckMeasureResult(SpacePlan.PartialRender(300, 50));
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
                .MeasureElement(FlowingHeight(400, 300))
                .ExpectChildMeasure(FlowingHeight(400, 300), SpacePlan.PartialRender(300, 250))
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
                .MeasureElement(FlowingHeight(400, 100))
                .ExpectChildMeasure(FlowingHeight(400, 100), SpacePlan.FullRender(300, 50))
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
                .MeasureElement(FlowingHeight(400, 300))
                .ExpectChildMeasure(FlowingHeight(400, 300), SpacePlan.FullRender(300, 250))
                .CheckMeasureResult(SpacePlan.FullRender(300, 250));
        }
        
        [Test]
        public void Draw_SkipsTheChild_WhenItMovesToTheNextPage()
        {
            TestPlan
                .For(x => new EnsureSpace
                {
                    Child = x.CreateChild(),
                    MinHeight = 200
                })
                .DrawElement(FlowingHeight(400, 100))
                .ExpectChildMeasure(FlowingHeight(400, 100), SpacePlan.PartialRender(300, 50))
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_DrawsTheChild_WhenTheHeightIsFinal()
        {
            TestPlan
                .For(x => new EnsureSpace
                {
                    Child = x.CreateChild(),
                    MinHeight = 200
                })
                .DrawElement(FinalHeight(400, 100))
                .ExpectChildMeasure(FinalHeight(400, 100), SpacePlan.PartialRender(300, 50))
                .ExpectChildDraw(FinalHeight(400, 100))
                .CheckDrawResult();
        }
    }
}
