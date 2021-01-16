using NUnit.Framework;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.MeasureTest;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class StackTests
    {
        [Test]
        public void Measure_NoChildren_Empty()
        {
            TestPlan
                .For(x => new Stack
                {
                    Spacing = 100
                })
                .MeasureElement(new Size(500, 1000))
                .CheckMeasureResult(new FullRender(Size.Zero));
        }
        
        [Test]
        public void Measure_ReturnsWrap_WhenPageable_AndAnyChildReturnsWrap()
        {
            TestPlan
                .For(x => new Stack
                {
                    Spacing = 100,
                    Pageable = true,
                    Children = new []
                    {
                        x.CreateChild("a"),
                        x.CreateChild("b"),
                        x.CreateChild("c"),
                        x.CreateChild("d")
                    }
                })
                .MeasureElement(new Size(500, 1000))
                .ExpectChildMeasure("a", expectedInput: new Size(500, 1000), returns: new FullRender(500, 200))
                .ExpectChildMeasure("b", expectedInput: new Size(500, 700), returns: new FullRender(500, 300))
                .ExpectChildMeasure("c", expectedInput: new Size(500, 300), returns: new Wrap())
                .CheckMeasureResult(new PartialRender(500, 600));
        }
        
        [Test]
        public void Measure_ReturnsWrap_WhenPageable_AndFirstChildReturnsWrap()
        {
            TestPlan
                .For(x => new Stack
                {
                    Spacing = 100,
                    Pageable = true,
                    Children = new []
                    {
                        x.CreateChild("a"),
                        x.CreateChild("b"),
                        x.CreateChild("c")
                    }
                })
                .MeasureElement(new Size(500, 1000))
                .ExpectChildMeasure("a", expectedInput: new Size(500, 1000), returns: new Wrap())
                .CheckMeasureResult(new Wrap());
        }
        
        [Test]
        public void Measure_ReturnsWrap_WhenNotPageable_AndAnyChildReturnsPartialRender()
        {
            TestPlan
                .For(x => new Stack
                {
                    Spacing = 50,
                    Pageable = false,
                    Children = new []
                    {
                        x.CreateChild("a"),
                        x.CreateChild("b"),
                        x.CreateChild("d"),
                        x.CreateChild("e")
                    }
                })
                .MeasureElement(new Size(500, 1000))
                .ExpectChildMeasure("a", expectedInput: new Size(500, 1000), returns: new FullRender(400, 200))
                .ExpectChildMeasure("b", expectedInput: new Size(500, 750), returns: new PartialRender(300, 500))
                .CheckMeasureResult(new Wrap());
        }
        
        [Test]
        public void Measure_DoNotApplySpacing_WhenNotPageable_AndChildReturnsNoContent()
        {
            TestPlan
                .For(x => new Stack
                {
                    Spacing = 50,
                    Pageable = false,
                    Children = new []
                    {
                        x.CreateChild("a"),
                        x.CreateChild("b"),
                        x.CreateChild("c"),
                        x.CreateChild("d"),
                        x.CreateChild("e")
                    }
                })
                .MeasureElement(new Size(500, 1000))
                .ExpectChildMeasure("a", expectedInput: new Size(500, 1000), returns: new FullRender(400, 200))
                .ExpectChildMeasure("b", expectedInput: new Size(500, 750), returns: new FullRender(Size.Zero))
                .ExpectChildMeasure("c", expectedInput: new Size(500, 750), returns: new FullRender(Size.Zero))
                .ExpectChildMeasure("d", expectedInput: new Size(500, 750), returns: new FullRender(300, 300))
                .ExpectChildMeasure("e", expectedInput: new Size(500, 400), returns: new FullRender(300, 100))
                .CheckMeasureResult(new FullRender(500, 700));
        }
        
        [Test]
        public void MultipleDraw_WhenNotPageable()
        {
            TestPlan
                .For(x => new Stack
                {
                    Spacing = 50,
                    Pageable = false,
                    Children = new []
                    {
                        x.CreateChild("a")
                    }
                })
                .MeasureElement(new Size(500, 1000))
                .ExpectChildMeasure("a", expectedInput: new Size(500, 1000), returns: new FullRender(400, 200))
                .CheckMeasureResult(new FullRender(500, 200))
                
                // second measure resets element state
                .MeasureElement(new Size(500, 1000))
                .ExpectChildMeasure("a", expectedInput: new Size(500, 1000), returns: new FullRender(400, 200))
                .CheckMeasureResult(new FullRender(500, 200));
        }
        
        [Test]
        public void WhenPageable_AndChildReturnsWrap()
        {
            TestPlan
                .For(x => new Stack
                {
                    Spacing = 50,
                    Pageable = true,
                    Children = new[]
                    {
                        x.CreateChild("a"),
                        x.CreateChild("b"),
                        x.CreateChild("c")
                    }
                })

                // page 1: measure
                .MeasureElement(new Size(500, 1000))
                .ExpectChildMeasure("a", expectedInput: new Size(500, 1000), returns: new FullRender(400, 200))
                .ExpectChildMeasure("b", expectedInput: new Size(500, 750), returns: new Wrap())
                .CheckMeasureResult(new PartialRender(500, 200))

                // page 1: draw
                .DrawElement(new Size(500, 1000))

                .ExpectChildMeasure("a", expectedInput: new Size(500, 1000), returns: new FullRender(400, 200))
                .ExpectCanvasTranslate(0, 0)
                .ExpectChildDraw("a", new Size(500, 200))
                .ExpectCanvasTranslate(0, 0)

                .ExpectChildMeasure("b", expectedInput: new Size(500, 750), returns: new Wrap())

                .CheckDrawResult()

                // // page 2: measure
                .MeasureElement(new Size(500, 900))
                .ExpectChildMeasure("b", expectedInput: new Size(500, 900), returns: new FullRender(300, 100))
                .ExpectChildMeasure("c", expectedInput: new Size(500, 750), returns: new FullRender(300, 250))
                .CheckMeasureResult(new FullRender(500, 400))
                
                // page 2: draw
                .DrawElement(new Size(500, 900))
                
                .ExpectChildMeasure("b", expectedInput: new Size(500, 900), returns: new FullRender(300, 100))
                .ExpectCanvasTranslate(0, 0)
                .ExpectChildDraw("b", new Size(500, 100))
                .ExpectCanvasTranslate(0, 0)
                
                .ExpectChildMeasure("c", expectedInput: new Size(500, 750), returns: new FullRender(300, 250))
                .ExpectCanvasTranslate(0, 150)
                .ExpectChildDraw("c", new Size(500, 250))
                .ExpectCanvasTranslate(0, -150)
                
                .CheckDrawResult();
        }
        
        [Test]
        public void Draw_ChildrenArePlacedCorrectly_WhenNotPageable()
        {
            TestPlan
                .For(x => new Stack
                {
                    Spacing = 50,
                    Pageable = false,
                    Children = new[]
                    {
                        x.CreateChild("a"),
                        x.CreateChild("b"),
                        x.CreateChild("c"),
                        x.CreateChild("d")
                    }
                })
                .DrawElement(new Size(500, 1000))

                .ExpectChildMeasure("a", expectedInput: new Size(500, 1000), returns: new FullRender(400, 200))
                .ExpectCanvasTranslate(0, 0)
                .ExpectChildDraw("a", new Size(500, 200))
                .ExpectCanvasTranslate(0, 0)

                .ExpectChildMeasure("b", expectedInput: new Size(500, 750), returns: new FullRender(300, 300))
                .ExpectCanvasTranslate(0, 250)
                .ExpectChildDraw("b", new Size(500, 300))
                .ExpectCanvasTranslate(0, -250)

                .ExpectChildMeasure("c", expectedInput: new Size(500, 400), returns: new FullRender(Size.Zero))

                .ExpectChildMeasure("d", expectedInput: new Size(500, 400), returns: new FullRender(200, 400))
                .ExpectCanvasTranslate(0, 600)
                .ExpectChildDraw("d", new Size(500, 400))
                .ExpectCanvasTranslate(0, -600)

                .CheckDrawResult();
        }
        
        [Test]
        public void WhenPageable_AndChildReturnsPartialRender()
        {
            TestPlan
                .For(x => new Stack
                {
                    Spacing = 50,
                    Pageable = true,
                    Children = new[]
                    {
                        x.CreateChild("a"),
                        x.CreateChild("b"),
                        x.CreateChild("c")
                    }
                })

                // page 1: measure
                .MeasureElement(new Size(500, 1000))
                .ExpectChildMeasure("a", expectedInput: new Size(500, 1000), returns: new FullRender(400, 200))
                .ExpectChildMeasure("b", expectedInput: new Size(500, 750), returns: new PartialRender(300, 300))
                .CheckMeasureResult(new PartialRender(500, 550))

                // page 1: draw
                .DrawElement(new Size(500, 1000))

                .ExpectChildMeasure("a", expectedInput: new Size(500, 1000), returns: new FullRender(400, 200))
                .ExpectCanvasTranslate(0, 0)
                .ExpectChildDraw("a", new Size(500, 200))
                .ExpectCanvasTranslate(0, 0)

                .ExpectChildMeasure("b", expectedInput: new Size(500, 750), returns: new PartialRender(300, 300))
                .ExpectCanvasTranslate(0, 250)
                .ExpectChildDraw("b", new Size(500, 300))
                .ExpectCanvasTranslate(0, -250)
                
                .CheckDrawResult()

                // // page 2: measure
                .MeasureElement(new Size(500, 900))
                .ExpectChildMeasure("b", expectedInput: new Size(500, 900), returns: new FullRender(300, 100))
                .ExpectChildMeasure("c", expectedInput: new Size(500, 750), returns: new FullRender(300, 250))
                .CheckMeasureResult(new FullRender(500, 400))
                
                // page 2: draw
                .DrawElement(new Size(500, 900))
                
                .ExpectChildMeasure("b", expectedInput: new Size(500, 900), returns: new FullRender(300, 100))
                .ExpectCanvasTranslate(0, 0)
                .ExpectChildDraw("b", new Size(500, 100))
                .ExpectCanvasTranslate(0, 0)
                
                .ExpectChildMeasure("c", expectedInput: new Size(500, 750), returns: new FullRender(300, 250))
                .ExpectCanvasTranslate(0, 150)
                .ExpectChildDraw("c", new Size(500, 250))
                .ExpectCanvasTranslate(0, -150)
                
                .CheckDrawResult();
        }
    }
}