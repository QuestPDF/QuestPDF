using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class InlinedExamples
    {
        [Test]
        public void Inlined()
        {
            RenderingTest
                .Create()
                .PageSize(800, 650)
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .Decoration(decoration =>
                        {
                            decoration.Before().Text(text =>
                            {
                                text.DefaultTextStyle(TextStyle.Default.Size(20));
                                
                                text.CurrentPageNumber();
                                text.Span(" / ");
                                text.TotalPages();
                            });
                            
                            decoration
                                .Content()
                                .PaddingTop(25)
                                //.MinimalBox()
                                .Border(1)
                                .Background(Colors.Grey.Lighten2)
                                .Inlined(inlined =>
                                {
                                    inlined.Spacing(25);

                                    inlined.AlignSpaceAround();
                                    inlined.BaselineMiddle();

                                    var random = new Random(123);

                                    foreach (var _ in Enumerable.Range(0, 50))
                                    {
                                        var width = random.Next(2, 7);
                                        var height = random.Next(2, 7);

                                        var sizeText = $"{width}×{height}";
                                        
                                        inlined
                                            .Item()
                                            .Border(1)
                                            .Width(width * 25)
                                            .Height(height * 25)
                                            .Background(Placeholders.BackgroundColor())
                                            .Layers(layers =>
                                            {
                                                layers.Layer().Grid(grid =>
                                                {
                                                    grid.Columns(width);
                                                    Enumerable.Range(0, width * height).ToList().ForEach(x => grid.Item().Border(1).BorderColor(Colors.White).Width(25).Height(25));
                                                });
                                                
                                                layers
                                                    .PrimaryLayer()
                                                    .AlignCenter()
                                                    .AlignMiddle()
                                                    .Text(sizeText)
                                                    .Size(15);
                                            });
                                    }
                                });
                        });
                });
        }
        
        [Test]
        public void Inline_AlignLeft_BaselineBottom()
        {
            RenderingTest
                .Create()
                .PageSize(800, 600)
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(20)
                        .MinimalBox()
                        .Border(1)
                        .Background(Colors.Grey.Lighten4)
                        .Inlined(inlined =>
                        {
                            inlined.VerticalSpacing(50);
                            inlined.HorizontalSpacing(25);
                            inlined.AlignRight();
                            inlined.BaselineMiddle();

                            foreach (var _ in Enumerable.Range(0, 100))
                                inlined.Item().Element(RandomBlock);
                        });
                });

            void RandomBlock(IContainer container)
            {
                container
                    .Width(Placeholders.Random.Next(1, 5) * 20)
                    .Height(Placeholders.Random.Next(1, 5) * 20)
                    .Border(1)
                    .BorderColor(Colors.Grey.Darken2)
                    .Background(Placeholders.BackgroundColor());
            }
        }
    }
}