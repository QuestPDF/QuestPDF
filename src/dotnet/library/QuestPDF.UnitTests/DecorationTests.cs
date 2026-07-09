using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class DecorationTests
    {
        private Decoration CreateDecoration(TestPlan testPlan)
        {
            return new Decoration
            {
                Before = testPlan.CreateChild("before"),
                Content = testPlan.CreateChild("content"),
                After = testPlan.CreateChild("after"),
            };
        }
        
        #region Measure

        [Test]
        public void Measure_ReturnsWrap_WhenBeforeReturnsWrap()
        {
            TestPlan
                .For(CreateDecoration)
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure("before", new Size(400, 300), SpacePlan.Wrap("Mock"))
                .ExpectChildMeasure("after", new Size(400, 300), SpacePlan.FullRender(100, 50))
                .ExpectChildMeasure("content", new Size(400, 250), SpacePlan.FullRender(100, 100))
                .CheckMeasureResult(SpacePlan.Wrap("Decoration slot (before or after) does not fit fully on the page."));
        }
        
        [Test]
        public void Measure_ReturnsWrap_WhenContentReturnsWrap()
        {
            TestPlan
                .For(CreateDecoration)
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure("before", new Size(400, 300), SpacePlan.FullRender(100, 50))
                .ExpectChildMeasure("after", new Size(400, 300), SpacePlan.FullRender(100, 50))
                .ExpectChildMeasure("content", new Size(400, 200), SpacePlan.Wrap("Mock"))
                .CheckMeasureResult(SpacePlan.Wrap("The primary content does not fit on the page."));
        }
        
        [Test]
        public void Measure_ReturnsWrap_WhenAfterReturnsWrap()
        {
            TestPlan
                .For(CreateDecoration)
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure("before", new Size(400, 300), SpacePlan.FullRender(100, 50))
                .ExpectChildMeasure("after", new Size(400, 300), SpacePlan.Wrap("Mock"))
                .ExpectChildMeasure("content", new Size(400, 250), SpacePlan.FullRender(100, 100))
                .CheckMeasureResult(SpacePlan.Wrap("Decoration slot (before or after) does not fit fully on the page."));
        }
        
        [Test]
        public void Measure_ReturnsWrap_WhenBeforeReturnsPartialRender()
        {
            TestPlan
                .For(CreateDecoration)
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure("before", new Size(400, 300), SpacePlan.PartialRender(100, 50))
                .ExpectChildMeasure("after", new Size(400, 300), SpacePlan.FullRender(100, 50))
                .ExpectChildMeasure("content", new Size(400, 250), SpacePlan.FullRender(100, 100))
                .CheckMeasureResult(SpacePlan.Wrap("Decoration slot (before or after) does not fit fully on the page."));
        }
        
        [Test]
        public void Measure_ReturnsWrap_WhenAfterReturnsPartialRender()
        {
            TestPlan
                .For(CreateDecoration)
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure("before", new Size(400, 300), SpacePlan.FullRender(100, 50))
                .ExpectChildMeasure("after", new Size(400, 300), SpacePlan.PartialRender(100, 50))
                .ExpectChildMeasure("content", new Size(400, 250), SpacePlan.FullRender(100, 100))
                .CheckMeasureResult(SpacePlan.Wrap("Decoration slot (before or after) does not fit fully on the page."));
        }
        
        [Test]
        public void Measure_ReturnsWrap_WhenContentReturnsPartialRender()
        {
            TestPlan
                .For(CreateDecoration)
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure("before", new Size(400, 300), SpacePlan.FullRender(100, 50))
                .ExpectChildMeasure("after", new Size(400, 300), SpacePlan.FullRender(100, 50))
                .ExpectChildMeasure("content", new Size(400, 200), SpacePlan.PartialRender(150, 100))
                .CheckMeasureResult(SpacePlan.PartialRender(150, 200));
        }
        
        [Test]
        public void Measure_ReturnsWrap_WhenContentReturnsFullRender()
        {
            TestPlan
                .For(CreateDecoration)
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure("before", new Size(400, 300), SpacePlan.FullRender(100, 50))
                .ExpectChildMeasure("after", new Size(400, 300), SpacePlan.FullRender(100, 50))
                .ExpectChildMeasure("content", new Size(400, 200), SpacePlan.FullRender(150, 100))
                .CheckMeasureResult(SpacePlan.FullRender(150, 200));
        }

        #endregion
        
        #region Draw
        
        [Test]
        public void Draw_Append()
        {
            TestPlan
                .For(CreateDecoration)
                .DrawElement(new Size(400, 300))
                .ExpectChildMeasure("before", new Size(400, 300), SpacePlan.FullRender(200, 40))
                .ExpectChildMeasure("after", new Size(400, 300), SpacePlan.FullRender(200, 60))
                .ExpectChildMeasure("content", new Size(400, 200), SpacePlan.FullRender(300, 100))
                
                .ExpectCanvasTranslate(0, 0)
                .ExpectChildDraw("before", new Size(300, 40))
                .ExpectCanvasTranslate(0, 0)
                
                .ExpectCanvasTranslate(0, 40)
                .ExpectChildDraw("content", new Size(300, 100))
                .ExpectCanvasTranslate(0, -40)
                
                .ExpectCanvasTranslate(0, 140)
                .ExpectChildDraw("after", new Size(300, 60))
                .ExpectCanvasTranslate(0, -140)

                .CheckDrawResult();
        }

        #endregion
    }
}