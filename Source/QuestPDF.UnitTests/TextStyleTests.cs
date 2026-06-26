using System;
using System.Collections.Generic;
using NUnit.Framework;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia.Text;

namespace QuestPDF.UnitTests
{
    [TestFixture]
    public class TextStyleTests
    {
        #region Font Color
        
        [Test]
        public void FontColor_Default()
        {
            var textStyle = TextStyle.LibraryDefault;
            Assert.That(textStyle.Color, Is.EqualTo(Colors.Black));
        }
        
        [Test]
        public void SetsCorrectFontColor()
        {
            var textStyle = TextStyle.Default.FontColor(Colors.Blue.Medium);
            Assert.That(textStyle.Color, Is.EqualTo(Colors.Blue.Medium));
        }
        
        [Test]
        public void FontColor_AlsoSetsDecorationColor()
        {
            var textStyle = TextStyle.Default.FontColor(Colors.Green.Darken2);
            Assert.That(textStyle.DecorationColor, Is.EqualTo(Colors.Green.Darken2));
        }
        
        #endregion
        
        #region Background Color
        
        [Test]
        public void BackgroundColor_Default()
        {
            var textStyle = TextStyle.LibraryDefault;
            Assert.That(textStyle.BackgroundColor, Is.EqualTo(Colors.Transparent));
        }
        
        [Test]
        public void SetsCorrectBackgroundColor()
        {
            var textStyle = TextStyle.Default.BackgroundColor(Colors.Yellow.Lighten3);
            Assert.That(textStyle.BackgroundColor, Is.EqualTo(Colors.Yellow.Lighten3));
        }
        
        #endregion
        
        #region Font Family
        
        [Test]
        public void FontFamily_Default()
        {
            var textStyle = TextStyle.LibraryDefault;
            Assert.That(textStyle.FontFamilies, Is.EqualTo(new[] { "Lato" }));
        }
        
        [Test]
        public void SetsCorrectFontFamily_Single()
        {
            var textStyle = TextStyle.Default.FontFamily("Arial");
            Assert.That(textStyle.FontFamilies, Is.EqualTo(new[] { "Arial" }));
        }
        
        [Test]
        public void SetsCorrectFontFamily_Multiple()
        {
            var textStyle = TextStyle.Default.FontFamily("Helvetica", "Arial", "sans-serif");
            Assert.That(textStyle.FontFamilies, Is.EqualTo(new[] { "Helvetica", "Arial", "sans-serif" }));
        }

        [Test]
        public void FontFamily_EmptyArray_ReturnsUnchanged()
        {
            var textStyle = TextStyle.Default.FontFamily([]);
            Assert.That(textStyle.FontFamilies, Is.Null);
        }
        
        [Test]
        public void FontFamily_Null_ReturnsUnchanged()
        {
            var textStyle = TextStyle.Default.FontFamily(null);
            Assert.That(textStyle.FontFamilies, Is.Null);
        }
        
        #endregion
        
        #region Font Size
        
        [Test]
        public void FontSize_Default()
        {
            var textStyle = TextStyle.LibraryDefault;
            Assert.That(textStyle.Size, Is.EqualTo(12));
        }
        
        [Test]
        public void SetsCorrectFontSize()
        {
            var textStyle = TextStyle.Default.FontSize(18);
            Assert.That(textStyle.Size, Is.EqualTo(18));
        }
        
        [TestCase(-10)]
        [TestCase(-5)]
        [TestCase(-float.Epsilon)]
        [TestCase(0)]
        public void FontSize_MustBePositive(float fontSize)
        {
            var exception = Assert.Throws<ArgumentException>(() =>
            {
                TextStyle.Default.FontSize(fontSize);
            });
            
            Assert.That(exception.Message, Is.EqualTo("Font size must be greater than 0."));
        }

        #endregion
        
        #region Line Height
        
        [Test]
        public void LineHeight_Default()
        {
            // special value: 0 = normal line height calculated from font metrics
            
            var textStyle = TextStyle.LibraryDefault;
            Assert.That(textStyle.LineHeight, Is.Zero);
        }
        
