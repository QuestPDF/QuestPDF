using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.VisualTests;

public class RotateTests
{
    [Test]
    public void Rotate(
        [Values(-15, 0, 30, 45, 60, 90)] int angle)
    {
        VisualTest.PerformWithDefaultPageSettings(container =>
        {
            container
                .Width(300)
                .Height(300)
                .AlignCenter()
                .AlignMiddle()

                .Rotate(angle) // <-

                .Width(200)
                .Height(100)
                .Background(Colors.Grey.Lighten3)
                .AlignCenter()
                .AlignMiddle()
                .Text($"Rotation: {angle} deg");
        });
    }
}