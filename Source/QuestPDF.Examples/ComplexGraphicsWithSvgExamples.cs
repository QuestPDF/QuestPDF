using System;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples;

public class ComplexGraphicsWithSvgExamples
{
    [Test]
    public void ComplexBorder()
    {
        RenderingTest
            .Create()
            .PageSize(300, 200)
            .ProduceImages()
            .ShowResults()
            .Render(container =>
            {
                container
                    .Padding(25)
                    .DashedBorder(content =>
                    {
                        content
                            .AlignCenter()
                            .AlignMiddle()
                            .Text("Text")
                            .FontSize(30);
                    });
            });
    }
}

public static class ComplexBorderExtensions
{
    public static void DashedBorder(this IContainer container, Action<IContainer> content)
    {
        container.Layers(layers =>
        {
            layers.Layer().Svg(size =>
            {
                return $"""
                        <svg width="{size.Width}" height="{size.Height}" xmlns="http://www.w3.org/2000/svg">
                            <rect x="0" y="0" width="{size.Width}" height="{size.Height}" rx="20" ry="20" style="stroke:black; stroke-width:5; stroke-dasharray:5,5,10,5; fill:#AACCEE;" />
                        </svg>
                        """;
            });
                
            layers.PrimaryLayer()
                .AlignCenter()
                .AlignMiddle()
                .Text("Text")
                .FontSize(30);
        });
    }
}