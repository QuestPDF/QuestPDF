using System;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Elements.Text;
using QuestPDF.Elements.Text.Items;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

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
}