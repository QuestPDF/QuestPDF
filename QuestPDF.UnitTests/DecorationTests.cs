using NUnit.Framework;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class DecorationTests
    {
        #region Measure

        [Test]
        public void Measure_ReturnsWrap_WhenDecorationReturnsWrap()
        {
            TestPlan
                .For(x => new Decoration
                {
                    Type = DecorationType.Append,
                    DecorationElement = x.CreateChild("decoration"),
                    ContentElement = x.CreateChild("content")
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure("decoration", new Size(400, 300), new Wrap())
                .CheckMeasureResult(new Wrap());
        }
        
        [Test]
        public void Measure_ReturnsWrap_WhenDecorationReturnsPartialRender()
        {
            TestPlan
                .For(x => new Decoration
                {
                    Type = DecorationType.Append,
                    DecorationElement = x.CreateChild("decoration"),
                    ContentElement = x.CreateChild("content")
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure("decoration", new Size(400, 300), new PartialRender(300, 200))
                .CheckMeasureResult(new Wrap());
        }
        
        [Test]
        public void Measure_ReturnsWrap_WhenContentReturnsWrap()
        {
            TestPlan
                .For(x => new Decoration
                {
                    Type = DecorationType.Append,
                    DecorationElement = x.CreateChild("decoration"),
                    ContentElement = x.CreateChild("content")
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure("decoration", new Size(400, 300), new FullRender(300, 100))
                .ExpectChildMeasure("content", new Size(400, 200), new Wrap())
                .CheckMeasureResult(new Wrap());
        }
        
        [Test]
        public void Measure_ReturnsPartialRender_WhenContentReturnsPartialRender()
        {
            TestPlan
                .For(x => new Decoration
                {
                    Type = DecorationType.Append,
                    DecorationElement = x.CreateChild("decoration"),
                    ContentElement = x.CreateChild("content")
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure("decoration", new Size(400, 300), new FullRender(300, 100))
                .ExpectChildMeasure("content", new Size(400, 200), new PartialRender(200, 150))
                .CheckMeasureResult(new PartialRender(400, 250));
        }
        
        [Test]
        public void Measure_ReturnsFullRender_WhenContentReturnsFullRender()
        {
            TestPlan
                .For(x => new Decoration
                {
                    Type = DecorationType.Append,
                    DecorationElement = x.CreateChild("decoration"),
                    ContentElement = x.CreateChild("content")
                })
                .MeasureElement(new Size(400, 300))
                .ExpectChildMeasure("decoration", new Size(400, 300), new FullRender(300, 100))
                .ExpectChildMeasure("content", new Size(400, 200), new FullRender(200, 150))
                .CheckMeasureResult(new FullRender(400, 250));
        }
        
        #endregion
        
        #region Draw
        
        [Test]
        public void Draw_Prepend()
        {
            TestPlan
                .For(x => new Decoration
                {
                    Type = DecorationType.Prepend,
                    DecorationElement = x.CreateChild("decoration"),
                    ContentElement = x.CreateChild("content")
                })
                .DrawElement(new Size(400, 300))
                .ExpectChildMeasure("decoration", new Size(400, 300), new FullRender(300, 100))
                .ExpectChildDraw("decoration", new Size(400, 100))
                .ExpectCanvasTranslate(0, 100)
                .ExpectChildDraw("content", new Size(400, 200))
                .ExpectCanvasTranslate(0, -100)
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_Append()
        {
            TestPlan
                .For(x => new Decoration
                {
                    Type = DecorationType.Append,
                    DecorationElement = x.CreateChild("decoration"),
                    ContentElement = x.CreateChild("content")
                })
                .DrawElement(new Size(400, 300))
                .ExpectChildMeasure("decoration", new Size(400, 300), new FullRender(300, 100))
                .ExpectChildDraw("content", new Size(400, 200))
                .ExpectCanvasTranslate(0, 200)
                .ExpectChildDraw("decoration", new Size(400, 100))
                .ExpectCanvasTranslate(0, -200)
                .CheckDrawResult();
        }

        #endregion
    }
}