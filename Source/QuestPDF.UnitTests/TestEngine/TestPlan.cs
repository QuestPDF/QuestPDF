using System;
using System.Collections.Generic;
using System.Text.Json;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine.Operations;

namespace QuestPDF.UnitTests.TestEngine
{
    internal sealed class TestPlan
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

                    ClassicAssert.AreEqual(expected.Position.X, position.X, "Translate X");
                    ClassicAssert.AreEqual(expected.Position.Y, position.Y, "Translate Y");
                },
                RotateFunc = angle =>
                {
                    var expected = GetExpected<CanvasRotateOperation>();

                    ClassicAssert.AreEqual(expected.Angle, angle, "Rotate angle");
                },
                ScaleFunc = (scaleX, scaleY) =>
                {
                    var expected = GetExpected<CanvasScaleOperation>();

                    ClassicAssert.AreEqual(expected.ScaleX, scaleX, "Scale X");
                    ClassicAssert.AreEqual(expected.ScaleY, scaleY, "Scale Y");
                },
                DrawRectFunc = (position, size, color) =>
                {
                    var expected = GetExpected<CanvasDrawRectangleOperation>();
                    
                    ClassicAssert.AreEqual(expected.Position.X, position.X, "Draw rectangle: X");
                    ClassicAssert.AreEqual(expected.Position.Y, position.Y, "Draw rectangle: Y");
                    
                    ClassicAssert.AreEqual(expected.Size.Width, size.Width, "Draw rectangle: width");
                    ClassicAssert.AreEqual(expected.Size.Height, size.Height, "Draw rectangle: height");
                    
                    ClassicAssert.AreEqual(expected.Color, color, "Draw rectangle: color");
                },
                DrawImageFunc = (image, position, size) =>
                {
                    var expected = GetExpected<CanvasDrawImageOperation>();
                    
                    ClassicAssert.AreEqual(expected.Position.X, position.X, "Draw image: X");
                    ClassicAssert.AreEqual(expected.Position.Y, position.Y, "Draw image: Y");
                    
                    ClassicAssert.AreEqual(expected.Size.Width, size.Width, "Draw image: width");
                    ClassicAssert.AreEqual(expected.Size.Height, size.Height, "Draw image: height");
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

                    ClassicAssert.AreEqual(expected.ChildId, id);
                    
                    ClassicAssert.AreEqual(expected.Input.Width, availableSpace.Width, $"Measure: width of child '{expected.ChildId}'");
                    ClassicAssert.AreEqual(expected.Input.Height, availableSpace.Height, $"Measure: height of child '{expected.ChildId}'");

                    return expected.Output;
                },
                DrawFunc = availableSpace =>
                {
                    var expected = GetExpected<ChildDrawOperation>();

                    ClassicAssert.AreEqual(expected.ChildId, id);
                    
                    ClassicAssert.AreEqual(expected.Input.Width, availableSpace.Width, $"Draw: width of child '{expected.ChildId}'");
                    ClassicAssert.AreEqual(expected.Input.Height, availableSpace.Height, $"Draw: width of child '{expected.ChildId}'");
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
        
        public TestPlan ExpectCanvasDrawRectangle(Position position, Size size, Color color)
        {
            return AddOperation(new CanvasDrawRectangleOperation(position, size, color));
        }
        
        public TestPlan ExpectCanvasDrawImage(Position position, Size size)
        {
            return AddOperation(new CanvasDrawImageOperation(position, size));
        }
        
        public TestPlan CheckMeasureResult(SpacePlan expected)
        {
            Element.InjectDependencies(null, Canvas);
            
            var actual = Element.Measure(OperationInput);
            
            ClassicAssert.AreEqual(expected.GetType(), actual.GetType());
            
            ClassicAssert.AreEqual(expected.Width, actual.Width, "Measure: width");
            ClassicAssert.AreEqual(expected.Height, actual.Height, "Measure: height");
            ClassicAssert.AreEqual(expected.Type, actual.Type, "Measure: height");
            
            return this;
        }
        
        public TestPlan CheckDrawResult()
        {
            Element.InjectDependencies(null, Canvas);
            Element.Draw(OperationInput);
            return this;
        }

        public TestPlan CheckState(Func<Element, bool> condition)
        {
            ClassicAssert.IsTrue(condition(Element), "Checking condition");
            return this;
        }

        public TestPlan CheckState<T>(Func<T, bool> condition) where T : Element
        {
            ClassicAssert.IsTrue(Element is T);
            ClassicAssert.IsTrue(condition(Element as T), "Checking condition");
            return this;
        }
        
        public static Element CreateUniqueElement()
        {
            var content = new Container();

            content
                .AspectRatio(4 / 3f)
                .Image(Placeholders.Image);
            
            return content;
        }

        public static void CompareOperations(Element value, Element expected, Size? availableSpace = null)
        {
            CompareMeasureOperations(value, expected, availableSpace);
            CompareDrawOperations(value, expected, availableSpace);
        }
        
        private static void CompareMeasureOperations(Element value, Element expected, Size? availableSpace = null)
        {
            availableSpace ??= new Size(400, 900);
            
            var canvas = new FreeCanvas();
            value.InjectDependencies(null, canvas);
            var valueMeasure = value.Measure(availableSpace.Value);
            
            expected.InjectDependencies(null, canvas);
            var expectedMeasure = expected.Measure(availableSpace.Value);
            
            valueMeasure.Should().BeEquivalentTo(expectedMeasure);
        }
        
        private static void CompareDrawOperations(Element value, Element expected, Size? availableSpace = null)
        {
            availableSpace ??= new Size(400, 1200);
            
            var valueCanvas = new OperationRecordingCanvas();
            value.InjectDependencies(null, valueCanvas);
            value.ApplyDefaultImageConfiguration(144, ImageCompressionQuality.Medium, false);
            value.Draw(availableSpace.Value);
            
            var expectedCanvas = new OperationRecordingCanvas();
            expected.InjectDependencies(null, expectedCanvas);
            expected.ApplyDefaultImageConfiguration(144, ImageCompressionQuality.Medium, false);
            expected.Draw(availableSpace.Value);
            
            valueCanvas.Operations.Should().BeEquivalentTo(expectedCanvas.Operations);
        }
    }
}