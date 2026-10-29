using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests;

[TestFixture]
public class RowTests
{
    #region Spacing
    
    [TestCase(float.MinValue)]
    [TestCase(-5)]
    [TestCase(-float.Epsilon)]
    public void NegativeSpacingThrowsException(float spacingValue)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            EmptyContainer
                .Create()
                .Row(row =>
                {
                    row.Spacing(spacingValue);
                });
        });
        
        Assert.That(exception.Message, Is.EqualTo("The row spacing cannot be negative. (Parameter 'spacing')"));
    }
    
    [TestCase(float.NaN)]
    [TestCase(float.PositiveInfinity)]
    [TestCase(float.NegativeInfinity)]
    public void SpacingMustBeFiniteNumber(float spacingValue)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            EmptyContainer
                .Create()
                .Row(row =>
                {
                    row.Spacing(spacingValue);
                });
        });
        
        Assert.That(exception.Message, Is.EqualTo("The row spacing must be a finite number. (Parameter 'spacing')"));
    }
    
    [TestCase(0)]
    [TestCase(float.Epsilon)]
    [TestCase(10)]
    public void ValidSpacingIsCorrectlyApplied(float spacingValue)
    {
        var container = EmptyContainer.Create();
        
        container.Row(row =>
        {
            row.Spacing(spacingValue);
        });
        
        var rowContainer = container.Child as Row;
        Assert.That(rowContainer?.Spacing, Is.EqualTo(spacingValue));
    }
    
    [Test]
    public void SpacingSupportsUnitConversion()
    {
        var container = EmptyContainer.Create();
        
        container.Row(row =>
        {
            row.Spacing(5, Unit.Inch);
        });
        
        var rowContainer = container.Child as Row;
        Assert.That(rowContainer?.Spacing, Is.EqualTo(360));
    }
    
    #endregion
    
    #region Relative Item
    
    [TestCase(-10)]
    [TestCase(-float.Epsilon)]
    [TestCase(0)]
    public void RelativeItemCannotHaveSizeSmallerOrEqualToZero(float size)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            EmptyContainer
                .Create()
                .Row(row =>
                {
                    row.RelativeItem(size);
                });
        });

        Assert.That(exception?.Message, Is.EqualTo("The relative item size must be greater than zero. (Parameter 'size')"));
    }
    
    [TestCase(float.NaN)]
    [TestCase(float.PositiveInfinity)]
    public void RelativeItemSizeMustBeFiniteNumber(float size)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            EmptyContainer
                .Create()
                .Row(row =>
                {
                    row.RelativeItem(size);
                });
        });

        Assert.That(exception?.Message, Is.EqualTo("The relative item size must be a finite number. (Parameter 'size')"));
    }
    
    [TestCase(float.Epsilon)]
    [TestCase(1)]
    [TestCase(5)]
    public void RelativeItemWithPositiveSizeIsAdded(float size)
    {
        var container = EmptyContainer.Create();
        
        container.Row(row =>
        {
            row.RelativeItem(size);
        });
        
        var rowContainer = container.Child as Row;
        Assert.That(rowContainer?.Items.Count, Is.EqualTo(1));
        
        var firstItem = rowContainer?.Items.Single();
        Assert.That(firstItem.Type, Is.EqualTo(RowItemType.Relative));
        Assert.That(firstItem.Size, Is.EqualTo(size));
    }
    
    #endregion
    
    #region Constant Item
    
    [TestCase(-10)]
    [TestCase(-float.Epsilon)]
    public void ConstantItemCannotHaveSizeSmallerThanZero(float size)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            EmptyContainer
                .Create()
                .Row(row =>
                {
                    row.ConstantItem(size);
                });
        });

        Assert.That(exception?.Message, Is.EqualTo("The constant item size cannot be negative. (Parameter 'size')"));
    }
    
    [TestCase(float.NaN)]
    [TestCase(float.PositiveInfinity)]
    public void ConstantItemSizeMustBeFiniteNumber(float size)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            EmptyContainer
                .Create()
                .Row(row =>
                {
                    row.ConstantItem(size);
                });
        });

        Assert.That(exception?.Message, Is.EqualTo("The constant item size must be a finite number. (Parameter 'size')"));
    }
    
    [TestCase(0)]
    [TestCase(100)]
    public void ConstantItemWithNonNegativeSizeIsAdded(float size)
    {
        var container = EmptyContainer.Create();
        
        container.Row(row =>
        {
            row.ConstantItem(size);
        });
        
        var rowContainer = container.Child as Row;
        Assert.That(rowContainer?.Items.Count, Is.EqualTo(1));
        
        var firstItem = rowContainer?.Items.Single();
        Assert.That(firstItem.Type, Is.EqualTo(RowItemType.Constant));
        Assert.That(firstItem.Size, Is.EqualTo(size));
    }
    
    [Test]
    public void ConstantItemSupportsUnitConversion()
    {
        var container = EmptyContainer.Create();
        
        container.Row(row =>
        {
            row.ConstantItem(2, Unit.Inch);
        });
        
        var rowContainer = container.Child as Row;
        Assert.That(rowContainer?.Items.Count, Is.EqualTo(1));
        
        var firstItem = rowContainer?.Items.Single();
        Assert.That(firstItem.Type, Is.EqualTo(RowItemType.Constant));
        Assert.That(firstItem.Size, Is.EqualTo(144));
    }
    
    #endregion
    
    [Test]
    public void CompanionHints()
    {
        var container = EmptyContainer.Create();
        
        container.Row(row =>
        {
            row.RelativeItem(3);
            row.ConstantItem(2, Unit.Inch);
            row.AutoItem();
        });
        
        var rowContainer = container.Child as Row;
        Assert.That(rowContainer?.Items.Count, Is.EqualTo(3));
        
        var items = rowContainer?.Items;
        
        Assert.That(items[0].GetCompanionHint(), Is.EqualTo("Relative 3"));
        Assert.That(items[1].GetCompanionHint(), Is.EqualTo("Constant 144"));
        Assert.That(items[2].GetCompanionHint(), Is.EqualTo("Auto"));
    }
    
    [Test]
    public void RowSupportsStatefulOperations()
    {
        var container = EmptyContainer.Create();
        
        container.Row(row =>
        {
            foreach (var i in Enumerable.Range(0, 4))
                row.RelativeItem();
        });
        
        var rowContainer = container.Child as Row;
        Assert.That(rowContainer?.Items.Count, Is.EqualTo(4));
        
        rowContainer.ResetState();
        Assert.That(rowContainer.GetState(), Is.EqualTo(new[] { false, false, false, false }));
        
        // the state lists which items are fully rendered, in the order of items
        var newState = new[] { true, false, false, true };
        
        rowContainer.SetState(newState);
        Assert.That(rowContainer.GetState(), Is.EqualTo(newState));
        Assert.That(rowContainer.Items.Select(x => x.IsRendered), Is.EqualTo(newState));
        
        rowContainer.ResetState();
        Assert.That(rowContainer.GetState(), Is.EqualTo(new[] { false, false, false, false }));
    }
    
    [Test]
    public void ResetStateMeasuresAutoItemsAgain()
    {
        var container = EmptyContainer.Create();
        
        container.Row(row =>
        {
            row.AutoItem();
            row.ConstantItem(20);
        });
        
        var rowContainer = container.Child as Row;
        
        // the width of an auto item is measured once and kept, the reset clears it,
        // e.g. so that text showing the total page count is measured again once the count is known
        rowContainer.Items[0].Size = 50;
        rowContainer.ResetState();
        
        Assert.That(rowContainer.Items.Select(x => x.Size), Is.EqualTo(new[] { 0f, 20f }));
    }
}