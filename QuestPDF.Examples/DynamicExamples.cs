using System.Collections.Generic;
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
    
    public class OrdersTable : IDynamicComponent
    {
        private ICollection<OrderItem> Items { get; }
        private ICollection<OrderItem> ItemsLeft { get; set; }
        
        public OrdersTable(ICollection<OrderItem> items)
        {
            Items = items;
        }
        
        public void Compose(DynamicContext context, IDynamicContainer container)
        {
            if (context.Operation == DynamicLayoutOperation.Reset)
            {
                ItemsLeft = new List<OrderItem>(Items);
                return;
            }

            var header = ComposeHeader(context);
            var sampleFooter = ComposeFooter(context, Enumerable.Empty<OrderItem>());
            var decorationHeight = header.Size.Height + sampleFooter.Size.Height;
            
            var rows = GetItemsForPage(context, decorationHeight).ToList();
            var footer = ComposeFooter(context, rows.Select(x => x.Item));

            if (ItemsLeft.Count > rows.Count)
                container.HasMoreContent();
            
            if (context.Operation == DynamicLayoutOperation.Draw)
                ItemsLeft = ItemsLeft.Skip(rows.Count).ToList();

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
        }

        private IDynamicElement ComposeHeader(DynamicContext context)
        {
            return context.CreateElement(element =>
            {
                element
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
                    .Padding(5)
                    .AlignRight()
                    .Text($"Subtotal: {total}$", TextStyle.Default.Size(14).SemiBold());
            });
        }
        
        private IEnumerable<(OrderItem Item, IDynamicElement Element)> GetItemsForPage(DynamicContext context, float decorationHeight)
        {
            var totalHeight = decorationHeight;
            var counter = Items.Count - ItemsLeft.Count + 1;
            
            foreach (var orderItem in ItemsLeft)
            {
                var element = context.CreateElement(content =>
                {
                    content
                        .BorderBottom(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .Padding(5)
                        .Row(row =>
                        {
                            row.ConstantItem(30).Text(counter++);
                            row.RelativeItem().Text(orderItem.ItemName);
                            row.ConstantItem(50).AlignRight().Text(orderItem.Count);
                            row.ConstantItem(50).AlignRight().Text($"{orderItem.Price}$");
                            row.ConstantItem(50).AlignRight().Text($"{orderItem.Count*orderItem.Price}$");
                        });
                });

                var elementHeight = element.Size.Height;
                    
                if (totalHeight + elementHeight > context.AvailableSize.Height)
                    break;
                    
                totalHeight += elementHeight;
                yield return (orderItem, element);
            }
        }
    }
    
    public static class DynamicExamples
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
                                .Dynamic(new OrdersTable(items));
                        });
                });
        }
    }
}