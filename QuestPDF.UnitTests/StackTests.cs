using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Equivalency;
using NUnit.Framework;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class StackTests
    {
        #region Measure
        
        [Test]
        public void Measure_NoChildren_Empty()
        {
            TestPlan
                .For(x => new Stack())
                .MeasureElement(new Size(500, 1000))
                .CheckMeasureResult(new FullRender(Size.Zero));
        }
        
        [Test]
        public void Measure_ReturnsWrap_WhenPageable_AndAnyChildReturnsWrap()
        {
            TestPlan
                .For(x => new Stack
                {
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
                .ExpectChildMeasure("b", expectedInput: new Size(500, 800), returns: new FullRender(500, 300))
                .ExpectChildMeasure("c", expectedInput: new Size(500, 500), returns: new Wrap())
                .CheckMeasureResult(new PartialRender(500, 500));
        }
        
        [Test]
        public void Measure_ReturnsWrap_WhenPageable_AndFirstChildReturnsWrap()
        {
            TestPlan
                .For(x => new Stack
                {
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
        
        #endregion

        #region Combined

        [Test]
        public void WhenPageable_AndChildReturnsWrap()
        {
            TestPlan
                .For(x => new Stack
                {
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
                .ExpectChildMeasure("b", expectedInput: new Size(500, 800), returns: new Wrap())
                .CheckMeasureResult(new PartialRender(500, 200))

                // page 1: draw
                .DrawElement(new Size(500, 1000))

                .ExpectChildMeasure("a", expectedInput: new Size(500, 1000), returns: new FullRender(400, 200))
                .ExpectCanvasTranslate(0, 0)
                .ExpectChildDraw("a", new Size(500, 200))
                .ExpectCanvasTranslate(0, 0)

                .ExpectChildMeasure("b", expectedInput: new Size(500, 800), returns: new Wrap())

                .CheckDrawResult()

                // // page 2: measure
                .MeasureElement(new Size(500, 900))
                .ExpectChildMeasure("b", expectedInput: new Size(500, 900), returns: new FullRender(300, 200))
                .ExpectChildMeasure("c", expectedInput: new Size(500, 700), returns: new FullRender(300, 300))
                .CheckMeasureResult(new FullRender(500, 500))
                
                // page 2: draw
                .DrawElement(new Size(500, 900))
                
                .ExpectChildMeasure("b", expectedInput: new Size(500, 900), returns: new FullRender(300, 200))
                .ExpectCanvasTranslate(0, 0)
                .ExpectChildDraw("b", new Size(500, 200))
                .ExpectCanvasTranslate(0, 0)
                
                .ExpectChildMeasure("c", expectedInput: new Size(500, 700), returns: new FullRender(300, 300))
                .ExpectCanvasTranslate(0, 200)
                .ExpectChildDraw("c", new Size(500, 300))
                .ExpectCanvasTranslate(0, -200)
                
                .CheckDrawResult();
        }

        [Test]
        public void WhenPageable_AndChildReturnsPartialRender()
        {
            TestPlan
                .For(x => new Stack
                {
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
                .ExpectChildMeasure("b", expectedInput: new Size(500, 800), returns: new PartialRender(300, 300))
                .CheckMeasureResult(new PartialRender(500, 500))

                // page 1: draw
                .DrawElement(new Size(500, 1000))

                .ExpectChildMeasure("a", expectedInput: new Size(500, 1000), returns: new FullRender(400, 200))
                .ExpectCanvasTranslate(0, 0)
                .ExpectChildDraw("a", new Size(500, 200))
                .ExpectCanvasTranslate(0, 0)

                .ExpectChildMeasure("b", expectedInput: new Size(500, 800), returns: new PartialRender(300, 300))
                .ExpectCanvasTranslate(0, 200)
                .ExpectChildDraw("b", new Size(500, 300))
                .ExpectCanvasTranslate(0, -200)
                
                .CheckDrawResult()

                // page 2: measure
                .MeasureElement(new Size(500, 900))
                .ExpectChildMeasure("b", expectedInput: new Size(500, 900), returns: new FullRender(300, 200))
                .ExpectChildMeasure("c", expectedInput: new Size(500, 700), returns: new FullRender(300, 300))
                .CheckMeasureResult(new FullRender(500, 500))
                
                // page 2: draw
                .DrawElement(new Size(500, 900))
                
                .ExpectChildMeasure("b", expectedInput: new Size(500, 900), returns: new FullRender(300, 200))
                .ExpectCanvasTranslate(0, 0)
                .ExpectChildDraw("b", new Size(500, 200))
                .ExpectCanvasTranslate(0, 0)
                
                .ExpectChildMeasure("c", expectedInput: new Size(500, 700), returns: new FullRender(300, 300))
                .ExpectCanvasTranslate(0, 200)
                .ExpectChildDraw("c", new Size(500, 300))
                .ExpectCanvasTranslate(0, -200)
                
                .CheckDrawResult();
        }
        
        #endregion
        
        #region Spacing
        
        [Test]
        public void Fluent_WithoutSpacing()
        {
            // arrange
            var structure = new Container();

            var childA = TestPlan.CreateUniqueElement();
            var childB = TestPlan.CreateUniqueElement();
            var childC = TestPlan.CreateUniqueElement();

            // act
            structure
                .Stack(stack =>
                {
                    stack.Spacing(0);

                    stack.Element(childA);
                    stack.Element(childB);
                    stack.Element(childC);
                });
            
            // assert
            var expected = new Stack
            {
                Children = new List<Element>
                {
                    childA,
                    childB,
                    childC
                }
            };
            
            structure.Child.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering().IncludingAllRuntimeProperties());
        }
        
        [Test]
        public void Fluent_WithSpacing()
        {
            // arrange
            var structure = new Container();

            var childA = TestPlan.CreateUniqueElement();
            var childB = TestPlan.CreateUniqueElement();
            var childC = TestPlan.CreateUniqueElement();

            // act
            structure
                .Stack(stack =>
                {
                    stack.Spacing(100);

                    stack.Element(childA);
                    stack.Element(childB);
                    stack.Element(childC);
                });
            
            // assert
            var expected = new Padding
            {
                Bottom = -100,
                Child = new Stack
                {
                    Children = new List<Element>
                    {
                        new Padding
                        {
                            Bottom = 100,
                            Child = childA
                        },
                        new Padding
                        {
                            Bottom = 100,
                            Child = childB
                        },
                        new Padding
                        {
                            Bottom = 100,
                            Child = childC
                        },
                    }
                }
            };
            
            var traceWriter = new StringBuilderTraceWriter();   
            structure.Child.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering().IncludingAllRuntimeProperties());
            Console.WriteLine( traceWriter.ToString());
        }
        
        #endregion
    }
}