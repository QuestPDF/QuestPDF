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
        private Action<IContainer> ContentDirectionTemplate(Action<IContainer> content)
        {
            return container =>
            {
                container
                    .Padding(20)
                    .MinimalBox()
                    .Border(1)
                    .ExtendHorizontal()
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });
                        
                        table.Header(header =>
                        {
                            header.Cell().ContentFromLeftToRight().Element(HeaderCell("Left-to-right"));
                            header.Cell().ContentFromRightToLeft().Element(HeaderCell("Right-to-left"));

                            static Action<IContainer> HeaderCell(string label)
                            {
                                return container => container
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Medium)
                                    .Background(Colors.Grey.Lighten3)
                                    .PaddingHorizontal(10)
                                    .PaddingVertical(5)
                                    .Text(label)
                                    .FontSize(18)
                                    .SemiBold();
                            }
                        });

                        table.Cell().Element(TestCell).ContentFromLeftToRight().Element(content);
                        
                        table.Cell()
                            .Element(TestCell)
                            .ContentFromRightToLeft()
                            .Element(content);
                        
                        static IContainer TestCell(IContainer container)
                        {
                            return container.Border(1).BorderColor(Colors.Grey.Medium).Padding(10);
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
                        
                        //page.ContentFromRightToLeft();
                        
                        page.Content().Column(column =>
                        {
                            column.Spacing(20);
                            
                            column.Item().Row(row =>
                            {
                                row.Spacing(10);
                                
                                row.AutoItem().AlignMiddle().Width(20).Height(20).Image(Placeholders.Image);
                                
                                row.RelativeItem()
                                    .Text("Document title")
                                    .FontSize(24).FontColor(Colors.Blue.Accent1).SemiBold();
                            });
                            
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                foreach (var i in Enumerable.Range(0, 9))
                                {
                                    var width = (i % 4 == 0) ? 2 : 1;

                                    table
                                        .Cell()
                                        .ColumnSpan((uint)width)
                                        .Background(i % 4 == 0 ? Colors.Grey.Lighten1 : Colors.Grey.Lighten2)
                                        .Padding(5)
                                        .AlignCenter()
                                        .Text(i)
                                        .FontSize(20);
                                }
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

                    column.Item().Background(Colors.Grey.Lighten3).Text("Default alignment").FontSize(13);
                    column.Item().Element(ContentWithAlignment(null));
                    
                    column.Item().Background(Colors.Grey.Lighten3).Text("Left alignment").FontSize(14);
                    column.Item().Element(ContentWithAlignment(InlinedAlignment.Left));
                    
                    column.Item().Background(Colors.Grey.Lighten3).Text("Center alignment").FontSize(14);
                    column.Item().Element(ContentWithAlignment(InlinedAlignment.Center));
                    
                    column.Item().Background(Colors.Grey.Lighten3).Text("Right alignment").FontSize(14);
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

                    column.Item().Background(Colors.Grey.Lighten3).Text("Default alignment").FontSize(13);
                    column.Item().Element(ContentWithAlignment(null));
                    
                    column.Item().Background(Colors.Grey.Lighten3).Text("Left alignment").FontSize(14);
                    column.Item().Element(ContentWithAlignment(HorizontalAlignment.Left));
                    
                    column.Item().Background(Colors.Grey.Lighten3).Text("Center alignment").FontSize(14);
                    column.Item().Element(ContentWithAlignment(HorizontalAlignment.Center));
                    
                    column.Item().Background(Colors.Grey.Lighten3).Text("Right alignment").FontSize(14);
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