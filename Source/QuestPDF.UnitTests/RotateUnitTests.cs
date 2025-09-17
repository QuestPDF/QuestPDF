using NUnit.Framework;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests;

public class RotateUnitTests
{
    [Test]
    public void RotateIsCumulative()
    {
        var container = EmptyContainer.Create();
        
        container
            .Rotate(45)
            .Rotate(-15)
            .Rotate(20);
        
        var rotation = container.Child as Rotate;
        Assert.That(rotation?.Angle, Is.EqualTo(50));
    }
    
    [TestCase(0, ExpectedResult = "No rotation")]
    [TestCase(45, ExpectedResult = "45 deg clockwise")]
    [TestCase(-75, ExpectedResult = "75 deg counter-clockwise")]
    [TestCase(12.345f, ExpectedResult = "12.3 deg clockwise")]
    public string RotateCompanionHint(float angle)
    {
        var container = EmptyContainer.Create();
        container.Rotate(angle);
        
        var rotation = container.Child as Rotate;
        return rotation?.GetCompanionHint();
    }
}