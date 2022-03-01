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

        private static Func<int?, string> DefaultTextFormat = x => (x ?? 123).ToString();
        
        private void PageNumber(Func<IPageContext, int?> pageNumber, TextStyle? style, Func<int?, string>? format)
        {
            style ??= TextStyle.Default;
            format ??= DefaultTextFormat;
            
            AddItemToLastTextBlock(new TextBlockPageNumber()
            {
                Source = context => format(pageNumber(context)),
                Style = style
            });
        }

        public void CurrentPageNumber(TextStyle? style = null, Func<int?, string>? format = null)
        {
            PageNumber(x => x.CurrentPage, style, format);
        }
        
        public void TotalPages(TextStyle? style = null, Func<int?, string>? format = null)
        {
            PageNumber(x => x.GetLocation(PageContext.DocumentLocation)?.Length, style, format);
        }

        [Obsolete("This element has been renamed since version 2022.3. Please use the BeginPageNumberOfSection method.")]
        public void PageNumberOfLocation(string locationName, TextStyle? style = null)
        {
            BeginPageNumberOfSection(locationName);
        }
        
        public void BeginPageNumberOfSection(string locationName, TextStyle? style = null, Func<int?, string>? format = null)
        {
            PageNumber(x => x.GetLocation(locationName)?.PageStart, style, format);
        }
        
        public void EndPageNumberOfSection(string locationName, TextStyle? style = null, Func<int?, string>? format = null)
        {
            PageNumber(x => x.GetLocation(locationName)?.PageEnd, style, format);
        }
        
        public void PageNumberWithinSection(string locationName, TextStyle? style = null, Func<int?, string>? format = null)
        {
            PageNumber(x => x.CurrentPage + 1 - x.GetLocation(locationName)?.PageEnd, style, format);
        }
        
        public void TotalPagesWithinSection(string locationName, TextStyle? style = null, Func<int?, string>? format = null)
        {
            PageNumber(x => x.GetLocation(locationName)?.Length, style, format);
        }
        
        public void SectionLink(string? text, string sectionName, TextStyle? style = null)
        {
            if (IsNullOrEmpty(text))
                return;
            
            if (IsNullOrEmpty(sectionName))
                throw new ArgumentException(nameof(sectionName));

            style ??= TextStyle.Default;
            
            AddItemToLastTextBlock(new TextBlockSectionLink
            {
                Style = style,
                Text = text,
                SectionName = sectionName
            });
        }
        
        [Obsolete("This element has been renamed since version 2022.3. Please use the SectionLink method.")]
        public void InternalLocation(string? text, string locationName, TextStyle? style = null)
        {
            SectionLink(text, locationName, style);
        }
        
        public void Hyperlink(string? text, string url, TextStyle? style = null)
        {
            if (IsNullOrEmpty(text))
                return;
            
            if (IsNullOrEmpty(url))
                throw new ArgumentException(nameof(url));
            
            style ??= TextStyle.Default;
            
            AddItemToLastTextBlock(new TextBlockHyperlink
            {
                Style = style,
                Text = text,
                Url = url
            });
        }
        
        [Obsolete("This element has been renamed since version 2022.3. Please use the Hyperlink method.")]
        public void ExternalLocation(string? text, string url, TextStyle? style = null)
        {
            Hyperlink(text, url, style);
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