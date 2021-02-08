using FluentAssertions;
using Moq;
using NUnit.Framework;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class PaddingTests
    {
        [Test]
        public void Measure_ShouldHandleNullChild() => new Padding().MeasureWithoutChild();
        
        [Test]
        public void Draw_ShouldHandleNullChild() => new Padding().DrawWithoutChild();
        
        private Padding GetPadding(Element child)
        {
            return new Padding()
            {
                Top = 5,
                Right = 10,
                Bottom = 15,
                Left = 20,
                
                Child = child
            };
        }

        [Test]
        public void Measure_WhenChildReturnsWrap_ReturnsWrap()
        {
            var child = new Mock<Element>();
            
            child
                .Setup(x => x.Measure(It.IsAny<Size>()))
                .Returns(() => new Wrap());

            GetPadding(child.Object)
                .Measure(Size.Zero)
                .Should()
                .BeOfType<Wrap>();
        }
        
        [Test]
        public void Measure_WhenChildReturnsFullRender_ReturnsFullRender()
        {
            var child = new Mock<Element>();
            
            child
                .Setup(x => x.Measure(It.IsAny<Size>()))
                .Returns(() => new FullRender(Size.Zero));

            GetPadding(child.Object)
                .Measure(Size.Zero)
                .Should()
                .BeOfType<FullRender>();
        }
        
        [Test]
        public void Measure_WhenChildReturnsPartialRender_ReturnsPartialRender()
        {
            var child = new Mock<Element>();
            
            child
                .Setup(x => x.Measure(It.IsAny<Size>()))
                .Returns(() => new PartialRender(Size.Zero));

            GetPadding(child.Object)
                .Measure(Size.Zero)
                .Should()
                .BeOfType<PartialRender>();
        }
    }
}