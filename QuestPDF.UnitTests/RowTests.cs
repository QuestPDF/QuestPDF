using NUnit.Framework;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class RowTests
    {
        #region Measure
        
        [Test]
        public void Measure_ReturnsWrap_WhenLeftChildReturnsWrap()
        {
            TestPlan
                .For(x => new SimpleRow
                {
                    Left = x.CreateChild("left"),
                    Right = x.CreateChild("right")
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure("left", new Size(400, 300), new Wrap())
                .CheckMeasureResult(new Wrap());
        }
        
        [Test]
        public void Measure_ReturnsWrap_WhenRightChildReturnsWrap()
        {
            TestPlan
                .For(x => new SimpleRow
                {
                    Left = x.CreateChild("left"),
                    Right = x.CreateChild("right")
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure("left", new Size(400, 300), new FullRender(250, 150))
                .ExpectChildMeasure("right", new Size(150, 300), new Wrap())
                .CheckMeasureResult(new Wrap());
        }
        
        [Test]
        public void Measure_ReturnsPartialRender_WhenLeftChildReturnsPartialRender()
        {
            TestPlan
                .For(x => new SimpleRow
                {
                    Left = x.CreateChild("left"),
                    Right = x.CreateChild("right")
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure("left", new Size(400, 300), new PartialRender(250, 150))
                .ExpectChildMeasure("right", new Size(150, 300), new FullRender(100, 100))
                .CheckMeasureResult(new PartialRender(350, 150));
        }
        
        [Test]
        public void Measure_ReturnsPartialRender_WhenRightChildReturnsPartialRender()
        {
            TestPlan
                .For(x => new SimpleRow
                {
                    Left = x.CreateChild("left"),
                    Right = x.CreateChild("right")
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure("left", new Size(400, 300), new FullRender(250, 150))
                .ExpectChildMeasure("right", new Size(150, 300), new PartialRender(100, 100))
                .CheckMeasureResult(new PartialRender(350, 150));
        }
        
        [Test]
        public void Measure_ReturnsFullRender_WhenBothChildrenReturnFullRender()
        {
            TestPlan
                .For(x => new SimpleRow
                {
                    Left = x.CreateChild("left"),
                    Right = x.CreateChild("right")
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure("left", new Size(400, 300), new FullRender(200, 150))
                .ExpectChildMeasure("right", new Size(200, 300), new FullRender(100, 100))
                .CheckMeasureResult(new FullRender(300, 150));
        }
        
        #endregion

        #region Draw

        [Test]
        public void Draw()
        {
            TestPlan
                .For(x => new SimpleRow
                {
                    Left = x.CreateChild("left"),
                    Right = x.CreateChild("right")
                })
                .DrawElement(new Size(400, 300))
                .ExpectChildMeasure("left", new Size(400, 300), new FullRender(250, 150))
                .ExpectChildDraw("left", new Size(250, 300))
                .ExpectCanvasTranslate(250, 0)
                .ExpectChildDraw("right", new Size(150, 300))
                .ExpectCanvasTranslate(-250, 0)
                .CheckDrawResult();
        }

        #endregion
        
        // TODO: add tests for the spacing property
        // TODO: add tests for the tree builder method
        // TODO: add tests for relative column
    }
}