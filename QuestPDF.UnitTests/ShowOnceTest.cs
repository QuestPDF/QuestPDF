using Moq;
using NUnit.Framework;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.MeasureTest;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class ShowOnceTest
    {
        [Test]
        public void Measure_ShouldHandleNullChild() => new ShowOnce().MeasureWithoutChild();
        
        [Test]
        public void Draw_ShouldHandleNullChild() => new ShowOnce().DrawWithoutChild();

        [Test]
        public void ShouldRenderOnce_WhenRenderingCalledMultipleTimes()
        {
            var child = new Mock<Element>();
            
            child
                .Setup(x => x.Measure(It.IsAny<Size>()))
                .Returns(() => new FullRender(Size.Zero));

            var element = new ShowOnce()
            {
                Child = child.Object
            };

            element.Draw(null, Size.Zero);
            element.Draw(null, Size.Zero);
            
            child.Verify(x => x.Draw(It.IsAny<ICanvas>(), It.IsAny<Size>()), Times.Once);
        }
        
        [Test]
        public void Draw_HorizontalRight_VerticalTop()
        {
            TestPlan
                .For(x => new ShowOnce()
                {
                    Child = x.CreateChild("child")
                })
                
                // Measure the element and return result
                .MeasureElement(new Size(300, 200))
                .ExpectChildMeasure("child", new Size(300, 200), new PartialRender(new Size(200, 200)))
                .CheckMeasureResult(new PartialRender(new Size(200, 200)))
                
                // Draw element partially
                .DrawElement(new Size(200, 200))
                .ExpectChildMeasure("child", new Size(200, 200), new PartialRender(new Size(200, 200)))
                .ExpectChildDraw("child", new Size(200, 200))
                .CheckDrawResult()
                
                // Element was not fully drawn
                // It should be measured again for rendering on next page
                .MeasureElement(new Size(800, 200))
                .ExpectChildMeasure("child", new Size(800, 200), new FullRender(new Size(400, 200)))
                .CheckMeasureResult(new FullRender(new Size(400, 200)))

                // Draw element on next page
                // Element was fully drawn at this point
                .DrawElement(new Size(400, 200))
                .ExpectChildMeasure("child", new Size(400, 200), new FullRender(new Size(400, 200)))
                .ExpectChildDraw("child", new Size(400, 200))
                .CheckDrawResult()
                
                // In the next attempt of measuring element, it should behave like empty parent.
                .MeasureElement(new Size(600, 200))
                .CheckMeasureResult(new FullRender(Size.Zero))
                
                // In the next attempt of measuring element, it should not draw its child
                .DrawElement(new Size(600, 200))
                .CheckDrawResult();
        }
    }
}