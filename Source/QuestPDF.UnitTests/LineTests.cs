using System;
using NUnit.Framework;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests;

public class LineTests
{
    #region Line Types
    
    [Test]
    public void VerticalLineTypeIsSetCorrectly()
    {
        var container = EmptyContainer.Create();
        container.LineVertical(2);
        
        var line = container.Child as Line;
        Assert.That(line?.Type, Is.EqualTo(LineType.Vertical));
        Assert.That(line?.Thickness, Is.EqualTo(2));
    }
    
    [Test]
    public void HorizontalLineTypeIsSetCorrectly()
    {
        var container = EmptyContainer.Create();
        container.LineHorizontal(3);
        
        var line = container.Child as Line;
        Assert.That(line?.Type, Is.EqualTo(LineType.Horizontal));
        Assert.That(line?.Thickness, Is.EqualTo(3));
    }
    
    #endregion
    
    #region Line Thickness
    
    [Test]
    public void VerticalLineThicknessSupportsUnitConversion()
    {
        var container = EmptyContainer.Create();
        container.LineVertical(2, Unit.Inch);
        
        var line = container.Child as Line;
        Assert.That(line?.Thickness, Is.EqualTo(144));
    }
    
    [TestCase(-5f)]
    [TestCase(-float.Epsilon)]
    public void LineThicknessCannotBeNegative(float thickness)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            EmptyContainer
                .Create()
                .LineVertical(thickness);
        });
        
        Assert.That(exception.Message, Is.EqualTo("The Line thickness cannot be negative. (Parameter 'thickness')"));
    }
    
    [Test]
    public void LineThicknessCanBeEqualToZero()
    {
        // thickness 0 corresponds to a hairline
        
        var container = EmptyContainer.Create();
        container.LineHorizontal(0);
        
        var line = container.Child as Line;
        Assert.That(line?.Thickness, Is.Zero);
    }
    
    [Test]
    public void HorizontalLineThicknessSupportsUnitConversion()
    {
        var container = EmptyContainer.Create();
        container.LineHorizontal(3, Unit.Inch);
        
        var line = container.Child as Line;
        Assert.That(line?.Thickness, Is.EqualTo(216));
    }
    
    #endregion
    
    [Test]
    public void LineColorIsSetCorrectly()
    {
        var container = EmptyContainer.Create();
        container.LineHorizontal(1).LineColor(Colors.Red.Medium);
        
        var line = container.Child as Line;
        Assert.That(line?.Color, Is.EqualTo(Colors.Red.Medium));
    }
    
    #region Line Dash Pattern
    
    [Test]
    public void LineDashPatternCannotBeNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
        {
            EmptyContainer
                .Create()
                .LineVertical(1)
                .LineDashPattern(null);
        });
        
        Assert.That(exception.Message, Is.EqualTo("The dash pattern cannot be null. (Parameter 'dashPattern')"));
    }
    
    [Test]
    public void LineDashPatternCannotBeEmpty()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            EmptyContainer
                .Create()
                .LineVertical(1)
                .LineDashPattern([]);
        });
        
        Assert.That(exception.Message, Is.EqualTo("The dash pattern cannot be empty. (Parameter 'dashPattern')"));
    }
    
    [Test]
    public void LineDashPatternMustHaveEvenNumberOfElements()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            EmptyContainer
                .Create()
                .LineVertical(1)
                .LineDashPattern([ 1, 2, 3 ]);
        });
        
        Assert.That(exception.Message, Is.EqualTo("The dash pattern must contain an even number of elements. (Parameter 'dashPattern')"));
    }
    
    [Test]
    public void LineDashPatternIsSetCorrectly()
    {
        var container = EmptyContainer.Create();

        container
            .LineVertical(1)
            .LineDashPattern([1, 2, 3, 4]);
        
        var line = container.Child as Line;
        Assert.That(line?.DashPattern, Is.EquivalentTo([ 1, 2, 3, 4 ]));
    }
    
    [Test]
    public void LineDashPatternSupportsUnitConversion()
    {
        var container = EmptyContainer.Create();

        container
            .LineVertical(1)
            .LineDashPattern([1, 2, 3, 4], Unit.Inch);
        
        var line = container.Child as Line;
        Assert.That(line?.DashPattern, Is.EquivalentTo([ 72, 144, 216, 288 ]));
    }
    
    #endregion
    
    #region Gradient Colors
    
    [Test]
    public void LineGradientColorsCannotBeBull()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
        {
            EmptyContainer
                .Create()
                .LineVertical(1)
                .LineGradient(null);
        });
        
        Assert.That(exception.Message, Is.EqualTo("The gradient colors cannot be null. (Parameter 'colors')"));
    }
    
    [Test]
    public void LineGradientColorsCannotBeEmpty()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            EmptyContainer
                .Create()
                .LineVertical(1)
                .LineGradient([]);
        });
        
        Assert.That(exception.Message, Is.EqualTo("The gradient colors cannot be empty. (Parameter 'colors')"));
    }
    
    [Test]
    public void LineGradientColorsAreSetCorrectly()
    {
        var container = EmptyContainer.Create();
            
        container
            .LineVertical(1)
            .LineGradient([Colors.Red.Medium, Colors.Green.Medium, Colors.Blue.Medium]);
        
        var line = container.Child as Line;
        Assert.That(line?.GradientColors, Is.EquivalentTo([ Colors.Red.Medium, Colors.Green.Medium, Colors.Blue.Medium ]));
    }
    
    #endregion
    
    #region Companion Hint
    
    [Test]
    public void VerticalLineCompanionHint()
    {
        var container = EmptyContainer.Create();
        container.LineVertical(123.45f);
        
        var line = container.Child as Line;
        Assert.That(line?.GetCompanionHint(), Is.EqualTo("Vertical 123.5"));
    }
    
    [Test]
    public void HorizontalLineCompanionHint()
    {
        var container = EmptyContainer.Create();
        container.LineHorizontal(234.56f);
        
        var line = container.Child as Line;
        Assert.That(line?.GetCompanionHint(), Is.EqualTo("Horizontal 234.6"));
    }
    
    #endregion
    
    [Test]
    [Repeat(10)]
    public void LineSupportsStatefulOperations()
    {
        var container = EmptyContainer.Create();

        container.LineHorizontal(1);
        
        var line = container.Child as Line;
        
        Assert.That(line.GetState(), Is.False);
        line.SetState(true);
        Assert.That(line.GetState(), Is.True);
        
        line.ResetState();
        Assert.That(line.GetState(), Is.False);
    }
}