        [Test]
        public void SetsCorrectLineHeight()
        {
            var textStyle = TextStyle.Default.LineHeight(1.5f);
            Assert.That(textStyle.LineHeight, Is.EqualTo(1.5f));
        }
        
        [Test]
        public void LineHeight_Null_SetsToNormalLineHeight()
        {
            var textStyle = TextStyle.Default.LineHeight(2.0f).LineHeight(null);
            Assert.That(textStyle.LineHeight, Is.EqualTo(TextStyle.NormalLineHeightCalculatedFromFontMetrics));
        }
        
        [TestCase(-5)]
        [TestCase(-float.Epsilon)]
        public void LineHeightMustBePositive(float lineHeight)
        {
            var exception = Assert.Throws<ArgumentException>(() =>
            {
                TextStyle.Default.LineHeight(lineHeight);
            });
            
            Assert.That(exception.Message, Is.EqualTo("Line height must be greater than 0."));
        }
        
        [Test]
        public void LineHeight_AllowsZero()
        {
            var textStyle = TextStyle.Default.LineHeight(0);
            Assert.That(textStyle.LineHeight, Is.Zero);
        }
        
        #endregion
        
        #region Letter Spacing
        
        [Test]
        public void LetterSpacing_Default()
        {
            var textStyle = TextStyle.LibraryDefault;
            Assert.That(textStyle.LetterSpacing, Is.Zero);
        }
        
        [Test]
        public void SetsCorrectLetterSpacing_Positive()
        {
            var textStyle = TextStyle.Default.LetterSpacing(0.5f);
            Assert.That(textStyle.LetterSpacing, Is.EqualTo(0.5f));
        }
        
        [Test]
        public void SetsCorrectLetterSpacing_Negative()
        {
            var textStyle = TextStyle.Default.LetterSpacing(-0.2f);
            Assert.That(textStyle.LetterSpacing, Is.EqualTo(-0.2f));
        }
        
        [Test]
        public void LetterSpacing_DefaultParameterIsZero()
        {
            var textStyle = TextStyle.Default.LetterSpacing();
            Assert.That(textStyle.LetterSpacing, Is.Zero);
        }
        
        #endregion
        
        #region Word Spacing
        
        [Test]
        public void WordSpacing_Default()
        {
            var textStyle = TextStyle.LibraryDefault;
            Assert.That(textStyle.WordSpacing, Is.Zero);
        }
        
        [Test]
        public void SetsCorrectWordSpacing_Positive()
        {
            var textStyle = TextStyle.Default.WordSpacing(2.0f);
            Assert.That(textStyle.WordSpacing, Is.EqualTo(2.0f));
        }
        
        [Test]
        public void SetsCorrectWordSpacing_Negative()
        {
            var textStyle = TextStyle.Default.WordSpacing(-1.0f);
            Assert.That(textStyle.WordSpacing, Is.EqualTo(-1.0f));
        }
        
        [Test]
        public void WordSpacing_DefaultParameterIsZero()
        {
            var textStyle = TextStyle.Default.WordSpacing();
            Assert.That(textStyle.WordSpacing, Is.Zero);
        }
        
        #endregion
        
        #region Italic
        
        [Test]
        public void Italic_Default()
        {
            var textStyle = TextStyle.LibraryDefault;
            Assert.That(textStyle.IsItalic, Is.False);
        }
        
        [Test]
        public void SetsCorrectItalic_Enabled()
        {
            var textStyle = TextStyle.Default.Italic();
            Assert.That(textStyle.IsItalic, Is.True);
        }
        
        [Test]
        public void SetsCorrectItalic_Disabled()
        {
            var textStyle = TextStyle.Default.Italic().Italic(false);
            Assert.That(textStyle.IsItalic, Is.False);
        }
        
        #endregion
        
        #region Text Decoration
        
