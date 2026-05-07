using NUnit.Framework;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class TextLayoutMeasurerTests
    {
        private TextStyle _style;

        [SetUp]
        public void Setup()
        {
            _style = TextStyle
                .Default
                .FontSize(20)
                .FontFamily("Arial")
                .EnableFontFeature(FontFeatures.StandardLigatures);
        }

        [Test]
        public void GetLineCount_ShortText_ShouldReturnSingleLine()
        {
            // Arrange
            const string text = "Hello World";

            // Act
            var lineCount = TextLayoutMeasurer.GetLineCount(
                text,
                _style,
                maxWidth: 300);

            // Assert
            Assert.That(lineCount, Is.EqualTo(1));
        }

        [Test]
        public void GetLineCount_LongText_ShouldWrapToMultipleLines()
        {
            // Arrange
            const string text = "Hello World Hello World Hello World Hello World";

            // Act
            var lineCount = TextLayoutMeasurer.GetLineCount(
                text,
                _style,
                maxWidth: 100);

            // Assert
            Assert.That(lineCount, Is.GreaterThan(1));
        }

        [Test]
        public void GetHeight_MultipleLines_ShouldBeGreaterThanSingleLine()
        {
            // Arrange
            const string shortText = "Hello World";
            const string longText = "Hello World Hello World Hello World Hello World";

            // Act
            var singleLineHeight = TextLayoutMeasurer.GetHeight(
                shortText,
                _style,
                maxWidth: 300);

            var multiLineHeight = TextLayoutMeasurer.GetHeight(
                longText,
                _style,
                maxWidth: 100);

            // Assert
            Assert.That(multiLineHeight, Is.GreaterThan(singleLineHeight));
        }

        [Test]
        public void GetHeight_WithPadding_ShouldReduceAvailableWidthAndIncreaseHeight()
        {
            // Arrange
            const string text = "Hello World Hello World Hello World";

            // Act
            var heightWithoutPadding = TextLayoutMeasurer.GetHeight(
                text,
                _style,
                maxWidth: 300);

            var heightWithPadding = TextLayoutMeasurer.GetHeight(
                text,
                _style,
                maxWidth: 300,
                paddingLeft: 100,
                paddingRight: 100);

            // Assert
            Assert.That(heightWithPadding, Is.GreaterThan(heightWithoutPadding));
        }

        [Test]
        public void GetLineCount_EmptyText_ShouldReturnZero()
        {
            // Arrange
            const string text = "";

            // Act
            var lineCount = TextLayoutMeasurer.GetLineCount(
                text,
                _style,
                maxWidth: 300);

            // Assert
            Assert.That(lineCount, Is.Zero);
        }

        [Test]
        public void GetHeight_EmptyText_ShouldReturnZero()
        {
            // Arrange
            const string text = "   ";

            // Act
            var height = TextLayoutMeasurer.GetHeight(
                text,
                _style,
                maxWidth: 300);

            // Assert
            Assert.That(height, Is.Zero);
        }

        [Test]
        public void GetLineCount_ZeroEffectiveWidth_ShouldReturnZero()
        {
            // Arrange
            const string text = "Hello World";

            // Act
            var lineCount = TextLayoutMeasurer.GetLineCount(
                text,
                _style,
                maxWidth: 50,
                paddingLeft: 30,
                paddingRight: 30);

            // Assert
            Assert.That(lineCount, Is.Zero);
        }
    }
}