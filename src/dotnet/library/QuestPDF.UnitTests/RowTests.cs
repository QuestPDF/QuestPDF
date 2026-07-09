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
    
    [TestCase(float.Epsilon)]
    [TestCase(1)]
    [TestCase(5)]
    public void RelativeItemMustHaveSizeLargerThanZero(float size)
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
    
    [TestCase(0)]
    [TestCase(100)]
    public void ConstantItemMustHaveSizeLargerOrEqualToZero(float size)
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
    [Repeat(10)]
    public void RowSupportsStatefulOperations()
    {
        var container = EmptyContainer.Create();
        
        container.Row(row =>
        {
            foreach (var i in Enumerable.Range(0, 10))
                row.RelativeItem();
        });
        
        var rowContainer = container.Child as Row;
        Assert.That(rowContainer?.Items.Count, Is.EqualTo(10));
        
        rowContainer.ResetState();
        Assert.That(rowContainer.GetState(), Is.EquivalentTo(new bool[10]));
        
        var newState = Enumerable
            .Range(0, 10)
            .Select(x => Random.Shared.Next() % 2 == 0)
            .ToArray();
        
        rowContainer.SetState(newState);
        Assert.That(rowContainer.GetState(), Is.EquivalentTo(newState));
        
        rowContainer.ResetState();
        Assert.That(rowContainer.GetState(), Is.EquivalentTo(new bool[10]));
    }
}