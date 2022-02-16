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

        public void ParagraphSpacing(float value, Unit unit = Unit.Point)
        {
            Spacing = value.ToPoints(unit);
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

        private void PageNumber(Func<IPageContext, string> source, TextStyle? style = null)
        {
            style ??= TextStyle.Default;
            
            AddItemToLastTextBlock(new TextBlockPageNumber()
            {
                Source = source,
                Style = style
            });
        }
        
        public void CurrentPageNumber(Func<int?, string> format, TextStyle? style = null)
        {
            PageNumber(context => format(context.CurrentPage), style);
        }
        
        public void CurrentPageNumber(TextStyle? style = null)
        {
            CurrentPageNumber(x => x.ToString(), style);
        }

        public void TotalPages(Func<int?, string> format, TextStyle? style = null)
        {
            PageNumber(context => format(context.GetLocation(PageContext.DocumentLocation)?.Length), style);
        }
        
        public void TotalPages(TextStyle? style = null)
        {
            TotalPages(x => x.ToString(), style);
        }
        
        public void PageNumberOfLocation(string locationName, TextStyle? style = null)
        {
            BeginPageNumberOfLocation(locationName);
        }
        
        public void BeginPageNumberOfLocation(string locationName, TextStyle? style = null)
        {
            PageNumber(context => (context.GetLocation(locationName)?.PageStart ?? 123).ToString(), style);
        }
        
        public void EndPageNumberOfLocation(string locationName, TextStyle? style = null)
        {
            PageNumber(context => (context.GetLocation(locationName)?.PageEnd ?? 123).ToString(), style);
        }
        
        public void PageNumberWithinLocation(string locationName, Func<int?, string> format, TextStyle? style = null)
        {
            PageNumber(context => format(context.CurrentPage + 1 - context.GetLocation(locationName)?.PageEnd), style);
        }
        
        public void PageLengthOfLocation(string locationName, Func<int?, string> format, TextStyle? style = null)
        {
            PageNumber(context => format(context.GetLocation(locationName)?.Length), style);
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

            container.DefaultTextStyle(DefaultStyle).Column(column =>
            {
                column.Spacing(Spacing);

                foreach (var textBlock in TextBlocks)
                    column.Item().Element(textBlock);
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