        [Test]
        public void TextDecoration_Default()
        {
            var textStyle = TextStyle.LibraryDefault;
            Assert.That(textStyle.HasStrikethrough, Is.False);
            Assert.That(textStyle.HasUnderline, Is.False);
            Assert.That(textStyle.HasOverline, Is.False);
            Assert.That(textStyle.DecorationColor, Is.EqualTo(Colors.Black));
            Assert.That(textStyle.DecorationThickness, Is.EqualTo(1f));
            Assert.That(textStyle.DecorationStyle, Is.EqualTo(TextStyleConfiguration.TextDecorationStyle.Solid));
        }
        
        [Test]
        public void SetsCorrectTextDecoration_Strikethrough()
        {
            var textStyle = TextStyle.Default.Strikethrough();
            Assert.That(textStyle.HasStrikethrough, Is.True);
        }
        
        [Test]
        public void SetsCorrectTextDecoration_StrikethroughDisabled()
        {
            var textStyle = TextStyle.Default.Strikethrough().Strikethrough(false);
            Assert.That(textStyle.HasStrikethrough, Is.False);
        }
        
        [Test]
        public void SetsCorrectTextDecoration_Underline()
        {
            var textStyle = TextStyle.Default.Underline();
            Assert.That(textStyle.HasUnderline, Is.True);
        }
        
        [Test]
        public void SetsCorrectTextDecoration_UnderlineDisabled()
        {
            var textStyle = TextStyle.Default.Underline().Underline(false);
            Assert.That(textStyle.HasUnderline, Is.False);
        }
        
        [Test]
        public void SetsCorrectTextDecoration_Overline()
        {
            var textStyle = TextStyle.Default.Overline();
            Assert.That(textStyle.HasOverline, Is.True);
        }
        
        [Test]
        public void SetsCorrectTextDecoration_OverlineDisabled()
        {
            var textStyle = TextStyle.Default.Overline().Overline(false);
            Assert.That(textStyle.HasOverline, Is.False);
        }
        
        [Test]
        public void SetsCorrectTextDecoration_DecorationColor()
        {
            var textStyle = TextStyle.Default.DecorationColor(Colors.Red.Medium);
            Assert.That(textStyle.DecorationColor, Is.EqualTo(Colors.Red.Medium));
        }
        
        [Test]
        public void SetsCorrectTextDecoration_DecorationThickness()
        {
            var textStyle = TextStyle.Default.DecorationThickness(1.5f);
            Assert.That(textStyle.DecorationThickness, Is.EqualTo(1.5f));
        }
        
        [Test]
        public void SetsCorrectTextDecoration_DecorationSolid()
        {
            var textStyle = TextStyle.Default.DecorationSolid();
            Assert.That(textStyle.DecorationStyle, Is.EqualTo(TextStyleConfiguration.TextDecorationStyle.Solid));
        }
        
        [Test]
        public void SetsCorrectTextDecoration_DecorationDouble()
        {
            var textStyle = TextStyle.Default.DecorationDouble();
            Assert.That(textStyle.DecorationStyle, Is.EqualTo(TextStyleConfiguration.TextDecorationStyle.Double));
        }
        
        [Test]
        public void SetsCorrectTextDecoration_DecorationWavy()
        {
            var textStyle = TextStyle.Default.DecorationWavy();
            Assert.That(textStyle.DecorationStyle, Is.EqualTo(TextStyleConfiguration.TextDecorationStyle.Wavy));
        }
        
        [Test]
        public void SetsCorrectTextDecoration_DecorationDotted()
        {
            var textStyle = TextStyle.Default.DecorationDotted();
            Assert.That(textStyle.DecorationStyle, Is.EqualTo(TextStyleConfiguration.TextDecorationStyle.Dotted));
        }
        
        [Test]
        public void SetsCorrectTextDecoration_DecorationDashed()
        {
            var textStyle = TextStyle.Default.DecorationDashed();
            Assert.That(textStyle.DecorationStyle, Is.EqualTo(TextStyleConfiguration.TextDecorationStyle.Dashed));
        }
        
        #endregion
        
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
        
        #region Text Direction
        
        [Test]
        public void TextDirection_Default()
        {
            var textStyle = TextStyle.LibraryDefault;
            Assert.That(textStyle.Direction, Is.EqualTo(TextDirection.Auto));
        }
        
