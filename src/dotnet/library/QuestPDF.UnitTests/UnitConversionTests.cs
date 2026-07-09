using System.Collections.Generic;
using NUnit.Framework;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests;

[TestFixture]
public class UnitConversionTests
{
    [TestCase(Unit.Point, 1f, 1f)]
    [TestCase(Unit.Inch, 1f, 72f)]
    [TestCase(Unit.Feet, 1f, 864f)]
    [TestCase(Unit.Mil, 1000f, 72f)]
    [TestCase(Unit.Centimetre, 2.54f, 72f)]      // 2.54 cm = 1 inch
    [TestCase(Unit.Millimetre, 25.4f, 72f)]      // 25.4 mm = 1 inch
    [TestCase(Unit.Meter, 0.0254f, 72f)]         // 0.0254 m = 1 inch
    public void ToPoints_ConvertsCorrectly(Unit unit, float input, float expected)
    {
        var result = input.ToPoints(unit);
        Assert.That(result, Is.EqualTo(expected));
        
        var result2 = (input * 2).ToPoints(unit);
        Assert.That(result2, Is.EqualTo(expected * 2));
    }
}