using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Elements;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class OrderItem
    {
        public string ItemName { get; set; } = Placeholders.Label();
        public int Price { get; set; } = Placeholders.Random.Next(1, 11) * 10;
        public int Count { get; set; } = Placeholders.Random.Next(1, 11);
    }

    public struct OrdersTableState
    {
        public int ShownItemsCount { get; set; }
    }
    
    public class OptimizedOrdersTable : IDynamicComponent<OrdersTableState>
    {
        private ICollection<OrderItem> Items { get; }
        public OrdersTableState State { get; set; }

        public OptimizedOrdersTable(ICollection<OrderItem> items)
        {
            Items = items;

            State = new OrdersTableState
            {
                ShownItemsCount = 0
            };
        }
        
        public DynamicComponentComposeResult Compose(DynamicContext context)
        {
            var header = ComposeHeader(context);
            var sampleFooter = ComposeFooter(context, Enumerable.Empty<OrderItem>());
            var decorationHeight = header.Size.Height + sampleFooter.Size.Height;
            
            var rows = GetItemsForPage(context, decorationHeight).ToList();
            var footer = ComposeFooter(context, rows.Select(x => x.Item));

            var content = context.CreateElement(container =>
            {
                container.MinimalBox().Decoration(decoration =>
                {
                    decoration.Header().Element(header);

                    decoration.Content().Box().Stack(stack =>
                    {
                        foreach (var row in rows)
                            stack.Item().Element(row.Element);
                    });

                    decoration.Footer().Element(footer);
                });
            });

            State = new OrdersTableState
            {
                ShownItemsCount = State.ShownItemsCount + rows.Count
            };

            return new DynamicComponentComposeResult
            {
                Content = content,
                HasMoreContent = State.ShownItemsCount < Items.Count
            };
        }

        private IDynamicElement ComposeHeader(DynamicContext context)
        {
            return context.CreateElement(element =>
            {
                element
                    .Width(context.AvailableSize.Width)
                    .BorderBottom(1)
                    .BorderColor(Colors.Grey.Darken2)
                    .Padding(5)
                    .Row(row =>
                    {
                        var textStyle = TextStyle.Default.SemiBold();

                        row.ConstantItem(30).Text("#", textStyle);
                        row.RelativeItem().Text("Item name", textStyle);
                        row.ConstantItem(50).AlignRight().Text("Count", textStyle);
                        row.ConstantItem(50).AlignRight().Text("Price", textStyle);
                        row.ConstantItem(50).AlignRight().Text("Total", textStyle);
                    });
            });
        }
        
        private IDynamicElement ComposeFooter(DynamicContext context, IEnumerable<OrderItem> items)
        {
            var total = items.Sum(x => x.Count * x.Price);

            return context.CreateElement(element =>
            {
                element
                    .Width(context.AvailableSize.Width)
                    .Padding(5)
                    .AlignRight()
                    .Text($"Subtotal: {total}$", TextStyle.Default.Size(14).SemiBold());
            });
        }
        
        private IEnumerable<(OrderItem Item, IDynamicElement Element)> GetItemsForPage(DynamicContext context, float decorationHeight)
        {
            var totalHeight = decorationHeight;

            foreach (var index in Enumerable.Range(State.ShownItemsCount, Items.Count - State.ShownItemsCount))
            {
                var item = Items.ElementAt(index);
                
                var element = context.CreateElement(content =>
                {
                    content
                        .Width(context.AvailableSize.Width)
                        .BorderBottom(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .Padding(5)
                        .Row(row =>
                        {
                            row.ConstantItem(30).Text((index + 1).ToString(CultureInfo.InvariantCulture));
                            row.RelativeItem().Text(item.ItemName);
                            row.ConstantItem(50).AlignRight().Text(item.Count.ToString(CultureInfo.InvariantCulture));
                            row.ConstantItem(50).AlignRight().Text($"{item.Price}$");
                            row.ConstantItem(50).AlignRight().Text($"{item.Count*item.Price}$");
                        });
                });

                var elementHeight = element.Size.Height;
                    
                if (totalHeight + elementHeight > context.AvailableSize.Height)
                    break;
                    
                totalHeight += elementHeight;
                yield return (item, element);
            }
        }
    }
    
    public static class DynamicOptimizedExamples
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
                                    text.Span(" of ");
                                    text.TotalPages();
                                });
                            
                            decoration
                                .Content()
                                .Dynamic(new OptimizedOrdersTable(items));
                        });
                });
        }
    }
}