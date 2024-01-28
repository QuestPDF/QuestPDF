using System.Collections.Generic;
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
        public void ApplyInheritedAndGlobalStyle()
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
                .FontFamily("Times New Roman")
                .Bold()
                .Strikethrough()
                .BackgroundColor(Colors.Red.Lighten2);
            
            // act
            var targetStyle = spanTextStyle.ApplyInheritedStyle(defaultTextStyle).ApplyGlobalStyle();
            
            // assert
            var expectedStyle = TextStyle.LibraryDefault with
            {
                Id = 20, // expect to break when adding new TextStyle properties
                Size = 20, 
                FontFamily = "Times New Roman",
                FontWeight = FontWeight.Bold,
                BackgroundColor = Colors.Red.Lighten2,
                HasStrikethrough = true
            };

            targetStyle.Should().BeEquivalentTo(expectedStyle);
        }
    }
}