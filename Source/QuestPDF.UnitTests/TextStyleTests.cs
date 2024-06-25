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
                .FontFamily("Arial", "Microsoft YaHei")
                .BackgroundColor(Colors.Green.Lighten2)
                .EnableFontFeature(FontFeatures.StandardLigatures);

            var spanTextStyle = TextStyle
                .Default
                .FontFamily("Times New Roman", "Arial", "Calibri")
                .Bold()
                .Strikethrough()
                .BackgroundColor(Colors.Red.Lighten2)
                .DisableFontFeature(FontFeatures.StandardLigatures)
                .EnableFontFeature(FontFeatures.Kerning);
            
            // act
            var targetStyle = spanTextStyle.ApplyInheritedStyle(defaultTextStyle).ApplyGlobalStyle();
            
            // assert
            var expectedStyle = TextStyle.LibraryDefault with
            {
                Id = targetStyle.Id, // expect to break when adding new TextStyle properties, so use the real one
                Size = 20, 
                FontFamilies = new[] { "Times New Roman", "Arial", "Calibri", "Microsoft YaHei", "Lato" },
                FontWeight = FontWeight.Bold,
                BackgroundColor = Colors.Red.Lighten2,
                FontFeatures = new[]
                {
                    (FontFeatures.Kerning, true),
                    (FontFeatures.StandardLigatures, false)
                },
                HasStrikethrough = true
            };

            spanTextStyle.Id.Should().BeGreaterThan(1);
            targetStyle.Should().BeEquivalentTo(expectedStyle);
        }
    }
}