        [Test]
        public void SetsCorrectTextDirection_LeftToRight()
        {
            var textStyle = TextStyle.Default.DirectionFromLeftToRight();
            Assert.That(textStyle.Direction, Is.EqualTo(TextDirection.LeftToRight));
        }
        
        [Test]
        public void SetsCorrectTextDirection_RightToLeft()
        {
            var textStyle = TextStyle.Default.DirectionFromRightToLeft();
            Assert.That(textStyle.Direction, Is.EqualTo(TextDirection.RightToLeft));
        }
        
        [Test]
        public void SetsCorrectTextDirection_Auto()
        {
            var textStyle = TextStyle.Default.DirectionFromRightToLeft().DirectionAuto(); // first change from default, then auto
            Assert.That(textStyle.Direction, Is.EqualTo(TextDirection.Auto));
        }
        
        #endregion
        
        #region Font Features

        [Test]
        public void FontFeatures_Default()
        {
            var textStyle = TextStyle.LibraryDefault;
            Assert.That(textStyle.FontFeatures, Is.Empty);
        }
        
        [Test]
        public void EnableFontFeature_SingleFeature()
        {
            var textStyle = TextStyle.Default
                .EnableFontFeature(FontFeatures.StandardLigatures);
            
            Assert.That(textStyle.FontFeatures, Has.Length.EqualTo(1));
            Assert.That(textStyle.FontFeatures[0], Is.EqualTo((FontFeatures.StandardLigatures, true)));
        }
        
        [Test]
        public void DisableFontFeature_SingleFeature()
        {
            var textStyle = TextStyle.Default
                .DisableFontFeature(FontFeatures.Kerning);
            
            Assert.That(textStyle.FontFeatures, Has.Length.EqualTo(1));
            Assert.That(textStyle.FontFeatures[0], Is.EqualTo((FontFeatures.Kerning, false)));
        }
        
        // NOTE: font features applied further down the chain override those applied earlier, and will appear first in the list
        
        [Test]
        public void FontFeatures_MixedEnableDisable()
        {
            var textStyle = TextStyle.Default
                .EnableFontFeature(FontFeatures.StandardLigatures)
                .DisableFontFeature(FontFeatures.Kerning)
                .EnableFontFeature(FontFeatures.DiscretionaryLigatures);
            
            Assert.That(textStyle.FontFeatures, Has.Length.EqualTo(3));
            Assert.That(textStyle.FontFeatures[0], Is.EqualTo((FontFeatures.DiscretionaryLigatures, true)));
            Assert.That(textStyle.FontFeatures[1], Is.EqualTo((FontFeatures.Kerning, false)));
            Assert.That(textStyle.FontFeatures[2], Is.EqualTo((FontFeatures.StandardLigatures, true)));
        }
        
        [Test]
        public void FontFeatures_OverrideSameFeature()
        {
            var textStyle = TextStyle.Default
                .EnableFontFeature(FontFeatures.StandardLigatures)
                .DisableFontFeature(FontFeatures.Kerning)
                .DisableFontFeature(FontFeatures.Kerning)
                .DisableFontFeature(FontFeatures.StandardLigatures);
            
            Assert.That(textStyle.FontFeatures, Has.Length.EqualTo(2));
            Assert.That(textStyle.FontFeatures[0], Is.EqualTo((FontFeatures.StandardLigatures, false)));
            Assert.That(textStyle.FontFeatures[1], Is.EqualTo((FontFeatures.Kerning, false)));
        }
        
        #endregion
        
        // TODO: add tests for text style inheritance
        
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

        [TestCase(true, FontWeight.Bold)]
        [TestCase(false, FontWeight.Normal)]
        public void BoldWithBooleanParameter(bool isBold, FontWeight expectedWeight)
        {
            // arrange
            var textStyle = TextStyle.Default.ExtraBold();

            // act
            textStyle = textStyle.Bold(isBold);

            // assert
            textStyle.FontWeight.Should().Be(expectedWeight);
        }
    }
}