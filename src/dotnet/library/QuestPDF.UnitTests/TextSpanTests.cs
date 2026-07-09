using System;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Elements.Text;
using QuestPDF.Elements.Text.Items;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia.Text;

namespace QuestPDF.UnitTests;

public class TextSpanTests
{
    internal (TextSpanDescriptor, TextBlockSpan) CreateTextBlockSpan()
    {
        var container = EmptyContainer.Create();
        var descriptor = container.Text("test");

        var textElement = container.Child as TextBlock;
        var textBlockSpan = textElement?.Items.SingleOrDefault() as TextBlockSpan;
        
        return (descriptor, textBlockSpan);
    }
    
    #region Override Style
    
    [Test]
    public void OverridesStyle()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();

        var customStyle = TextStyle
            .Default
            .FontColor(Colors.Black)
            .BackgroundColor(Colors.Transparent)
            .Underline()
            .DecorationColor(Colors.Red.Medium)
            .DecorationWavy();

        descriptor
            .FontSize(30)
            .FontColor(Colors.Blue.Darken4)
            .BackgroundColor(Colors.Blue.Lighten5)
            .Bold()
            .Style(customStyle);
        
        Assert.That(textBlockSpan.Style.Size, Is.EqualTo(30));
        Assert.That(textBlockSpan.Style.Color, Is.EqualTo(Colors.Black));
        Assert.That(textBlockSpan.Style.BackgroundColor, Is.EqualTo(Colors.Transparent));
        Assert.That(textBlockSpan.Style.HasUnderline, Is.True);
        Assert.That(textBlockSpan.Style.DecorationColor, Is.EqualTo(Colors.Red.Medium));
        Assert.That(textBlockSpan.Style.DecorationStyle, Is.EqualTo(TextStyleConfiguration.TextDecorationStyle.Wavy));
        Assert.That(textBlockSpan.Style.FontWeight, Is.EqualTo(FontWeight.Bold));
    }
    
    [Test]
    public void OverridesStyle_AcceptsNull()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();

        descriptor
            .FontSize(30)
            .FontColor(Colors.Blue.Darken4)
            .BackgroundColor(Colors.Blue.Lighten5)
            .Bold()
            .Style(null);
        
        Assert.That(textBlockSpan.Style.Size, Is.EqualTo(30));
        Assert.That(textBlockSpan.Style.Color, Is.EqualTo(Colors.Blue.Darken4));
        Assert.That(textBlockSpan.Style.BackgroundColor, Is.EqualTo(Colors.Blue.Lighten5));
        Assert.That(textBlockSpan.Style.FontWeight, Is.EqualTo(FontWeight.Bold));
    }
    
    #endregion
    
    #region Font Color
    
    [Test]
    public void SetsCorrectFontColor()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.FontColor(Colors.Blue.Medium);
        Assert.That(textBlockSpan.Style.Color, Is.EqualTo(Colors.Blue.Medium));
    }
    
    [Test]
    public void FontColor_AlsoSetsDecorationColor()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.FontColor(Colors.Green.Darken2);
        Assert.That(textBlockSpan.Style.DecorationColor, Is.EqualTo(Colors.Green.Darken2));
    }
    
    #endregion
    
    #region Background Color
    
    [Test]
    public void SetsCorrectBackgroundColor()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.BackgroundColor(Colors.Yellow.Lighten3);
        Assert.That(textBlockSpan.Style.BackgroundColor, Is.EqualTo(Colors.Yellow.Lighten3));
    }
    
    #endregion
    
    #region Font Family
    
    [Test]
    public void SetsCorrectFontFamily_Single()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.FontFamily("Arial");
        Assert.That(textBlockSpan.Style.FontFamilies, Is.EqualTo(new[] { "Arial" }));
    }
    
    [Test]
    public void SetsCorrectFontFamily_Multiple()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.FontFamily("Helvetica", "Arial", "sans-serif");
        Assert.That(textBlockSpan.Style.FontFamilies, Is.EqualTo(new[] { "Helvetica", "Arial", "sans-serif" }));
    }

    [Test]
    public void FontFamily_EmptyArray_ReturnsUnchanged()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.FontFamily([]);
        Assert.That(textBlockSpan.Style.FontFamilies, Is.Null);
    }
    
    [Test]
    public void FontFamily_Null_ReturnsUnchanged()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.FontFamily(null);
        Assert.That(textBlockSpan.Style.FontFamilies, Is.Null);
    }
    
    #endregion
    
    #region Font Size
    

    [Test]
    public void SetsCorrectFontSize()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.FontSize(18);
        Assert.That(textBlockSpan.Style.Size, Is.EqualTo(18));
    }
    
    [TestCase(-10)]
    [TestCase(-5)]
    [TestCase(-float.Epsilon)]
    [TestCase(0)]
    public void FontSize_MustBePositive(float fontSize)
    {
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            var (descriptor, textBlockSpan) = CreateTextBlockSpan();
            descriptor.FontSize(fontSize);
        });
        
        Assert.That(exception.Message, Is.EqualTo("Font size must be greater than 0."));
    }

    #endregion
    
    #region Line Height

    [Test]
    public void SetsCorrectLineHeight()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.LineHeight(1.5f);
        Assert.That(textBlockSpan.Style.LineHeight, Is.EqualTo(1.5f));
    }
    
    [Test]
    public void LineHeight_Null_SetsToNormalLineHeight()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.LineHeight(null);
        Assert.That(textBlockSpan.Style.LineHeight, Is.EqualTo(TextStyle.NormalLineHeightCalculatedFromFontMetrics));
    }
    
    [TestCase(-5)]
    [TestCase(-float.Epsilon)]
    public void LineHeightMustBePositive(float lineHeight)
    {
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            var (descriptor, textBlockSpan) = CreateTextBlockSpan();
            descriptor.LineHeight(lineHeight);
        });
        
        Assert.That(exception.Message, Is.EqualTo("Line height must be greater than 0."));
    }
    
    [Test]
    public void LineHeight_AllowsZero()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.LineHeight(0);
        Assert.That(textBlockSpan.Style.LineHeight, Is.Zero);
    }
    
    #endregion
    
    #region Letter Spacing
    
    [Test]
    public void SetsCorrectLetterSpacing_Positive()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.LetterSpacing(0.5f);
        Assert.That(textBlockSpan.Style.LetterSpacing, Is.EqualTo(0.5f));
    }
    
    [Test]
    public void SetsCorrectLetterSpacing_Negative()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.LetterSpacing(-0.2f);
        Assert.That(textBlockSpan.Style.LetterSpacing, Is.EqualTo(-0.2f));
    }
    
    [Test]
    public void LetterSpacing_DefaultParameterIsZero()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.LetterSpacing();
        Assert.That(textBlockSpan.Style.LetterSpacing, Is.Zero);
    }
    
    #endregion
    
    #region Word Spacing
    
    [Test]
    public void SetsCorrectWordSpacing_Positive()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.WordSpacing(2.0f);
        Assert.That(textBlockSpan.Style.WordSpacing, Is.EqualTo(2.0f));
    }
    
    [Test]
    public void SetsCorrectWordSpacing_Negative()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.WordSpacing(-1.0f);
        Assert.That(textBlockSpan.Style.WordSpacing, Is.EqualTo(-1.0f));
    }
    
    [Test]
    public void WordSpacing_DefaultParameterIsZero()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.WordSpacing();
        Assert.That(textBlockSpan.Style.WordSpacing, Is.Zero);
    }
    
    #endregion
    
    #region Italic
    
    [Test]
    public void SetsCorrectItalic_Enabled()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.Italic();
        Assert.That(textBlockSpan.Style.IsItalic, Is.True);
    }
    
    [Test]
    public void SetsCorrectItalic_Disabled()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.Italic().Italic(false);
        Assert.That(textBlockSpan.Style.IsItalic, Is.False);
    }
    
    #endregion
    
    #region Text Decoration

    [Test]
    public void SetsCorrectTextDecoration_Strikethrough()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.Strikethrough();
        Assert.That(textBlockSpan.Style.HasStrikethrough, Is.True);
    }
    
    [Test]
    public void SetsCorrectTextDecoration_StrikethroughDisabled()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.Strikethrough(false);
        Assert.That(textBlockSpan.Style.HasStrikethrough, Is.False);
    }
    
    [Test]
    public void SetsCorrectTextDecoration_Underline()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.Underline();
        Assert.That(textBlockSpan.Style.HasUnderline, Is.True);
    }
    
    [Test]
    public void SetsCorrectTextDecoration_UnderlineDisabled()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.Underline(false);
        Assert.That(textBlockSpan.Style.HasUnderline, Is.False);
    }
    
    [Test]
    public void SetsCorrectTextDecoration_Overline()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.Overline();
        Assert.That(textBlockSpan.Style.HasOverline, Is.True);
    }
    
    [Test]
    public void SetsCorrectTextDecoration_OverlineDisabled()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.Overline(false);
        Assert.That(textBlockSpan.Style.HasOverline, Is.False);
    }
    
    [Test]
    public void SetsCorrectTextDecoration_DecorationColor()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.DecorationColor(Colors.Red.Medium);
        Assert.That(textBlockSpan.Style.DecorationColor, Is.EqualTo(Colors.Red.Medium));
    }
    
    [Test]
    public void SetsCorrectTextDecoration_DecorationThickness()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.DecorationThickness(1.5f);
        Assert.That(textBlockSpan.Style.DecorationThickness, Is.EqualTo(1.5f));
    }
    
    [Test]
    public void SetsCorrectTextDecoration_DecorationSolid()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.DecorationSolid();
        Assert.That(textBlockSpan.Style.DecorationStyle, Is.EqualTo(TextStyleConfiguration.TextDecorationStyle.Solid));
    }
    
    [Test]
    public void SetsCorrectTextDecoration_DecorationDouble()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.DecorationDouble();
        Assert.That(textBlockSpan.Style.DecorationStyle, Is.EqualTo(TextStyleConfiguration.TextDecorationStyle.Double));
    }
    
    [Test]
    public void SetsCorrectTextDecoration_DecorationWavy()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.DecorationWavy();
        Assert.That(textBlockSpan.Style.DecorationStyle, Is.EqualTo(TextStyleConfiguration.TextDecorationStyle.Wavy));
    }
    
    [Test]
    public void SetsCorrectTextDecoration_DecorationDotted()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.DecorationDotted();
        Assert.That(textBlockSpan.Style.DecorationStyle, Is.EqualTo(TextStyleConfiguration.TextDecorationStyle.Dotted));
    }
    
    [Test]
    public void SetsCorrectTextDecoration_DecorationDashed()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.DecorationDashed();
        Assert.That(textBlockSpan.Style.DecorationStyle, Is.EqualTo(TextStyleConfiguration.TextDecorationStyle.Dashed));
    }
    
    #endregion
    
    #region Font Weight

    [Test]
    public void SetsCorrectSetsCorrectFontWeight_Thin()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.Thin();
        Assert.That(textBlockSpan.Style.FontWeight, Is.EqualTo(FontWeight.Thin));
    }
    
    [Test]
    public void SetsCorrectFontWeight_ExtraLight()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.ExtraLight();
        Assert.That(textBlockSpan.Style.FontWeight, Is.EqualTo(FontWeight.ExtraLight));
    }
    
    [Test]
    public void SetsCorrectFontWeight_Light()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.Light();
        Assert.That(textBlockSpan.Style.FontWeight, Is.EqualTo(FontWeight.Light));
    }
    
    [Test]
    public void SetsCorrectFontWeight_Normal()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.NormalWeight();
        Assert.That(textBlockSpan.Style.FontWeight, Is.EqualTo(FontWeight.Normal));
    }
    
    [Test]
    public void SetsCorrectFontWeight_Medium()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.Medium();
        Assert.That(textBlockSpan.Style.FontWeight, Is.EqualTo(FontWeight.Medium));
    }
    
    [Test]
    public void SetsCorrectFontWeight_SemiBold()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.SemiBold();
        Assert.That(textBlockSpan.Style.FontWeight, Is.EqualTo(FontWeight.SemiBold));
    }
    
    [Test]
    public void SetsCorrectFontWeight_Bold()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.Bold();
        Assert.That(textBlockSpan.Style.FontWeight, Is.EqualTo(FontWeight.Bold));
    }
    
    [Test]
    public void SetsCorrectFontWeight_ExtraBold()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.ExtraBold();
        Assert.That(textBlockSpan.Style.FontWeight, Is.EqualTo(FontWeight.ExtraBold));
    }
    
    [Test]
    public void SetsCorrectFontWeight_Black()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.Black();
        Assert.That(textBlockSpan.Style.FontWeight, Is.EqualTo(FontWeight.Black));
    }
    
    [Test]
    public void SetsCorrectFontWeight_ExtraBlack()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.ExtraBlack();
        Assert.That(textBlockSpan.Style.FontWeight, Is.EqualTo(FontWeight.ExtraBlack));
    }

    #endregion
    
    #region Text Position
    
    [Test]
    public void SetsCorrectTextPosition_Subscript()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.Subscript();
        Assert.That(textBlockSpan.Style.FontPosition, Is.EqualTo(FontPosition.Subscript));
    }
    
    [Test]
    public void SetsCorrectTextPosition_Normal()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.Superscript().NormalPosition(); // first change from default, then normal
        Assert.That(textBlockSpan.Style.FontPosition, Is.EqualTo(FontPosition.Normal));
    }
    
    [Test]
    public void SetsCorrectTextPosition_Superscript()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.Superscript();
        Assert.That(textBlockSpan.Style.FontPosition, Is.EqualTo(FontPosition.Superscript));
    }
    
    #endregion
    
    #region Text Direction
        
    [Test]
    public void SetsCorrectTextDirection_LeftToRight()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.DirectionFromLeftToRight();
        Assert.That(textBlockSpan.Style.Direction, Is.EqualTo(TextDirection.LeftToRight));
    }
        
    [Test]
    public void SetsCorrectTextDirection_RightToLeft()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.DirectionFromRightToLeft();
        Assert.That(textBlockSpan.Style.Direction, Is.EqualTo(TextDirection.RightToLeft));
    }
        
    [Test]
    public void SetsCorrectTextDirection_Auto()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        descriptor.DirectionFromRightToLeft().DirectionAuto();
        Assert.That(textBlockSpan.Style.Direction, Is.EqualTo(TextDirection.Auto));
    }
        
    #endregion
    
    #region Font Features
    
    [Test]
    public void EnableFontFeature_SingleFeature()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        
        descriptor.EnableFontFeature(FontFeatures.StandardLigatures);
        Assert.That(textBlockSpan.Style.FontFeatures, Has.Length.EqualTo(1));
        Assert.That(textBlockSpan.Style.FontFeatures[0], Is.EqualTo((FontFeatures.StandardLigatures, true)));
    }
    
    [Test]
    public void DisableFontFeature_SingleFeature()
    {
        var (descriptor, textBlockSpan) = CreateTextBlockSpan();
        
        descriptor.DisableFontFeature(FontFeatures.Kerning);
        Assert.That(textBlockSpan.Style.FontFeatures, Has.Length.EqualTo(1));
        Assert.That(textBlockSpan.Style.FontFeatures[0], Is.EqualTo((FontFeatures.Kerning, false)));
    }

    [Test]
    public void FontFeatures_MixedEnableDisable()
    {
        var textStyle = TextStyle.Default
            .EnableFontFeature(FontFeatures.StandardLigatures)
            .DisableFontFeature(FontFeatures.Kerning)
            .DisableFontFeature(FontFeatures.DiscretionaryLigatures)
            .EnableFontFeature(FontFeatures.Kerning);
        
        Assert.That(textStyle.FontFeatures, Has.Length.EqualTo(3));
        Assert.That(textStyle.FontFeatures[0], Is.EqualTo((FontFeatures.Kerning, true)));
        Assert.That(textStyle.FontFeatures[1], Is.EqualTo((FontFeatures.DiscretionaryLigatures, false)));
        Assert.That(textStyle.FontFeatures[2], Is.EqualTo((FontFeatures.StandardLigatures, true)));
    }
    
    #endregion
}