using System;
using NUnit.Framework;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests;

public class StyledBoxTests
{
    [Test]
    public void BorderShorthandSetsCorrectValues()
    {
        var container = EmptyContainer.Create();

        container.Border(1.23f, Colors.Amber.Darken2);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderLeft, Is.EqualTo(1.23f));
        Assert.That(styledBox?.BorderRight, Is.EqualTo(1.23f));
        Assert.That(styledBox?.BorderTop, Is.EqualTo(1.23f));
        Assert.That(styledBox?.BorderBottom, Is.EqualTo(1.23f));
        
        Assert.That(styledBox?.BorderColor, Is.EqualTo(Colors.Amber.Darken2));
        Assert.That(styledBox?.BackgroundColor, Is.EqualTo(Colors.Transparent));
        
        Assert.That(styledBox?.BorderRadiusTopLeft, Is.Zero);
        Assert.That(styledBox?.BorderRadiusTopRight, Is.Zero);
        Assert.That(styledBox?.BorderRadiusBottomLeft, Is.Zero);
        Assert.That(styledBox?.BorderRadiusBottomRight, Is.Zero);
    }
    
    #region Background

    [Test]
    public void BackgroundColorSetsCorrectValue()
    {
        var container = EmptyContainer.Create();

        container.Background(Colors.Green.Medium);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BackgroundColor, Is.EqualTo(Colors.Green.Medium));
    }
    
    [Test]
    public void BackgroundLinearGradientCannotBeEmpty()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            EmptyContainer.Create().BackgroundLinearGradient(123f, []);
        });
        
        Assert.That(exception.Message, Does.Contain("The background linear-gradient colors cannot be empty. (Parameter 'colors')"));
    }
    
    [Test]
    public void BackgroundLinearGradientSetsCorrectValue()
    {
        var container = EmptyContainer.Create();

        container.BackgroundLinearGradient(30f, [ Colors.Red.Lighten3, Colors.Orange.Lighten3, Colors.Yellow.Lighten3 ]);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BackgroundGradientAngle, Is.EqualTo(30));
        Assert.That(styledBox?.BackgroundGradientColors, Is.EqualTo([ Colors.Red.Lighten3, Colors.Orange.Lighten3, Colors.Yellow.Lighten3 ]));
    }

    #endregion
    
    #region Thickness
    
    [Test] 
    public void BorderAllSetsCorrectValue()
    {
        var container = EmptyContainer.Create();
        
        container.Border(1.23f);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderLeft, Is.EqualTo(1.23f));
        Assert.That(styledBox?.BorderRight, Is.EqualTo(1.23f));
        Assert.That(styledBox?.BorderTop, Is.EqualTo(1.23f));
        Assert.That(styledBox?.BorderBottom, Is.EqualTo(1.23f));
    }
    
    [Test] 
    public void BorderAllSupportsUnitConversion()
    {
        var container = EmptyContainer.Create();
        
        container.Border(0.25f, Unit.Inch);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderLeft, Is.EqualTo(18));
        Assert.That(styledBox?.BorderRight, Is.EqualTo(18));
        Assert.That(styledBox?.BorderTop, Is.EqualTo(18));
        Assert.That(styledBox?.BorderBottom, Is.EqualTo(18));
    }
    
    [Test] 
    public void BorderVerticalSetsCorrectValue()
    {
        var container = EmptyContainer.Create();
        
        container.BorderVertical(20);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderLeft, Is.EqualTo(20));
        Assert.That(styledBox?.BorderRight, Is.EqualTo(20));
        Assert.That(styledBox?.BorderTop, Is.Zero);
        Assert.That(styledBox?.BorderBottom, Is.Zero);
    }
    
    [Test] 
    public void BorderVerticalSupportsUnitConversion()
    {
        var container = EmptyContainer.Create();
        
        container.BorderVertical(0.5f, Unit.Inch);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderLeft, Is.EqualTo(36));
        Assert.That(styledBox?.BorderRight, Is.EqualTo(36));
        Assert.That(styledBox?.BorderTop, Is.Zero);
        Assert.That(styledBox?.BorderBottom, Is.Zero);
    }
    
    [Test] 
    public void BorderHorizontalSetsCorrectValue()
    {
        var container = EmptyContainer.Create();
        
        container.BorderHorizontal(25);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderLeft, Is.Zero);
        Assert.That(styledBox?.BorderRight, Is.Zero);
        Assert.That(styledBox?.BorderTop, Is.EqualTo(25));
        Assert.That(styledBox?.BorderBottom, Is.EqualTo(25));
    }
    
    [Test] 
    public void BorderHorizontalSupportsUnitConversion()
    {
        var container = EmptyContainer.Create();
        
        container.BorderHorizontal(0.75f, Unit.Inch);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderLeft, Is.Zero);
        Assert.That(styledBox?.BorderRight, Is.Zero);
        Assert.That(styledBox?.BorderTop, Is.EqualTo(54));
        Assert.That(styledBox?.BorderBottom, Is.EqualTo(54));
    }
    
    [Test]
    public void BorderLeftSetsCorrectValue()
    {
        var container = EmptyContainer.Create();
        
        container.BorderLeft(5);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderLeft, Is.EqualTo(5));
        Assert.That(styledBox?.BorderRight, Is.Zero);
        Assert.That(styledBox?.BorderTop, Is.Zero);
        Assert.That(styledBox?.BorderBottom, Is.Zero);
    }
    
    [Test]
    public void BorderLeftSupportsUnitConversion()
    {
        var container = EmptyContainer.Create();
        
        container.BorderLeft(0.25f, Unit.Inch);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderLeft, Is.EqualTo(18));
        Assert.That(styledBox?.BorderRight, Is.Zero);
        Assert.That(styledBox?.BorderTop, Is.Zero);
        Assert.That(styledBox?.BorderBottom, Is.Zero);
    }
    
    [TestCase(-5)]
    [TestCase(-Size.Epsilon)]
    public void BorderLeftCannotBeNegative(float border)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            EmptyContainer.Create().BorderLeft(border);
        });
        
        Assert.That(exception.Message, Does.Contain("The left border cannot be negative."));
    }
    
    [Test]
    public void BorderRightSetsCorrectValue()
    {
        var container = EmptyContainer.Create();
        
        container.BorderRight(10);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderLeft, Is.Zero);
        Assert.That(styledBox?.BorderRight, Is.EqualTo(10));
        Assert.That(styledBox?.BorderTop, Is.Zero);
        Assert.That(styledBox?.BorderBottom, Is.Zero);
    }
    
    [Test]
    public void BorderRightSupportsUnitConversion()
    {
        var container = EmptyContainer.Create();
        
        container.BorderRight(0.5f, Unit.Inch);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderLeft, Is.Zero);
        Assert.That(styledBox?.BorderRight, Is.EqualTo(36));
        Assert.That(styledBox?.BorderTop, Is.Zero);
        Assert.That(styledBox?.BorderBottom, Is.Zero);
    }
    
    [TestCase(-5)]
    [TestCase(-Size.Epsilon)]
    public void BorderRightCannotBeNegative(float border)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            EmptyContainer.Create().BorderRight(border);
        });
        
        Assert.That(exception.Message, Does.Contain("The right border cannot be negative."));
    }
    
    [Test]
    public void BorderTopSetsCorrectValue()
    {
        var container = EmptyContainer.Create();
        
        container.BorderTop(15);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderLeft, Is.Zero);
        Assert.That(styledBox?.BorderRight, Is.Zero);
        Assert.That(styledBox?.BorderTop, Is.EqualTo(15));
        Assert.That(styledBox?.BorderBottom, Is.Zero);
    }
    
    [Test]
    public void BorderTopSupportsUnitConversion()
    {
        var container = EmptyContainer.Create();
        
        container.BorderTop(0.75f, Unit.Inch);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderLeft, Is.Zero);
        Assert.That(styledBox?.BorderRight, Is.Zero);
        Assert.That(styledBox?.BorderTop, Is.EqualTo(54f));
        Assert.That(styledBox?.BorderBottom, Is.Zero);
    }
    
    [TestCase(-5)]
    [TestCase(-Size.Epsilon)]
    public void BorderTopCannotBeNegative(float border)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            EmptyContainer.Create().BorderTop(border);
        });
        
        Assert.That(exception.Message, Does.Contain("The top border cannot be negative."));
    }
    
    [Test]
    public void BorderBottomSetsCorrectValue()
    {
        var container = EmptyContainer.Create();
        
        container.BorderBottom(20);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderLeft, Is.Zero);
        Assert.That(styledBox?.BorderRight, Is.Zero);
        Assert.That(styledBox?.BorderTop, Is.Zero);
        Assert.That(styledBox?.BorderBottom, Is.EqualTo(20));
    }
    
    [Test]
    public void BorderBottomSupportsUnitConversion()
    {
        var container = EmptyContainer.Create();
        
        container.BorderBottom(1f, Unit.Inch);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderLeft, Is.Zero);
        Assert.That(styledBox?.BorderRight, Is.Zero);
        Assert.That(styledBox?.BorderTop, Is.Zero);
        Assert.That(styledBox?.BorderBottom, Is.EqualTo(72f));
    }
    
    [TestCase(-5)]
    [TestCase(-Size.Epsilon)]
    public void BorderBottomCannotBeNegative(float border)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            EmptyContainer.Create().BorderBottom(border);
        });
        
        Assert.That(exception.Message, Does.Contain("The bottom border cannot be negative."));
    }
    
    [Test]
    public void ZeroBorderIsSupported()
    {
        var container = EmptyContainer.Create();

        container
            .BorderLeft(0)
            .BorderRight(0)
            .BorderTop(0)
            .BorderBottom(0);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderLeft, Is.Zero);
        Assert.That(styledBox?.BorderRight, Is.Zero);
        Assert.That(styledBox?.BorderTop, Is.Zero);
        Assert.That(styledBox?.BorderBottom, Is.Zero);
    }
    
    [Test]
    public void BorderValuesAreOverridingEachOther()
    {
        var container = EmptyContainer.Create();

        container
            .Border(5)
            .BorderVertical(10)
            .BorderLeft(15)
            .BorderBottom(20);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderLeft, Is.EqualTo(15));
        Assert.That(styledBox?.BorderRight, Is.EqualTo(10));
        Assert.That(styledBox?.BorderTop, Is.EqualTo(5));
        Assert.That(styledBox?.BorderBottom, Is.EqualTo(20));
    }
    
    [Test]
    public void BorderSetsColorToBlack()
    {
        var container = EmptyContainer.Create();

        container.Border(5);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderColor, Is.EqualTo(Colors.Black));
    }
    
    #endregion

    #region Corner Radius

    [Test] 
    public void CornerRadiusAllSetsCorrectValue()
    {
        var container = EmptyContainer.Create();
        
        container.CornerRadius(8);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderRadiusTopLeft, Is.EqualTo(8));
        Assert.That(styledBox?.BorderRadiusTopRight, Is.EqualTo(8));
        Assert.That(styledBox?.BorderRadiusBottomLeft, Is.EqualTo(8));
        Assert.That(styledBox?.BorderRadiusBottomRight, Is.EqualTo(8));
    }
    
    [Test] 
    public void CornerRadiusAllSupportsUnitConversion()
    {
        var container = EmptyContainer.Create();
        
        container.CornerRadius(1.5f, Unit.Inch);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderRadiusTopLeft, Is.EqualTo(108));
        Assert.That(styledBox?.BorderRadiusTopRight, Is.EqualTo(108));
        Assert.That(styledBox?.BorderRadiusBottomLeft, Is.EqualTo(108));
        Assert.That(styledBox?.BorderRadiusBottomRight, Is.EqualTo(108));
    }

    [TestCase(-5)]
    [TestCase(-Size.Epsilon)]
    public void CornerRadiusAllCannotBeNegative(float cornerRadius)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            EmptyContainer.Create().CornerRadius(cornerRadius);
        });
        
        Assert.That(exception.Message, Does.Contain("The top-left corner radius cannot be negative."));
    }
    
    [Test] 
    public void CornerRadiusTopLeftSetsCorrectValue()
    {
        var container = EmptyContainer.Create();
        
        container.CornerRadiusTopLeft(8);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderRadiusTopLeft, Is.EqualTo(8));
        Assert.That(styledBox?.BorderRadiusTopRight, Is.Zero);
        Assert.That(styledBox?.BorderRadiusBottomLeft, Is.Zero);
        Assert.That(styledBox?.BorderRadiusBottomRight, Is.Zero);
    }
    
    [Test] 
    public void CornerRadiusTopLeftSupportsUnitConversion()
    {
        var container = EmptyContainer.Create();
        
        container.CornerRadiusTopLeft(0.25f, Unit.Inch);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderRadiusTopLeft, Is.EqualTo(18));
        Assert.That(styledBox?.BorderRadiusTopRight, Is.Zero);
        Assert.That(styledBox?.BorderRadiusBottomLeft, Is.Zero);
        Assert.That(styledBox?.BorderRadiusBottomRight, Is.Zero);
    }

    [TestCase(-5)]
    [TestCase(-Size.Epsilon)]
    public void CornerRadiusTopLeftCannotBeNegative(float cornerRadius)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            EmptyContainer.Create().CornerRadiusTopLeft(cornerRadius);
        });
        
        Assert.That(exception.Message, Does.Contain("The top-left corner radius cannot be negative."));
    }
    
    [Test] 
    public void CornerRadiusTopRightSetsCorrectValue()
    {
        var container = EmptyContainer.Create();
        
        container.CornerRadiusTopRight(10);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderRadiusTopLeft, Is.Zero);
        Assert.That(styledBox?.BorderRadiusTopRight, Is.EqualTo(10));
        Assert.That(styledBox?.BorderRadiusBottomLeft, Is.Zero);
        Assert.That(styledBox?.BorderRadiusBottomRight, Is.Zero);
    }
    
    [Test] 
    public void CornerRadiusTopRightSupportsUnitConversion()
    {
        var container = EmptyContainer.Create();
        
        container.CornerRadiusTopRight(0.5f, Unit.Inch);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderRadiusTopLeft, Is.Zero);
        Assert.That(styledBox?.BorderRadiusTopRight, Is.EqualTo(36));
        Assert.That(styledBox?.BorderRadiusBottomLeft, Is.Zero);
        Assert.That(styledBox?.BorderRadiusBottomRight, Is.Zero);
    }

    [TestCase(-5)]
    [TestCase(-Size.Epsilon)]
    public void CornerRadiusTopRightCannotBeNegative(float cornerRadius)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            EmptyContainer.Create().CornerRadiusTopRight(cornerRadius);
        });
        
        Assert.That(exception.Message, Does.Contain("The top-right corner radius cannot be negative."));
    }
    
    [Test] 
    public void CornerRadiusBottomLeftSetsCorrectValue()
    {
        var container = EmptyContainer.Create();
        
        container.CornerRadiusBottomLeft(12);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderRadiusTopLeft, Is.Zero);
        Assert.That(styledBox?.BorderRadiusTopRight, Is.Zero);
        Assert.That(styledBox?.BorderRadiusBottomLeft, Is.EqualTo(12));
        Assert.That(styledBox?.BorderRadiusBottomRight, Is.Zero);
    }
    
    [Test] 
    public void CornerRadiusBottomLeftSupportsUnitConversion()
    {
        var container = EmptyContainer.Create();
        
        container.CornerRadiusBottomLeft(0.75f, Unit.Inch);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderRadiusTopLeft, Is.Zero);
        Assert.That(styledBox?.BorderRadiusTopRight, Is.Zero);
        Assert.That(styledBox?.BorderRadiusBottomLeft, Is.EqualTo(54));
        Assert.That(styledBox?.BorderRadiusBottomRight, Is.Zero);
    }

    [TestCase(-5)]
    [TestCase(-Size.Epsilon)]
    public void CornerRadiusBottomLeftCannotBeNegative(float cornerRadius)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            EmptyContainer.Create().CornerRadiusBottomLeft(cornerRadius);
        });
        
        Assert.That(exception.Message, Does.Contain("The bottom-left corner radius cannot be negative."));
    }
    
    [Test] 
    public void CornerRadiusBottomRightSetsCorrectValue()
    {
        var container = EmptyContainer.Create();
        
        container.CornerRadiusBottomRight(14);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderRadiusTopLeft, Is.Zero);
        Assert.That(styledBox?.BorderRadiusTopRight, Is.Zero);
        Assert.That(styledBox?.BorderRadiusBottomLeft, Is.Zero);
        Assert.That(styledBox?.BorderRadiusBottomRight, Is.EqualTo(14));
    }
    
    [Test] 
    public void CornerRadiusBottomRightSupportsUnitConversion()
    {
        var container = EmptyContainer.Create();
        
        container.CornerRadiusBottomRight(1f, Unit.Inch);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderRadiusTopLeft, Is.Zero);
        Assert.That(styledBox?.BorderRadiusTopRight, Is.Zero);
        Assert.That(styledBox?.BorderRadiusBottomLeft, Is.Zero);
        Assert.That(styledBox?.BorderRadiusBottomRight, Is.EqualTo(72));
    }

    [TestCase(-5)]
    [TestCase(-Size.Epsilon)]
    public void CornerRadiusBottomRightCannotBeNegative(float cornerRadius)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            EmptyContainer.Create().CornerRadiusBottomRight(cornerRadius);
        });
        
        Assert.That(exception.Message, Does.Contain("The bottom-right corner radius cannot be negative."));
    }
    
    [Test]
    public void CornerRadiusValuesAreOverridingEachOther()
    {
        var container = EmptyContainer.Create();

        container
            .CornerRadius(4)
            .CornerRadiusTopLeft(6)
            .CornerRadiusTopRight(8)
            .CornerRadiusBottomLeft(10)
            .CornerRadiusBottomRight(12)
            .CornerRadiusBottomRight(14);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderRadiusTopLeft, Is.EqualTo(6));
        Assert.That(styledBox?.BorderRadiusTopRight, Is.EqualTo(8));
        Assert.That(styledBox?.BorderRadiusBottomLeft, Is.EqualTo(10));
        Assert.That(styledBox?.BorderRadiusBottomRight, Is.EqualTo(14));
    }
    
    #endregion

    #region Border Style
    
    [Test]
    public void BorderColorSetsCorrectValue()
    {
        var container = EmptyContainer.Create();

        container.BorderColor(Colors.Red.Darken3);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderColor, Is.EqualTo(Colors.Red.Darken3));
    }
    
    [Test]
    public void BorderLinearGradientCannotBeEmpty()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            EmptyContainer.Create().BorderLinearGradient(234f, []);
        });
        
        Assert.That(exception.Message, Does.Contain("The border linear-gradient colors cannot be empty. (Parameter 'colors')"));
    }
    
    [Test]
    public void BorderLinearGradientSetsCorrectValue()
    {
        var container = EmptyContainer.Create();

        container.BorderLinearGradient(123f, [ Colors.Red.Darken3, Colors.Orange.Darken3, Colors.Yellow.Darken3 ]);
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderGradientAngle, Is.EqualTo(123));
        Assert.That(styledBox?.BorderGradientColors, Is.EqualTo([ Colors.Red.Darken3, Colors.Orange.Darken3, Colors.Yellow.Darken3 ]));
    }
    
    #endregion
    
    #region Border Alignment
    
    [Test]
    public void BorderAlignmentIsMiddleWhenNoRoundedCorners()
    {
        var container = EmptyContainer.Create();
        container.Border(5);
        
        var styledBox = container.Child as StyledBox;
        styledBox.AdjustBorderAlignment();
        Assert.That(styledBox?.BorderAlignment, Is.EqualTo(0.5f));
    }
    
    [Test]
    public void BorderAlignmentIsInsideWhenHasRoundedCorners()
    {
        var container = EmptyContainer.Create();
        container.Border(5).CornerRadius(10);
        
        var styledBox = container.Child as StyledBox;
        styledBox.AdjustBorderAlignment();
        Assert.That(styledBox?.BorderAlignment, Is.EqualTo(0f));
    }
    
    [Test]
    public void BorderAlignmentInsideSetsCorrectValue()
    {
        var container = EmptyContainer.Create();
        container.BorderAlignmentInside();
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderAlignment, Is.EqualTo(0));
    }
    
    [Test]
    public void BorderAlignmentMiddleSetsCorrectValue()
    {
        var container = EmptyContainer.Create();
        container.BorderAlignmentMiddle();
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderAlignment, Is.EqualTo(0.5f));
    }
    
    [Test]
    public void BorderAlignmentOutsideSetsCorrectValue()
    {
        var container = EmptyContainer.Create();
        container.BorderAlignmentOutside();
        
        var styledBox = container.Child as StyledBox;
        Assert.That(styledBox?.BorderAlignment, Is.EqualTo(1f));
    }
    
    #endregion
    
    #region Shadow
    
    [Test]
    public void ShadowStyleSetsCorrectValue()
    {
        var container = EmptyContainer.Create();
        
        container.Shadow(new BoxShadowStyle
        {
            Color = Colors.Black.WithAlpha(0.5f),
            OffsetX = 5,
            OffsetY = 10,
            Blur = 15,
            Spread = 20
        });
        
        var styledBox = container.Child as StyledBox;
        var shadow = styledBox?.Shadow;
        
        Assert.That(shadow.Color, Is.EqualTo(new Color(0x7F000000)));
        Assert.That(shadow.OffsetX, Is.EqualTo(5));
        Assert.That(shadow.OffsetY, Is.EqualTo(10));
        Assert.That(shadow.Blur, Is.EqualTo(15));
        Assert.That(shadow.Spread, Is.EqualTo(20));
    }
    
    [Test]
    public void ShadowStyleCannotBeNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
        {
            EmptyContainer.Create().Shadow(null);
        });
        
        Assert.That(exception.Message, Does.Contain("The box shadow style cannot be null."));
    }
    
    [TestCase(-10)]
    [TestCase(-Size.Epsilon)]
    public void ShadowBlurCannotBeNegative(float blur)
    {
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            EmptyContainer.Create().Shadow(new BoxShadowStyle { Blur = blur });
        });
        
        Assert.That(exception.Message, Does.Contain("Shadow blur radius cannot be negative."));
    }
    
    #endregion
}