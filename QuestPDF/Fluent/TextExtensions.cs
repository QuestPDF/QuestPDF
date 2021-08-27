using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Elements;
using QuestPDF.Elements.Text;
using QuestPDF.Elements.Text.Items;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class TextDescriptor
    {
        private ICollection<TextBlock> TextBlocks { get; } = new List<TextBlock>();
        private TextStyle DefaultStyle { get; set; } = TextStyle.Default;
        private HorizontalAlignment Alignment { get; set; } = HorizontalAlignment.Left;
        private float Spacing { get; set; } = 0f;

        public void DefaultTextStyle(TextStyle style)
        {
            DefaultStyle = style;
        }
        
        public void AlignLeft()
        {
            Alignment = HorizontalAlignment.Left;
        }
        
        public void AlignCenter()
        {
            Alignment = HorizontalAlignment.Center;
        }
        
        public void AlignRight()
        {
            Alignment = HorizontalAlignment.Right;
        }

        public void ParagraphSpacing(float value)
        {
            Spacing = value;
        }

        private void AddItemToLastTextBlock(ITextBlockItem item)
        {
            if (!TextBlocks.Any())
                TextBlocks.Add(new TextBlock());
            
            TextBlocks.Last().Children.Add(item);
        }
        
        public void Span(string text, TextStyle? style = null)
        {
            style ??= DefaultStyle;
            
            if (string.IsNullOrWhiteSpace(text))
                return;
            
            var items = text
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => new TextBlockSpan
                {
                    Text = x,
                    Style = style
                })
                .ToList();

            AddItemToLastTextBlock(items.First());

            items
                .Skip(1)
                .Select(x => new TextBlock
                {   
                    Children = new List<ITextBlockItem> { x }
                })
                .ToList()
                .ForEach(TextBlocks.Add);
        }

        public void NewLine()
        {
            Span(Environment.NewLine);
        }

        private void PageNumber(string slotName, TextStyle? style = null)
        {
            style ??= DefaultStyle;
            
            AddItemToLastTextBlock(new TextBlockPageNumber()
            {
                Style = style,
                SlotName = slotName
            });
        }
        
        public void CurrentPageNumber(TextStyle? style = null)
        {
            PageNumber(PageContext.CurrentPageSlot, style);
        }
        
        public void TotalPages(TextStyle? style = null)
        {
            PageNumber(PageContext.TotalPagesSlot, style);
        }
        
        public void PageNumberOfLocation(string locationName, TextStyle? style = null)
        {
            PageNumber(locationName, style);
        }
        
        public void InternalLocation(string text, string locationName, TextStyle? style = null)
        {
            style ??= DefaultStyle;
            
            AddItemToLastTextBlock(new TextBlockInternalLink
            {
                Style = style,
                Text = text,
                LocationName = locationName
            });
        }
        
        public void ExternalLocation(string text, string url, TextStyle? style = null)
        {
            style ??= DefaultStyle;
            
            AddItemToLastTextBlock(new TextBlockExternalLink
            {
                Style = style,
                Text = text,
                Url = url
            });
        }
        
        public IContainer Element()
        {
            var container = new Container();
                
            AddItemToLastTextBlock(new TextBlockElement
            {
                Element = container
            });
            
            return container.Box();
        }
        
        internal void Compose(IContainer container)
        {
            TextBlocks.ToList().ForEach(x => x.Alignment = Alignment);
            
            container.Stack(stack =>
            {
                stack.Spacing(Spacing);

                foreach (var textBlock in TextBlocks)
                    stack.Item().Element(textBlock);
            });
        }
    }
    
    public static class TextExtensions
    {
        public static void Text(this IContainer element, Action<TextDescriptor> content)
        {
            var textBlock = new TextBlock();

            if (element is Alignment alignment)
                textBlock.Alignment = alignment.Horizontal;
            
            var descriptor = new TextDescriptor();
            content?.Invoke(descriptor);
            descriptor.Compose(element);
        }
        
        public static void Text(this IContainer element, object text, TextStyle? style = null)
        {
            element.Text(x => x.Span(text.ToString(), style));
        }
    }
}