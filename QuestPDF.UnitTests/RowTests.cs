using NUnit.Framework;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Elements;
using QuestPDF.Fluent;
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
        
        #region Structure
        
        [Test]
        public void Structure_RelativeColumnsHandling()
        { 
            // arrange
            var childA = TestPlan.CreateUniqueElement();
            var childB = TestPlan.CreateUniqueElement();
            var childC = TestPlan.CreateUniqueElement();
            var childD = TestPlan.CreateUniqueElement();
            var childE = TestPlan.CreateUniqueElement();

            const int spacing = 25;
            var availableSpace = new Size(1100, 400);
            
            // act
            var value = new Container();

            value.Row(row =>
            {
                row.Spacing(spacing);
                
                row.ConstantColumn(150).Element(childA);
                row.ConstantColumn(250).Element(childB);
                row.RelativeColumn(1).Element(childC);
                row.RelativeColumn(2).Element(childD);
                row.RelativeColumn(3).Element(childE);
            });
            
            // assert
            var expected = new Container();

            expected.Row(row =>
            {
                row.Spacing(spacing);
                
                row.ConstantColumn(150).Element(childA);
                row.ConstantColumn(250).Element(childB);
                row.ConstantColumn(100).Element(childC);
                row.ConstantColumn(200).Element(childD);
                row.ConstantColumn(300).Element(childE);
            });
            
            TestPlan.CompareOperations(value, expected, availableSpace);
        }
        
        [Test]
        public void Structure_Tree()
        { 
            // arrange
            var childA = TestPlan.CreateUniqueElement();
            var childB = TestPlan.CreateUniqueElement();
            var childC = TestPlan.CreateUniqueElement();
            var childD = TestPlan.CreateUniqueElement();
            var childE = TestPlan.CreateUniqueElement();

            const int spacing = 25;
            var availableSpace = new Size(1200, 400);
            
            // act
            var value = new Container();

            value.Row(row =>
            {
                row.Spacing(spacing);
                
                row.ConstantColumn(150).Element(childA);
                row.ConstantColumn(200).Element(childB);
                row.ConstantColumn(250).Element(childC);
                row.RelativeColumn(2).Element(childD);
                row.RelativeColumn(3).Element(childE);
            });
            
            // assert
            var expected = new SimpleRow
            {
                Left = new SimpleRow
                {
                    Left = new SimpleRow
                    {
                        Left = new Constrained
                        {
                            MinWidth = 150,
                            MaxWidth = 150,
                            Child = childA
                        },
                        Right = new Constrained
                        {
                            MinWidth = 25,
                            MaxWidth = 25
                        }
                    },
                    Right = new SimpleRow
                    {
                        Left = new Constrained
                        {
                            MinWidth = 200,
                            MaxWidth = 200,
                            Child = childB
                        },
                        Right = new Constrained
                        {
                            MinWidth = 25,
                            MaxWidth = 25
                        }
                    }
                },
                Right = new SimpleRow
                {
                    Left = new SimpleRow
                    {
                        Left = new Constrained
                        {
                            MinWidth = 250,
                            MaxWidth = 250,
                            Child = childC
                        },
                        Right = new Constrained
                        {
                            MinWidth = 25,
                            MaxWidth = 25
                        }
                    },
                    Right = new SimpleRow
                    {
                        Left = new Constrained
                        {
                            MinWidth = 200,
                            MaxWidth = 200,
                            Child = childD
                        },
                        Right = new SimpleRow
                        {
                            Left = new Constrained
                            {
                                MinWidth = 25,
                                MaxWidth = 25
                            },
                            Right = new Constrained
                            {
                                MinWidth = 300,
                                MaxWidth = 300,
                                Child = childE
                            }
                        }
                    }
                }
            };
            
            TestPlan.CompareOperations(value, expected, availableSpace);
        }
        
        #endregion
    }
}