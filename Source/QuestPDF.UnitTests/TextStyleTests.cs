using FluentAssertions;
using NUnit.Framework;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class TextStyleTests
    {
        [Test]
        public void Font()
        {
            // arrange
            var defaultTextStyle = TextStyle
                .Default
                .FontSize(20)
                .FontFamily("Arial")
                .BackgroundColor(Colors.Green.Lighten2)
                .Fallback(y => y
                    .FontFamily("Microsoft YaHei")
                    .Underline()
                    .NormalWeight()
                    .BackgroundColor(Colors.Blue.Lighten2));

            var spanTextStyle = TextStyle
                .Default
                .Bold()
                .Strikethrough()
                .BackgroundColor(Colors.Red.Lighten2);
            
            // act
            var targetStyle = spanTextStyle.ApplyInheritedStyle(defaultTextStyle).ApplyGlobalStyle();
            
            // assert
            var expectedStyle = TextStyle.LibraryDefault with
            {
                Size = 20, 
                FontFamily = "Arial",
                FontWeight = FontWeight.Bold,
                BackgroundColor = Colors.Red.Lighten2,
                HasStrikethrough = true,
                Fallback = TextStyle.LibraryDefault with
                {
                    Size = 20,
                    FontFamily = "Microsoft YaHei",
                    FontWeight = FontWeight.Bold,
                    BackgroundColor = Colors.Red.Lighten2,
                    HasUnderline = true,
                    HasStrikethrough = true
                }
            };

            targetStyle.Should().BeEquivalentTo(expectedStyle);
        }
    }
}