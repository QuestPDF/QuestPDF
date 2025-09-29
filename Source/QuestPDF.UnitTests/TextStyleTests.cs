using System.Collections.Generic;
using NUnit.Framework;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class TextStyleTests
    {
        #region Font Weight
        
        [Test]
        public void FontWeight_Default()
        {
            var textStyle = TextStyle.LibraryDefault;
            Assert.That(textStyle.FontWeight, Is.EqualTo(FontWeight.Normal));
        }
        
        [Test]
        public void SetsCorrectSetsCorrectFontWeight_Thin()
        {
            var textStyle = TextStyle.Default.Thin();
            Assert.That(textStyle.FontWeight, Is.EqualTo(FontWeight.Thin));
        }
        
        [Test]
        public void SetsCorrectFontWeight_ExtraLight()
        {
            var textStyle = TextStyle.Default.ExtraLight();
            Assert.That(textStyle.FontWeight, Is.EqualTo(FontWeight.ExtraLight));
        }
        
        [Test]
        public void SetsCorrectFontWeight_Light()
        {
            var textStyle = TextStyle.Default.Light();
            Assert.That(textStyle.FontWeight, Is.EqualTo(FontWeight.Light));
        }
        
        [Test]
        public void SetsCorrectFontWeight_Normal()
        {
            var textStyle = TextStyle.Default.Bold().NormalWeight(); // first change from default, then normal
            Assert.That(textStyle.FontWeight, Is.EqualTo(FontWeight.Normal));
        }
        
        [Test]
        public void SetsCorrectFontWeight_Medium()
        {
            var textStyle = TextStyle.Default.Medium();
            Assert.That(textStyle.FontWeight, Is.EqualTo(FontWeight.Medium));
        }
        
        [Test]
        public void SetsCorrectFontWeight_SemiBold()
        {
            var textStyle = TextStyle.Default.SemiBold();
            Assert.That(textStyle.FontWeight, Is.EqualTo(FontWeight.SemiBold));
        }
        
        [Test]
        public void SetsCorrectFontWeight_Bold()
        {
            var textStyle = TextStyle.Default.Bold();
            Assert.That(textStyle.FontWeight, Is.EqualTo(FontWeight.Bold));
        }
        
        [Test]
        public void SetsCorrectFontWeight_ExtraBold()
        {
            var textStyle = TextStyle.Default.ExtraBold();
            Assert.That(textStyle.FontWeight, Is.EqualTo(FontWeight.ExtraBold));
        }
        
        [Test]
        public void SetsCorrectFontWeight_Black()
        {
            var textStyle = TextStyle.Default.Black();
            Assert.That(textStyle.FontWeight, Is.EqualTo(FontWeight.Black));
        }
        
        [Test]
        public void SetsCorrectFontWeight_ExtraBlack()
        {
            var textStyle = TextStyle.Default.ExtraBlack();
            Assert.That(textStyle.FontWeight, Is.EqualTo(FontWeight.ExtraBlack));
        }
        
        #endregion
        
        #region Text Position
    
        [Test]
        public void TextPosition_Default()
        {
            var textStyle = TextStyle.LibraryDefault;
            Assert.That(textStyle.FontPosition, Is.EqualTo(FontPosition.Normal));
        }
        
        [Test]
        public void SetsCorrectTextPosition_Subscript()
        {
            var textStyle = TextStyle.Default.Subscript();
            Assert.That(textStyle.FontPosition, Is.EqualTo(FontPosition.Subscript));
        }
        
        [Test]
        public void SetsCorrectTextPosition_Normal()
        {
            var textStyle = TextStyle.Default.Subscript().NormalPosition(); // first change from default, then normal
            Assert.That(textStyle.FontPosition, Is.EqualTo(FontPosition.Normal));
        }
        
        [Test]
        public void SetsCorrectTextPosition_Superscript()
        {
            var textStyle = TextStyle.Default.Superscript();
            Assert.That(textStyle.FontPosition, Is.EqualTo(FontPosition.Superscript));
        }
    
        #endregion
        
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

            Assert.That(targetStyle, Is.Not.Null);
            Assert.That(targetStyle.Id, Is.GreaterThan(1));

            Assert.That(targetStyle.ToString(), Is.EqualTo(expectedStyle.ToString()));
        }
    }
}