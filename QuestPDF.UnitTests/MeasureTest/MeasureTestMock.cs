using System;
using System.Collections.Generic;
using System.Text.Json;
using FluentAssertions;
using NUnit.Framework;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.UnitTests.MeasureTest
{
    public abstract class Operation
    {
        
    }

    public class ElementMeasureOperation : Operation
    {
        public ElementMeasureOperation(Size input)
        {
            
        }
    }
    
    internal class ChildMeasureOperation : Operation
    {
        public string ChildId { get; }
        public Size Input { get; }
        public ISpacePlan Output { get; }

        public ChildMeasureOperation(string childId, Size input, ISpacePlan output)
        {
            ChildId = childId;
            Input = input;
            Output = output;
        }
    }

    public class ChildDrawOperation : Operation
    {
        public string ChildId { get; }
        public Size Input { get; }

        public ChildDrawOperation(string childId, Size input)
        {
            ChildId = childId;
            Input = input;
        }
    }
    
    internal class CanvasTranslateOperation : Operation
    {
        public Position Position { get; }

        public CanvasTranslateOperation(Position position)
        {
            Position = position;
        }
    }
    
    internal class CanvasDrawRectangleOperation : Operation
    {
        public Position Position { get; } 
        public Size Size { get; }
        public string Color { get; }

        public CanvasDrawRectangleOperation(Position position, Size size, string color)
        {
            Position = position;
            Size = size;
            Color = color;
        }
    }
    
    internal class CanvasDrawTextOperation : Operation
    {
        public string Text { get; }
        public Position Position { get; }
        public TextStyle Style { get; }

        public CanvasDrawTextOperation(string text, Position position, TextStyle style)
        {
            Text = text;
            Position = position;
            Style = style;
        }
    }
    
    internal class CanvasDrawImageOperation : Operation
    {
        public Position Position { get; }
        public Size Size { get; }

        public CanvasDrawImageOperation(Position position, Size size)
        {
            Position = position;
            Size = size;
        }
    }
    
    internal class ElementMock : Element
    {
        public string Id { get; set; }
        public Func<Size, ISpacePlan> MeasureFunc { get; set; }
        public Action<Size> DrawFunc { get; set; }

        internal override ISpacePlan Measure(Size availableSpace) => MeasureFunc(availableSpace);
        internal override void Draw(ICanvas canvas, Size availableSpace) => DrawFunc(availableSpace);
    }

    internal class CanvasMock : ICanvas
    {
        public Action<Position> TranslateFunc { get; set; }
        public Action<SKImage, Position, Size> DrawImageFunc { get; set; }
        public Action<string, Position, TextStyle> DrawTextFunc { get; set; }
        public Action<Position, Size, string> DrawRectFunc { get; set; }
        
        public void Translate(Position vector) => TranslateFunc(vector);
        public void DrawRectangle(Position vector, Size size, string color) => DrawRectFunc(vector, size, color);
        public void DrawText(string text, Position position, TextStyle style) => DrawTextFunc(text, position, style);
        public void DrawImage(SKImage image, Position position, Size size) => DrawImageFunc(image, position, size);
        
        public void DrawLink(string url, Size size)
        {
            throw new NotImplementedException();
        }

        public Size MeasureText(string text, TextStyle style)
        {
            return new Size(text.Length * style.Size, style.Size);
        }
    }
    
    internal class TestPlan
    {
        private Element Element { get; set; }
        private ICanvas Canvas { get; }
        
        private Size OperationInput { get; set; }
        private Queue<Operation> Operations { get; } = new Queue<Operation>();

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

        private T GetExpected<T>() where T : Operation
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
                    var expected = GetExpected<CanvasTranslateOperation>();
                    
                    Assert.AreEqual(expected.Position.X, position.X);
                    Assert.AreEqual(expected.Position.Y, position.Y);
                    
                    //position.Should().BeEquivalentTo(expected.Position);
                },
                DrawRectFunc = (position, size, color) =>
                {
                    var expected = GetExpected<CanvasDrawRectangleOperation>();
                    
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
                    var expected = GetExpected<CanvasDrawTextOperation>();
                    
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
                    var expected = GetExpected<CanvasDrawImageOperation>();
                    
                    Assert.AreEqual(expected.Position.X, position.X);
                    Assert.AreEqual(expected.Position.Y, position.Y);
                    
                    Assert.AreEqual(expected.Size.Width, size.Width);
                    Assert.AreEqual(expected.Size.Height, size.Height);
                    
                    /*position.Should().BeEquivalentTo(expected.Position);
                    size.Should().BeEquivalentTo(expected.Size);*/
                }
            };
        }
        
        public Element CreateChild(string id)
        {
            return new ElementMock
            {
                Id = id,
                MeasureFunc = availableSpace =>
                {
                    var expected = GetExpected<ChildMeasureOperation>();

                    Assert.AreEqual(expected.ChildId, id);
                    
                    Assert.AreEqual(expected.Input.Width, availableSpace.Width);
                    Assert.AreEqual(expected.Input.Height, availableSpace.Height);

                    // id.Should().Be(expected.ChildId);
                    // availableSpace.Should().Be(expected.Input);

                    return expected.Output;
                },
                DrawFunc = availableSpace =>
                {
                    var expected = GetExpected<ChildDrawOperation>();

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

        private TestPlan AddOperation(Operation operation)
        {
            Operations.Enqueue(operation);
            return this;
        }
        
        public TestPlan ExpectChildMeasure(string child, Size expectedInput, ISpacePlan returns)
        {
            return AddOperation(new ChildMeasureOperation(child, expectedInput, returns));
        }
        
        public TestPlan ExpectChildDraw(string child, Size expectedInput)
        {
            return AddOperation(new ChildDrawOperation(child, expectedInput));
        }

        public TestPlan ExpectCanvasTranslate(Position position)
        {
            return AddOperation(new CanvasTranslateOperation(position));
        }
        
        public TestPlan ExpectCanvasTranslate(float left, float top)
        {
            return AddOperation(new CanvasTranslateOperation(new Position(left, top)));
        }

        public TestPlan ExpectCanvasDrawRectangle(Position position, Size size, string color)
        {
            return AddOperation(new CanvasDrawRectangleOperation(position, size, color));
        }
        
        public TestPlan ExpectCanvasDrawText(string text, Position position, TextStyle style)
        {
            return AddOperation(new CanvasDrawTextOperation(text, position, style));
        }
        
        public TestPlan ExpectCanvasDrawImage(Position position, Size size)
        {
            return AddOperation(new CanvasDrawImageOperation(position, size));
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

    public static class SingleChildTests
    {
        internal static void MeasureWithoutChild<T>(this T element) where T : ContainerElement
        {
            element.Child = null;
            element.Measure(Size.Zero).Should().BeEquivalentTo(new FullRender(Size.Zero));
        }
        
        internal static void DrawWithoutChild<T>(this T element) where T : ContainerElement
        {
            // component does not throw an exception when called with null child
            Assert.DoesNotThrow(() =>
            {
                element.Child = null;
                
                // component does not perform any canvas operation when called with null child
                TestPlan
                    .For(x => element)
                    .DrawElement(new Size(200, 100))
                    .CheckDrawResult();
            });
        }
    }

    [TestFixture]
    public class LetsTest
    {
        [Test]
        public void TestExample()
        {
            TestPlan
                .For(x => new Padding
                {
                    Top = 5,
                    Right = 10,
                    Bottom = 15,
                    Left = 20, 
                    
                    Child = x.CreateChild("child")
                })
                .MeasureElement(new Size(200, 100))
                .ExpectChildMeasure("child", expectedInput: new Size(170, 80), returns: new FullRender(new Size(100, 50)))
                .CheckMeasureResult(new FullRender(130, 70));
        }
        
        [Test]
        public void TestExample2()
        {
            TestPlan
                .For(x => new Padding
                { 
                    Top = 5,
                    Right = 10,
                    Bottom = 15,
                    Left = 20, 
                    
                    Child = x.CreateChild("child")
                })
                .DrawElement(new Size(200, 100))
                .ExpectCanvasTranslate(new Position(20, 5))
                .ExpectChildDraw("child", expectedInput: new Size(170, 80))
                .ExpectCanvasTranslate(new Position(-20, -5))
                .CheckDrawResult();
        }
    }
}