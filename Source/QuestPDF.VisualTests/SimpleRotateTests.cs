using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.VisualTests;

public class SimpleRotateTests
{
    [Test]
    public void Rotate(
        [Values(0, 1, 2, 3)] int rotationCount)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Width(150)
                .Height(150)
                .Element(element =>
                {
                    foreach (var i in Enumerable.Range(0, rotationCount))
                        element = element.RotateRight();
    
                    return element;
                })
                .Shrink()
                .Background(Colors.Grey.Lighten3)
                .Padding(10)
                .Text($"Rotation #{rotationCount}");
        });
    }
}