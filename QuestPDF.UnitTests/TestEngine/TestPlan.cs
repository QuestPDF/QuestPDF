using System;
using System.Collections.Generic;
using System.Text.Json;
using NUnit.Framework;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine.Operations;

namespace QuestPDF.UnitTests.TestEngine
{
    internal class TestPlan
    {
        private const string DefaultChildName = "child";
        
        private Element Element { get; set; }
        private ICanvas Canvas { get; }
        
        private Size OperationInput { get; set; }
        private Queue<OperationBase> Operations { get; } = new Queue<OperationBase>();

        public TestPlan()
        {
            Canvas = CreateCanvas();
        }
        
        public static TestPlan For(Func<TestPlan, Element> create)
        {
            var plan = new TestPlan();
            plan.Element = create(plan);
            
            return plan;
        }

        private T GetExpected<T>() where T : OperationBase
        {
            if (Operations.TryDequeue(out var value) && value is T result)
                return result;

            var gotType = value?.GetType()?.Name ?? "null";
            Assert.Fail($"Expected: {typeof(T).Name}, got {gotType}: {JsonSerializer.Serialize(value)}");
            return null;
        }
        
        private ICanvas CreateCanvas()
        {
            return new CanvasMock
            {
                TranslateFunc = position =>
                {
                    var expected = GetExpected<CanvasTranslateOperationBase>();
                    
                    Assert.AreEqual(expected.Position.X, position.X);
                    Assert.AreEqual(expected.Position.Y, position.Y);
                    
                    //position.Should().BeEquivalentTo(expected.Position);
                },
                DrawRectFunc = (position, size, color) =>
                {
                    var expected = GetExpected<CanvasDrawRectangleOperationBase>();
                    
                    Assert.AreEqual(expected.Position.X, position.X);
                    Assert.AreEqual(expected.Position.Y, position.Y);
                    
                    Assert.AreEqual(expected.Size.Width, size.Width);
                    Assert.AreEqual(expected.Size.Height, size.Height);
                    
                    Assert.AreEqual(expected.Color, color);
                    
                    /*position.Should().BeEquivalentTo(expected.Position);
                    size.Should().BeEquivalentTo(expected.Size);
                    color.Should().Be(expected.Color);*/
                },
                DrawTextFunc = (text, position, style) => 
                {
                    var expected = GetExpected<CanvasDrawTextOperationBase>();
                    
                    Assert.AreEqual(expected.Text, text);
                    
                    Assert.AreEqual(expected.Position.X, position.X);
                    Assert.AreEqual(expected.Position.Y, position.Y);
                    
                    Assert.AreEqual(expected.Style.Color, style.Color);
                    Assert.AreEqual(expected.Style.FontType, style.FontType);
                    Assert.AreEqual(expected.Style.Size, style.Size);

                    /*text.Should().Be(expected.Text);
                    position.Should().BeEquivalentTo(expected.Position);
                    style.Should().BeEquivalentTo(expected.Style);*/
                },
                DrawImageFunc = (image, position, size) =>
                {
                    var expected = GetExpected<CanvasDrawImageOperationBase>();
                    
                    Assert.AreEqual(expected.Position.X, position.X);
                    Assert.AreEqual(expected.Position.Y, position.Y);
                    
                    Assert.AreEqual(expected.Size.Width, size.Width);
                    Assert.AreEqual(expected.Size.Height, size.Height);
                    
                    /*position.Should().BeEquivalentTo(expected.Position);
                    size.Should().BeEquivalentTo(expected.Size);*/
                }
            };
        }

        public Element CreateChild() => CreateChild(DefaultChildName);
        
        public Element CreateChild(string id)
        {
            return new ElementMock
            {
                Id = id,
                MeasureFunc = availableSpace =>
                {
                    var expected = GetExpected<ChildMeasureOperationBase>();

                    Assert.AreEqual(expected.ChildId, id);
                    
                    Assert.AreEqual(expected.Input.Width, availableSpace.Width);
                    Assert.AreEqual(expected.Input.Height, availableSpace.Height);

                    // id.Should().Be(expected.ChildId);
                    // availableSpace.Should().Be(expected.Input);

                    return expected.Output;
                },
                DrawFunc = availableSpace =>
                {
                    var expected = GetExpected<ChildDrawOperationBase>();

                    Assert.AreEqual(expected.ChildId, id);
                    
                    Assert.AreEqual(expected.Input.Width, availableSpace.Width);
                    Assert.AreEqual(expected.Input.Height, availableSpace.Height);
                    
                    /*id.Should().Be(expected.ChildId);
                    availableSpace.Should().Be(expected.Input);*/
                }
            };
        }
        
        public TestPlan MeasureElement(Size input)
        {
            OperationInput = input;
            return this;
        }
        
        public TestPlan DrawElement(Size input)
        {
            OperationInput = input;
            return this;
        }

        private TestPlan AddOperation(OperationBase operationBase)
        {
            Operations.Enqueue(operationBase);
            return this;
        }
        
        public TestPlan ExpectChildMeasure(Size expectedInput, ISpacePlan returns)
        {
            return ExpectChildMeasure(DefaultChildName, expectedInput, returns);
        }
        
        public TestPlan ExpectChildMeasure(string child, Size expectedInput, ISpacePlan returns)
        {
            return AddOperation(new ChildMeasureOperationBase(child, expectedInput, returns));
        }
        
        public TestPlan ExpectChildDraw(Size expectedInput)
        {
            return ExpectChildDraw(DefaultChildName, expectedInput);
        }
        
        public TestPlan ExpectChildDraw(string child, Size expectedInput)
        {
            return AddOperation(new ChildDrawOperationBase(child, expectedInput));
        }

        public TestPlan ExpectCanvasTranslate(Position position)
        {
            return AddOperation(new CanvasTranslateOperationBase(position));
        }
        
        public TestPlan ExpectCanvasTranslate(float left, float top)
        {
            return AddOperation(new CanvasTranslateOperationBase(new Position(left, top)));
        }

        public TestPlan ExpectCanvasDrawRectangle(Position position, Size size, string color)
        {
            return AddOperation(new CanvasDrawRectangleOperationBase(position, size, color));
        }
        
        public TestPlan ExpectCanvasDrawText(string text, Position position, TextStyle style)
        {
            return AddOperation(new CanvasDrawTextOperationBase(text, position, style));
        }
        
        public TestPlan ExpectCanvasDrawImage(Position position, Size size)
        {
            return AddOperation(new CanvasDrawImageOperationBase(position, size));
        }
        
        public TestPlan CheckMeasureResult(ISpacePlan expected)
        {
            var actual = Element.Measure(OperationInput);
            
            Assert.AreEqual(expected.GetType(), actual.GetType());

            var expectedSize = expected as Size;
            var actualSize = actual as Size;

            if (expectedSize != null)
            {
                Assert.AreEqual(expectedSize.Width, actualSize.Width);
                Assert.AreEqual(expectedSize.Height, actualSize.Height);
            }
            
            return this;
        }
        
        public TestPlan CheckDrawResult()
        {
            Element.Draw(Canvas, OperationInput);
            return this;
        }
    }
}