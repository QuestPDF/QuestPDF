using System;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Elements;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class ContentDirectionExamples
    {
        private static Action<IContainer> ContentDirectionTemplate(Action<IContainer> content)
        {
            return container =>
            {
                container
                    .MinimalBox()
                    .ExtendHorizontal()
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.ConstantColumn(1);
                            columns.RelativeColumn();
                        });
                        
                        table.Header(header =>
                        {
                            header.Cell().ContentFromLeftToRight().Element(HeaderCell("Left-to-right"));
                            header.Cell().LineVertical(1).LineColor(Colors.Grey.Medium);
                            header.Cell().ContentFromRightToLeft().Element(HeaderCell("Right-to-left"));

                            static Action<IContainer> HeaderCell(string label)
                            {
                                return container => container
                                    .BorderColor(Colors.Grey.Medium)
                                    .Background(Colors.Grey.Lighten2)
                                    .PaddingHorizontal(15)
                                    .PaddingVertical(5)
                                    .Text(label)
                                    .FontSize(18)
                                    .SemiBold();
                            }
                        });

                        table.Cell().Element(TestCell).ContentFromLeftToRight().Element(content);
                        table.Cell().LineVertical(1).LineColor(Colors.Grey.Medium);
                        table.Cell().Element(TestCell).ContentFromRightToLeft().Element(content);
                        
                        static IContainer TestCell(IContainer container)
                        {
                            return container.Padding(15);
                        }
                    });
            };
        }
        
        [Test]
        public void Page()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .ShowResults()
                .EnableDebugging()
                .RenderDocument(document =>
                {
                    document.Page(page =>
                    {
                        page.Size(PageSizes.A5);
                        page.Margin(20);
                        page.PageColor(Colors.White);
                        
                        page.DefaultTextStyle(x => x.FontFamily("Calibri").FontSize(20));
                        page.ContentFromRightToLeft();
                        
                        page.Content().Column(column =>
                        {
                            column.Spacing(20);

                            column.Item()
                                .Text("مثال على الفاتورة") // example invoice
                                .FontSize(32).FontColor(Colors.Blue.Darken2).SemiBold();
                            
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.ConstantColumn(75);
                                    columns.ConstantColumn(100);
                                });

                                table.Cell().Element(HeaderStyle).Text("وصف السلعة"); // item description
                                table.Cell().Element(HeaderStyle).Text("كمية"); // quantity
                                table.Cell().Element(HeaderStyle).Text("سعر"); // price

                                var items = new[]
                                {
                                    "دورة البرمجة", // programming course
                                    "دورة تصميم الرسومات", // graphics design course
                                    "تحليل وتصميم الخوارزميات", // analysis and design of algorithms
                                };
                                
                                foreach (var item in items)
                                {
                                    var price = Placeholders.Random.NextDouble() * 100;
                                    
                                    table.Cell().Text(item);
                                    table.Cell().Text(Placeholders.Random.Next(1, 10));
                                    table.Cell().Text($"USD${price:F2}");
                                }

                                static IContainer HeaderStyle(IContainer x) => x.BorderBottom(1).PaddingVertical(5);
                            });
                        });
                    });
                });
        }

        [Test]
        public void Column()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .ProduceImages()
                .ShowResults()
                .EnableDebugging()
                .Render(ContentDirectionTemplate(Content));

            void Content(IContainer container)
            {
                container.Column(column =>
                {
                    column.Spacing(5);

                    column.Item().Height(50).Width(50).Background(Colors.Red.Lighten1);
                    column.Item().Height(50).Width(100).Background(Colors.Green.Lighten1);
                    column.Item().Height(50).Width(150).Background(Colors.Blue.Lighten1);
                });
            }
        }
        
        [Test]
        public void Row()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .ProduceImages()
                .ShowResults()
                .EnableDebugging()
                .Render(ContentDirectionTemplate(Content));

            void Content(IContainer container)
            {
                container.Row(row =>
                {
                    row.Spacing(5);
                    
                    row.AutoItem().Height(50).Width(50).Background(Colors.Red.Lighten1);
                    row.AutoItem().Height(50).Width(50).Background(Colors.Green.Lighten1);
                    row.AutoItem().Height(50).Width(75).Background(Colors.Blue.Lighten1);
                });
            }
        }
        
        [Test]
        public void Table()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .ProduceImages()
                .ShowResults()
                .EnableDebugging()
                .Render(ContentDirectionTemplate(Content));

            void Content(IContainer container)
            {
                container.Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });
                    
                    table.Cell().Height(50).Background(Colors.Red.Lighten1);
                    table.Cell().Height(50).Background(Colors.Green.Lighten1);
                    table.Cell().Height(50).Background(Colors.Blue.Lighten1);
                    table.Cell().ColumnSpan(2).Height(50).Background(Colors.Orange.Lighten1);
                });
            }
        }
        
        [Test]
        public void Constrained()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .ProduceImages()
                .ShowResults()
                .EnableDebugging()
                .Render(ContentDirectionTemplate(Content));

            void Content(IContainer container)
            {
                container.Width(50).Height(50).Background(Colors.Red.Lighten1);
            }
        }
        
        [Test]
        public void Unconstrained()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .ProduceImages()
                .ShowResults()
                .EnableDebugging()
                .Render(ContentDirectionTemplate(Content));

            void Content(IContainer container)
            {
                container
                    .Width(100)
                    .Height(100)
                    .Background(Colors.Grey.Lighten3)
                    .AlignCenter()
                    .AlignMiddle()
                    
                    .Unconstrained()
                    
                    .Width(50)
                    .Height(50)
                    .Background(Colors.Red.Lighten1);
            }
        }
        
        [Test]
        public void Inlined()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .ProduceImages()
                .ShowResults()
                .EnableDebugging()
                .Render(ContentDirectionTemplate(Content));

            void Content(IContainer container)
            {
                container.Column(column =>
                {
                    column.Spacing(10);

                    column.Item().Text("Default alignment").FontSize(14).SemiBold();
                    column.Item().Element(ContentWithAlignment(null));
                    
                    column.Item().Text("Left alignment").FontSize(14).SemiBold();
                    column.Item().Element(ContentWithAlignment(InlinedAlignment.Left));
                    
                    column.Item().Text("Center alignment").FontSize(14).SemiBold();
                    column.Item().Element(ContentWithAlignment(InlinedAlignment.Center));
                    
                    column.Item().Text("Right alignment").FontSize(14).SemiBold();
                    column.Item().Element(ContentWithAlignment(InlinedAlignment.Right));
                });
                
                static Action<IContainer> ContentWithAlignment(InlinedAlignment? alignment)
                {
                    return container =>
                    {
                        container.Inlined(inlined =>
                        {
                            inlined.Spacing(5);
                            
                            inlined.Alignment(alignment);
                    
                            inlined.Item().Height(50).Width(50).Background(Colors.Red.Lighten1);
                            inlined.Item().Height(50).Width(75).Background(Colors.Green.Lighten1);
                            inlined.Item().Height(50).Width(100).Background(Colors.Blue.Lighten1);
                            inlined.Item().Height(50).Width(125).Background(Colors.Orange.Lighten1);
                        });
                    };
                }
            }
        }
        
        [Test]
        public void Text()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .ProduceImages()
                .ShowResults()
                .EnableDebugging()
                .Render(ContentDirectionTemplate(Content));

            void Content(IContainer container)
            {
                container.Column(column =>
                {
                    column.Spacing(10);

                    column.Item().Text("Default alignment").FontSize(14).SemiBold();
                    column.Item().Element(ContentWithAlignment(null));
                    
                    column.Item().Text("Left alignment").FontSize(14).SemiBold();
                    column.Item().Element(ContentWithAlignment(HorizontalAlignment.Left));
                    
                    column.Item().Text("Center alignment").FontSize(14).SemiBold();
                    column.Item().Element(ContentWithAlignment(HorizontalAlignment.Center));
                    
                    column.Item().Text("Right alignment").FontSize(14).SemiBold();
                    column.Item().Element(ContentWithAlignment(HorizontalAlignment.Right));
                });

                static Action<IContainer> ContentWithAlignment(HorizontalAlignment? alignment)
                {
                    return container =>
                    {
                        container.Text(text =>
                        {
                            text.Alignment = alignment; // internal API
                    
                            text.Span("Lorem ipsum").Bold().FontColor(Colors.Red.Medium);
                            text.Element().Width(5);
                            text.Span(Placeholders.LoremIpsum());
                        });
                    };
                }
            }
        }
        
        [Test]
        public void Decoration()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .ProduceImages()
                .ShowResults()
                .EnableDebugging()
                .Render(ContentDirectionTemplate(Content));

            void Content(IContainer container)
            {
                container.Decoration(decoration =>
                {
                    decoration.Before().Background(Colors.Green.Lighten1).Padding(5).Text("Before").FontSize(16);
                    decoration.Content().Background(Colors.Green.Lighten2).Padding(5).Text("Content").FontSize(16);
                    decoration.After().Background(Colors.Green.Lighten3).Padding(5).Text("After").FontSize(16);
                });
            }
        }
        
        [Test]
        public void Dynamic()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .ProduceImages()
                .ShowResults()
                .EnableDebugging()
                .Render(ContentDirectionTemplate(Content));

            void Content(IContainer container)
            {
                container.Dynamic(new SimpleDynamic());
            }
        }
        
        class SimpleDynamic : IDynamicComponent<int>
        {
            public int State { get; set; }

            public DynamicComponentComposeResult Compose(DynamicContext context)
            {
                var content = context.CreateElement(container =>
                {
                    container.Row(row =>
                    {
                        row.ConstantItem(50).Background(Colors.Red.Lighten2).Height(50);
                        row.ConstantItem(75).Background(Colors.Green.Lighten2).Height(50);
                        row.ConstantItem(100).Background(Colors.Blue.Lighten2).Height(50);
                    });
                });
                
                return new DynamicComponentComposeResult
                {
                    Content = content,
                    HasMoreContent = false
                };
            }
        }
    }
}