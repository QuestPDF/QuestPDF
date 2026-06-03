using NUnit.Framework;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class OffsetTests
    {
        [TestCase(0, 0, "")]
        [TestCase(5, 0, "X=5")]
        [TestCase(-10, 0, "X=-10")]
        [TestCase(0, 15, "Y=15")]
        [TestCase(0, -20, "Y=-20")]
        [TestCase(30, -40, "X=30   Y=-40")]
        [TestCase(1.2345f, -2.3456f, "X=1.2   Y=-2.3")]
        public void CompanionHint(float x, float y, string expected)
        {
            var container = EmptyContainer.Create();

            container.OffsetX(x).OffsetY(y);

            var offsetElement = container.Child as Offset;
            var companionHint = offsetElement?.GetCompanionHint();

            Assert.That(companionHint, Is.EqualTo(expected));
        }

        [Test]
        public void HorizontalOffsetIsCumulative()
        {
            var container = EmptyContainer.Create();

            container.OffsetX(-5).OffsetX(10).OffsetX(20);

            var offsetContainer = container.Child as Offset;
            Assert.That(offsetContainer?.OffsetX, Is.EqualTo(25));
        }

        [Test]
        public void VerticalOffsetIsCumulative()
        {
            var container = EmptyContainer.Create();

            container.OffsetY(5).OffsetY(-10).OffsetY(20);

            var offsetContainer = container.Child as Offset;
            Assert.That(offsetContainer?.OffsetY, Is.EqualTo(15));
        }

        [Test]
        public void HorizontalOffsetSupportsUnitConversion()
        {
            var container = EmptyContainer.Create();

            container.OffsetX(2, Unit.Inch);

            var offsetContainer = container.Child as Offset;
            Assert.That(offsetContainer?.OffsetX, Is.EqualTo(144));
        }

        [Test]
        public void VerticalOffsetSupportsUnitConversion()
        {
            var container = EmptyContainer.Create();

            container.OffsetY(3, Unit.Inch);

            var offsetContainer = container.Child as Offset;
            Assert.That(offsetContainer?.OffsetY, Is.EqualTo(216));
        }
    }
}
