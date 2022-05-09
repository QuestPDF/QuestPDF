using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Elements;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class OrdersTable : IDynamicComponent<OrdersTableState>
    {
        private IList<OrderItem> Items { get; }
        public OrdersTableState State { get; set; }

        public OrdersTable(IList<OrderItem> items)
        {
            Items = items;

            State = new OrdersTableState
            {
                ShownItemsCount = 0
            };
        }
        
        public DynamicComponentComposeResult Compose(DynamicContext context)
        {
            var possibleItems = Enumerable
                .Range(1, Items.Count - State.ShownItemsCount)
                .Select(itemsToDisplay => ComposeContent(context, itemsToDisplay))
                .TakeWhile(x => x.Size.Height <= context.AvailableSize.Height)
                .ToList();

            State = new OrdersTableState
            {
                ShownItemsCount = State.ShownItemsCount + possibleItems.Count
            };

            return new DynamicComponentComposeResult
            {
                Content = possibleItems.Last(),
                HasMoreContent = State.ShownItemsCount < Items.Count
            };
        }

        private IDynamicElement ComposeContent(DynamicContext context, int itemsToDisplay)
        {
            var total = Items.Skip(State.ShownItemsCount).Take(itemsToDisplay).Sum(x => x.Count * x.Price);

            return context.CreateElement(container =>
            {
                container
                    .MinimalBox()
                    .Width(context.AvailableSize.Width)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);
                            columns.RelativeColumn();
                            columns.ConstantColumn(50);
                            columns.ConstantColumn(50);
                            columns.ConstantColumn(50);
                        });
                        
                        table.Header(header =>
                        {
                            header.Cell().Element(Style).Text("#");
                            header.Cell().Element(Style).Text("Item name");
                            header.Cell().Element(Style).AlignRight().Text("Count");
                            header.Cell().Element(Style).AlignRight().Text("Price");
                            header.Cell().Element(Style).AlignRight().Text("Total");

                            IContainer Style(IContainer container)
                            {
                                return container
                                    .DefaultTextStyle(x => x.SemiBold())
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Grey.Darken2)
                                    .Padding(5);
                            }
                        });
                        
                        table.Footer(footer =>
                        {
                            footer
                                .Cell().ColumnSpan(5)
                                .AlignRight()
                                .Text($"Subtotal: {total}$", TextStyle.Default.Size(14).SemiBold());
                        });
                        
                        foreach (var index in Enumerable.Range(State.ShownItemsCount, itemsToDisplay))
                        {
                            var item = Items[index];
                                
                            table.Cell().Element(Style).Text(index + 1);
                            table.Cell().Element(Style).Text(item.ItemName);
                            table.Cell().Element(Style).AlignRight().Text(item.Count);
                            table.Cell().Element(Style).AlignRight().Text($"{item.Price}$");
                            table.Cell().Element(Style).AlignRight().Text($"{item.Count*item.Price}$");

                            IContainer Style(IContainer container)
                            {
                                return container
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Padding(5);
                            }
                        }
                    });
            });
        }
    }
    
    public static class DynamicSimpleTableExample
    {
        [Test]
        public static void Dynamic()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A5)
                .ShowResults()
                .Render(container =>
                {
                    var items = Enumerable.Range(0, 25).Select(x => new OrderItem()).ToList();
                    
                    container
                        .Background(Colors.White)
                        .Padding(25)
                        .Decoration(decoration =>
                        {
                            decoration
                                .Header()
                                .PaddingBottom(5)
                                .Text(text =>
                                {
                                    text.DefaultTextStyle(TextStyle.Default.SemiBold().FontColor(Colors.Blue.Darken2).FontSize(16));
                                    text.Span("Page ");
                                    text.CurrentPageNumber();
                                    text.Span(" / ");
                                    text.TotalPages();
                                });
                            
                            decoration
                                .Content()
                                .Dynamic(new OrdersTable(items));
                        });
                });
        }
    }
}