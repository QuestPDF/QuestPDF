using System;
using System.Collections.Generic;
using System.Text.Json;
using FluentAssertions;
using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Helpers;

namespace QuestPDF.UnitTests.TestEngine
{
    internal class TestPlan
    {
        private const string DefaultChildName = "child";

        private static Random Random { get; } = new Random();
        
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
            return new MockCanvas
            {
                TranslateFunc = position =>
                {
                    var expected = GetExpected<CanvasTranslateOperation>();

                    Assert.AreEqual(expected.Position.X, position.X, "Translate X");
                    Assert.AreEqual(expected.Position.Y, position.Y, "Translate Y");
                },
                RotateFunc = angle =>
                {
                    var expected = GetExpected<CanvasRotateOperation>();

                    Assert.AreEqual(expected.Angle, angle, "Rotate angle");
                },
                ScaleFunc = (scaleX, scaleY) =>
                {
                    var expected = GetExpected<CanvasScaleOperation>();

                    Assert.AreEqual(expected.ScaleX, scaleX, "Scale X");
                    Assert.AreEqual(expected.ScaleY, scaleY, "Scale Y");
                },
                DrawRectFunc = (position, size, color) =>
                {
                    var expected = GetExpected<CanvasDrawRectangleOperation>();
                    
                    Assert.AreEqual(expected.Position.X, position.X, "Draw rectangle: X");
                    Assert.AreEqual(expected.Position.Y, position.Y, "Draw rectangle: Y");
                    
                    Assert.AreEqual(expected.Size.Width, size.Width, "Draw rectangle: width");
                    Assert.AreEqual(expected.Size.Height, size.Height, "Draw rectangle: height");
                    
                    Assert.AreEqual(expected.Color, color, "Draw rectangle: color");
                },
                DrawTextFunc = (text, position, style) => 
                {
                    var expected = GetExpected<CanvasDrawTextOperation>();
                    
                    Assert.AreEqual(expected.Text, text);
                    
                    Assert.AreEqual(expected.Position.X, position.X, "Draw text: X");
                    Assert.AreEqual(expected.Position.Y, position.Y, "Draw text: Y");
                    
                    Assert.AreEqual(expected.Style.Color, style.Color, "Draw text: color");
                    Assert.AreEqual(expected.Style.FontType, style.FontType, "Draw text: font");
                    Assert.AreEqual(expected.Style.Size, style.Size, "Draw text: size");
                },
                DrawImageFunc = (image, position, size) =>
                {
                    var expected = GetExpected<CanvasDrawImageOperation>();
                    
                    Assert.AreEqual(expected.Position.X, position.X, "Draw image: X");
                    Assert.AreEqual(expected.Position.Y, position.Y, "Draw image: Y");
                    
                    Assert.AreEqual(expected.Size.Width, size.Width, "Draw image: width");
                    Assert.AreEqual(expected.Size.Height, size.Height, "Draw image: height");
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
                    var expected = GetExpected<ChildMeasureOperation>();

                    Assert.AreEqual(expected.ChildId, id);
                    
                    Assert.AreEqual(expected.Input.Width, availableSpace.Width, $"Measure: width of child '{expected.ChildId}'");
                    Assert.AreEqual(expected.Input.Height, availableSpace.Height, $"Measure: height of child '{expected.ChildId}'");

                    return expected.Output;
                },
                DrawFunc = availableSpace =>
                {
                    var expected = GetExpected<ChildDrawOperation>();

                    Assert.AreEqual(expected.ChildId, id);
                    
                    Assert.AreEqual(expected.Input.Width, availableSpace.Width, $"Draw: width of child '{expected.ChildId}'");
                    Assert.AreEqual(expected.Input.Height, availableSpace.Height, $"Draw: width of child '{expected.ChildId}'");
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
        
        public TestPlan ExpectChildMeasure(Size expectedInput, SpacePlan returns)
        {
            return ExpectChildMeasure(DefaultChildName, expectedInput, returns);
        }
        
        public TestPlan ExpectChildMeasure(string child, Size expectedInput, SpacePlan returns)
        {
            return AddOperation(new ChildMeasureOperation(child, expectedInput, returns));
        }
        
        public TestPlan ExpectChildDraw(Size expectedInput)
        {
            return ExpectChildDraw(DefaultChildName, expectedInput);
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

        public TestPlan ExpectCanvasScale(float scaleX, float scaleY)
        {
            return AddOperation(new CanvasScaleOperation(scaleX, scaleY));
        }
        
        public TestPlan ExpectCanvasRotate(float angle)
        {
            return AddOperation(new CanvasRotateOperation(angle));
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
        
        public TestPlan CheckMeasureResult(SpacePlan expected)
        {
            Element.HandleVisitor(x => x?.Initialize(null, Canvas));
            
            var actual = Element.Measure(OperationInput);
            
            Assert.AreEqual(expected.GetType(), actual.GetType());
            
            Assert.AreEqual(expected.Width, actual.Width, "Measure: width");
            Assert.AreEqual(expected.Height, actual.Height, "Measure: height");
            Assert.AreEqual(expected.Type, actual.Type, "Measure: height");
            
            return this;
        }
        
        public TestPlan CheckDrawResult()
        {
            Element.HandleVisitor(x => x?.Initialize(null, Canvas));
            Element.Draw(OperationInput);
            return this;
        }

        public TestPlan CheckState(Func<Element, bool> condition)
        {
            Assert.IsTrue(condition(Element), "Checking condition");
            return this;
        }

        public TestPlan CheckState<T>(Func<T, bool> condition) where T : Element
        {
            Assert.IsTrue(Element is T);
            Assert.IsTrue(condition(Element as T), "Checking condition");
            return this;
        }
        
        public static Element CreateUniqueElement()
        {
            return new Constrained
            {
                MinWidth = 90,
                MinHeight = 60,
                
                Child = new DynamicImage
                {
                    Source = Placeholders.Image
                }
            };
        }

        public static void CompareOperations(Element value, Element expected, Size? availableSpace = null)
        {
            CompareMeasureOperations(value, expected, availableSpace);
            CompareDrawOperations(value, expected, availableSpace);
        }
        
        private static void CompareMeasureOperations(Element value, Element expected, Size? availableSpace = null)
        {
            availableSpace ??= new Size(400, 300);
            
            var canvas = new FreeCanvas();
            value.HandleVisitor(x => x.Initialize(null, canvas));
            var valueMeasure = value.Measure(availableSpace);
            
            expected.HandleVisitor(x => x.Initialize(null, canvas));
            var expectedMeasure = expected.Measure(availableSpace);
            
            valueMeasure.Should().BeEquivalentTo(expectedMeasure);
        }
        
        private static void CompareDrawOperations(Element value, Element expected, Size? availableSpace = null)
        {
            availableSpace ??= new Size(400, 300);
            
            var valueCanvas = new OperationRecordingCanvas();
            value.HandleVisitor(x => x.Initialize(null, valueCanvas));
            value.Draw(availableSpace);
            
            var expectedCanvas = new OperationRecordingCanvas();
            expected.HandleVisitor(x => x.Initialize(null, expectedCanvas));
            expected.Draw(availableSpace);
            
            valueCanvas.Operations.Should().BeEquivalentTo(expectedCanvas.Operations);
        }
    }
}