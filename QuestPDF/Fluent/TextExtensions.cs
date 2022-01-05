using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Elements.Text;
using QuestPDF.Elements.Text.Items;
using QuestPDF.Infrastructure;
using static System.String;

namespace QuestPDF.Fluent
{
    public class TextDescriptor
    {
        private ICollection<TextBlock> TextBlocks { get; } = new List<TextBlock>();
        private TextStyle DefaultStyle { get; set; } = TextStyle.Default;
        internal HorizontalAlignment Alignment { get; set; } = HorizontalAlignment.Left;
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
            
            TextBlocks.Last().Items.Add(item);
        }
        
        public void Span(string? text, TextStyle? style = null)
        {
            if (IsNullOrEmpty(text))
                return;
            
            style ??= TextStyle.Default;
 
            var items = text
                .Replace("\r", string.Empty)
                .Split(new[] { '\n' }, StringSplitOptions.None)
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
                    Items = new List<ITextBlockItem> { x }
                })
                .ToList()
                .ForEach(TextBlocks.Add);
        }

        public void Line(string? text, TextStyle? style = null)
        {
            text ??= string.Empty;
            Span(text + Environment.NewLine, style);
        }

        public void EmptyLine()
        {
            Span(Environment.NewLine);
        }

        private void PageNumber(string slotName, TextStyle? style = null)
        {
            if (IsNullOrEmpty(slotName))
                throw new ArgumentException(nameof(slotName));
            
            style ??= TextStyle.Default;
            
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
            if (IsNullOrEmpty(locationName))
                throw new ArgumentException(nameof(locationName));
            
            PageNumber(locationName, style);
        }
        
        public void InternalLocation(string? text, string locationName, TextStyle? style = null)
        {
            if (IsNullOrEmpty(text))
                return;
            
            if (IsNullOrEmpty(locationName))
                throw new ArgumentException(nameof(locationName));

            style ??= TextStyle.Default;
            
            AddItemToLastTextBlock(new TextBlockInternalLink
            {
                Style = style,
                Text = text,
                LocationName = locationName
            });
        }
        
        public void ExternalLocation(string? text, string url, TextStyle? style = null)
        {
            if (IsNullOrEmpty(text))
                return;
            
            if (IsNullOrEmpty(url))
                throw new ArgumentException(nameof(url));
            
            style ??= TextStyle.Default;
            
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
            
            return container.AlignBottom().MinimalBox();
        }
        
        internal void Compose(IContainer container)
        {
            TextBlocks.ToList().ForEach(x => x.Alignment = Alignment);

            container.DefaultTextStyle(DefaultStyle).Stack(stack =>
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
            var descriptor = new TextDescriptor();
            
            if (element is Alignment alignment)
                descriptor.Alignment = alignment.Horizontal;
            
            content?.Invoke(descriptor);
            descriptor.Compose(element);
        }
        
        public static void Text(this IContainer element, object? text, TextStyle? style = null)
        {
            element.Text(x => x.Span(text?.ToString(), style));
        }
    }
}