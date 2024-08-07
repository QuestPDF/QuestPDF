using System;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples;

public class MultiColumnExamples
{
    [Test]
    public void TypicalCase()
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
                    .DefaultTextStyle(x => x.FontSize(8))
                    .MultiColumn(multiColumn =>
                    {
                        multiColumn.Columns(3);
                        
                        multiColumn
                            .Content()
                            .Column(column =>
                            {
                                column.Spacing(10);

                                foreach (var sectionId in Enumerable.Range(0, 10))
                                {
                                    foreach (var textId in Enumerable.Range(0, Random.Shared.Next(5, 10)))
                                        column.Item().Text(Placeholders.Paragraph());

                                    foreach (var blockId in Enumerable.Range(0, Random.Shared.Next(5, 10)))
                                        column.Item().Width(50 + blockId * 5).Height(50).Background(Placeholders.BackgroundColor());
                                }
                            });
                    });
            });
    }
    
    [Test]
    public void Decoration()
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
                    .DefaultTextStyle(x => x.FontSize(8))
                    .MultiColumn(multiColumn =>
                    {
                        multiColumn.Columns(3);
                        multiColumn.Spacing(25);
                        multiColumn.BalanceHeight();
                        
                        multiColumn.Spacer().AlignCenter().LineVertical(2).LineColor(Colors.Grey.Medium);
                        
                        multiColumn
                            .Content()
                            .Column(column =>
                            {
                                column.Spacing(10);

                                foreach (var blockId in Enumerable.Range(0, 100))
                                    column.Item().Height(50).Background(Placeholders.BackgroundColor());
                            });
                    });
            });
    }
    
    [Test]
    public void Table()
    {
        Settings.EnableCaching = true;
        
        RenderingTest
            .Create()
            .PageSize(PageSizes.A4)
            .ProducePdf()
            .ShowResults()
            .Render(container =>
            {
                container
                    .Padding(25)
                    .DefaultTextStyle(x => x.FontSize(8))
                    .MultiColumn(multiColumn =>
                    {
                        multiColumn.Spacing(10);
                        multiColumn.BalanceHeight(false);
                        
                        multiColumn
                            .Content()
                            .Border(1)
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(3);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(Style).Text("#").Bold();
                                    header.Cell().Element(Style).Text("Label").Bold();
                                    header.Cell().Element(Style).Text("Description").Bold();
                            
                                    IContainer Style(IContainer container) => container.Border(1).BorderColor(Colors.Grey.Medium).Background(Colors.Grey.Lighten2).Padding(2);
                                });
                        
                                foreach (var i in Enumerable.Range(1, 1_000))
                                {
                                    table.Cell().Element(Style).ShowEntire().Text(i.ToString());
                                    table.Cell().Element(Style).ShowEntire().Text(Placeholders.Label());
                                    table.Cell().Element(Style).ShowEntire().Text(Placeholders.Sentence());
                            
                                    IContainer Style(IContainer container) => container.Border(1).BorderColor(Colors.Grey.Medium).Background(i % 2 == 0 ? Colors.White : Colors.Grey.Lighten4).Padding(2);
                                }
                            });
                    });
            });
    }
    
    [Test]
    public void BalanceHeight()
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
                    .DefaultTextStyle(x => x.FontSize(8))
                    .MultiColumn(multiColumn =>
                    {
                        multiColumn.Columns(4);
                        multiColumn.BalanceHeight(true);
                        multiColumn.Spacing(10);
                        
                        multiColumn
                            .Content()
                            .Column(column =>
                            {
                                column.Spacing(10);

                                foreach (var sectionId in Enumerable.Range(0, 20))
                                {
                                    column.Item().Text(Placeholders.Paragraph());
                                }
                            });
                    });
            });
    }
    
    [Test]
    public void RightToLeft()
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
                    .DefaultTextStyle(x => x.FontSize(8))
                    .ShrinkVertical()
                    .ContentFromRightToLeft()
                    .MultiColumn(multiColumn =>
                    {
                        multiColumn.Columns(4);
                        multiColumn.BalanceHeight(true);
                        multiColumn.Spacing(10);
                        
                        multiColumn
                            .Content()
                            .Column(column =>
                            {
                                column.Spacing(10);

                                foreach (var i in Enumerable.Range(0, 100))
                                {
                                    column.Item().Height(50).Background(Placeholders.BackgroundColor()).Text(i.ToString());
                                }
                            });
                    });
            });
    }
}