using System;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.Examples;

public class MultiColumnExamples
{
    [Test]
    public void Padding()
    {
        RenderingTest
            .Create()
            .PageSize(PageSizes.A4)
            .ProducePdf()
            .ShowResults()
            .Render(container =>
            {
                container
                    .Padding(25)
                    .MultiColumn()
                    .Column(column =>
                    {
                        column.Spacing(10);

                        foreach (var sectionId in Enumerable.Range(0, 10))
                        {
                            foreach (var textId in Enumerable.Range(0, Random.Shared.Next(5, 10)))
                                column.Item().Text(Placeholders.Paragraph());

                            foreach (var blockId in Enumerable.Range(0, Random.Shared.Next(5, 10)))
                                column.Item().Width(50 + blockId * 10).Height(50).Background(Placeholders.BackgroundColor());
                        }
                    });
            });
    }
}