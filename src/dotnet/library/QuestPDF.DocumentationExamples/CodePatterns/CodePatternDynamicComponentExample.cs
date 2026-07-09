using System.Globalization;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples.CodePatterns;

public class CodePatternDynamicComponentExample
{ 
    [Test]
    public static void Dynamic()
    {
        var items = Enumerable.Range(0, 25).Select(x => new OrderItem()).ToList();
        
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(50);

                    page.Content()
                        .Decoration(decoration =>
                        {
                            decoration
                                .Before()
                                .PaddingBottom(10)
                                .Text(text =>
                                {
                                    text.DefaultTextStyle(TextStyle.Default.Bold().FontColor(Colors.Blue.Darken2));
                                    text.Span("Page ");
                                    text.CurrentPageNumber();
                                    text.Span(" of ");
                                    text.TotalPages();
                                });
                            
                            decoration
                                .Content()
                                .Dynamic(new OrdersTableWithPageSubtotalsComponent(items));
                        });
                });
            })
            .GeneratePdf("code-pattern-dynamic-component-table-with-per-page-subtotals.pdf");
    }
    
    public class OrderItem
    {
        public string ItemName { get; set; } = Placeholders.Label();
        public int Price { get; set; } = Placeholders.Random.Next(1, 11) * 10;
        public int Count { get; set; } = Placeholders.Random.Next(1, 11);
    }

    public struct OrdersTableWithPageSubtotalsComponentState
    {
        public int ShownItemsCount { get; set; }
    }
    
    public class OrdersTableWithPageSubtotalsComponent : IDynamicComponent<OrdersTableWithPageSubtotalsComponentState>
    {
        private ICollection<OrderItem> Items { get; }
        public OrdersTableWithPageSubtotalsComponentState State { get; set; }

        public OrdersTableWithPageSubtotalsComponent(ICollection<OrderItem> items)
        {
            Items = items;

            State = new OrdersTableWithPageSubtotalsComponentState
            {
                ShownItemsCount = 0
            };
        }
        
        public DynamicComponentComposeResult Compose(DynamicContext context)
        {
            var header = ComposeHeader(context);
            var sampleFooter = ComposeFooter(context, []);
            var decorationHeight = header.Size.Height + sampleFooter.Size.Height;
            
            var rows = GetItemsForPage(context, decorationHeight).ToList();
            var footer = ComposeFooter(context, rows.Select(x => x.Item));

            var content = context.CreateElement(container =>
            {
                container.Shrink().Decoration(decoration =>
                {
                    decoration.Before().Element(header);

                    decoration.Content().Column(column =>
                    {
                        foreach (var row in rows)
                            column.Item().Element(row.Element);
                    });

                    decoration.After().Element(footer);
                });
            });

            State = new OrdersTableWithPageSubtotalsComponentState
            {
                ShownItemsCount = State.ShownItemsCount + rows.Count
            };

            return new DynamicComponentComposeResult
            {
                Content = content,
                HasMoreContent = State.ShownItemsCount < Items.Count
            };
        }

        private static IDynamicElement ComposeHeader(DynamicContext context)
        {
            return context.CreateElement(element =>
            {
                element
                    .Width(context.AvailableSize.Width)
                    .BorderBottom(1)
                    .BorderColor(Colors.Grey.Darken2)
                    .Padding(10)
                    .DefaultTextStyle(TextStyle.Default.SemiBold())
                    .Row(row =>
                    {
                        row.ConstantItem(50).Text("#");
                        row.RelativeItem().Text("Item name");
                        row.ConstantItem(75).AlignRight().Text("Count");
                        row.ConstantItem(75).AlignRight().Text("Price");
                        row.ConstantItem(75).AlignRight().Text("Total");
                    });
            });
        }
        
        private static IDynamicElement ComposeFooter(DynamicContext context, IEnumerable<OrderItem> items)
        {
            var total = items.Sum(x => x.Count * x.Price);

            return context.CreateElement(element =>
            {
                element
                    .Width(context.AvailableSize.Width)
                    .Padding(10)
                    .AlignRight()
                    .Text($"Subtotal: {total}$")
                    .Bold();
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
                        .Padding(10)
                        .Row(row =>
                        {
                            row.ConstantItem(50).Text((index + 1).ToString(CultureInfo.InvariantCulture));
                            row.RelativeItem().Text(item.ItemName);
                            row.ConstantItem(75).AlignRight().Text(item.Count.ToString(CultureInfo.InvariantCulture));
                            row.ConstantItem(75).AlignRight().Text($"{item.Price}$");
                            row.ConstantItem(75).AlignRight().Text($"{item.Count*item.Price}$");
                        });
                });

                var elementHeight = element.Size.Height;

                // it is important to use the Size.Epsilon constant to avoid floating point comparison issues
                if (totalHeight + elementHeight > context.AvailableSize.Height + Size.Epsilon)
                    break;
                    
                totalHeight += elementHeight;
                yield return (item, element);
            }
        }
    }
}