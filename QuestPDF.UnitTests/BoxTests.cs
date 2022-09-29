using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class BoxTests
    {
        [Test]
        public void Measure() => SimpleContainerTests.Measure<MinimalBox>();
        
        [Test]
        public void Draw_Wrap()
        {
            TestPlan
                .For(x => new MinimalBox
                {
                    Child = x.CreateChild()
                })
                .DrawElement(new Size(400, 300))
                .ExpectChildMeasure(expectedInput: new Size(400, 300), returns: SpacePlan.Wrap())
                .CheckDrawResult();
        }
        
        [Test]
        public void Measure_PartialRender()
        {
            TestPlan
                .For(x => new MinimalBox
                {
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure(expectedInput: new Size(400, 300), returns: SpacePlan.PartialRender(200, 100))
                .ExpectCanvasTranslate(0, 0)
                .ExpectChildDraw(new Size(200, 100))
                .ExpectCanvasTranslate(0, 0)
                .CheckDrawResult();
        }
        
        [Test]
        public void Measure_FullRender()
        {
            TestPlan
                .For(x => new MinimalBox
                {
                    Child = x.CreateChild()
                })
                .MeasureElement(new Size(500, 400))
                .ExpectChildMeasure(expectedInput: new Size(500, 400), returns: SpacePlan.FullRender(300, 200))
                .ExpectCanvasTranslate(0, 0)
                .ExpectChildDraw(new Size(300, 200))
                .ExpectCanvasTranslate(0, 0)
                .CheckDrawResult();
        }
        
        [Test]
        public void Measure_PartialRender_RightToLeft()
        {
            TestPlan
                .For(x => new MinimalBox
                {
                    Child = x.CreateChild(),
                    ContentDirection = ContentDirection.RightToLeft
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure(expectedInput: new Size(400, 300), returns: SpacePlan.PartialRender(200, 100))
                .ExpectCanvasTranslate(200, 0)
                .ExpectChildDraw(new Size(200, 100))
                .ExpectCanvasTranslate(-200, 0)
                .CheckDrawResult();
        }
        
        [Test]
        public void Measure_FullRender_RightToLeft()
        {
            TestPlan
                .For(x => new MinimalBox
                {
                    Child = x.CreateChild(),
                    ContentDirection = ContentDirection.RightToLeft
                })
                .MeasureElement(new Size(500, 400))
                .ExpectChildMeasure(expectedInput: new Size(500, 400), returns: SpacePlan.FullRender(350, 200))
                .ExpectCanvasTranslate(150, 0)
                .ExpectChildDraw(new Size(350, 200))
                .ExpectCanvasTranslate(-150, 0)
                .CheckDrawResult();
        }
    }
}