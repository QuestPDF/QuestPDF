using QuestPDF.Helpers;

namespace QuestPDF.LayoutTests.TestEngine;

internal static class MockChildren
{
    private static MockChild Create(string id, string color, float width, float height)
    {
        return new MockChild
        {
            Id = id,
            Color = color,
            TotalWidth = width,
            TotalHeight = height
        };
    }

    public static MockChild Red(float width, float height) => Create("red", Colors.Red.Medium, width, height);
    public static MockChild Green(float width, float height) => Create("green", Colors.Green.Medium, width, height);
    public static MockChild Blue(float width, float height) => Create("blue", Colors.Blue.Medium, width, height);
}