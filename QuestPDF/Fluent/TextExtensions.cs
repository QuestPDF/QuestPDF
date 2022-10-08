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
    public class TextSpanDescriptor
    {
        internal TextStyle TextStyle = TextStyle.Default;
        internal Action<TextStyle> AssignTextStyle { get; }

        internal TextSpanDescriptor(Action<TextStyle> assignTextStyle)
        {
            AssignTextStyle = assignTextStyle;
        }

        internal void MutateTextStyle(Func<TextStyle, TextStyle> handler)
        {
            TextStyle = handler(TextStyle);
            AssignTextStyle(TextStyle);
        }
    }

    public delegate string PageNumberFormatter(int? pageNumber);
    
    public class TextPageNumberDescriptor : TextSpanDescriptor
    {
        internal Action<PageNumberFormatter> AssignFormatFunction { get; }
        
        internal TextPageNumberDescriptor(Action<TextStyle> assignTextStyle, Action<PageNumberFormatter> assignFormatFunction) : base(assignTextStyle)
        {
            AssignFormatFunction = assignFormatFunction;
            AssignFormatFunction(x => x?.ToString());
        }

        public TextPageNumberDescriptor Format(PageNumberFormatter formatter)
        {
            AssignFormatFunction(formatter);
            return this;
        }
    }
    
    public class TextDescriptor
    {
        private ICollection<TextBlock> TextBlocks { get; } = new List<TextBlock>();
        private TextStyle? DefaultStyle { get; set; }
        internal HorizontalAlignment Alignment { get; set; } = HorizontalAlignment.Left;
        private float Spacing { get; set; } = 0f;

        public void DefaultTextStyle(TextStyle style)
        {
            DefaultStyle = style;
        }
        
        public void DefaultTextStyle(Func<TextStyle, TextStyle> style)
        {
            DefaultStyle = style(TextStyle.Default);
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
        
        [Obsolete("This element has been renamed since version 2022.3. Please use the overload that returns a TextSpanDescriptor object which allows to specify text style.")]
        public void Span(string? text, TextStyle style)
        {
            Span(text).Style(style);
        }
        
        public TextSpanDescriptor Span(string? text)
        {
            if (text == null)
                return new TextSpanDescriptor(_ => { });
 
            var items = text
                .Replace("\r", string.Empty)
                .Split(new[] { '\n' }, StringSplitOptions.None)
                .Select(x => new TextBlockSpan
                {
                    Text = x
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

            return new TextSpanDescriptor(x => items.ForEach(y => y.Style = x));
        }

        public TextSpanDescriptor Line(string? text)
        {
            text ??= string.Empty;
            return Span(text + Environment.NewLine);
        }

        public TextSpanDescriptor EmptyLine()
        {
            return Span(Environment.NewLine);
        }
        
        private TextPageNumberDescriptor PageNumber(Func<IPageContext, int?> pageNumber)
        {
            var textBlockItem = new TextBlockPageNumber();
            AddItemToLastTextBlock(textBlockItem);
            
            return new TextPageNumberDescriptor(x => textBlockItem.Style = x, x => textBlockItem.Source = context => x(pageNumber(context)));
        }

        public TextPageNumberDescriptor CurrentPageNumber()
        {
            return PageNumber(x => x.CurrentPage);
        }
        
        public TextPageNumberDescriptor TotalPages()
        {
            return PageNumber(x => x.GetLocation(PageContext.DocumentLocation)?.Length);
        }

        [Obsolete("This element has been renamed since version 2022.3. Please use the BeginPageNumberOfSection method.")]
        public void PageNumberOfLocation(string locationName, TextStyle? style = null)
        {
            BeginPageNumberOfSection(locationName).Style(style);
        }
        
        public TextPageNumberDescriptor BeginPageNumberOfSection(string locationName)
        {
            return PageNumber(x => x.GetLocation(locationName)?.PageStart);
        }
        
        public TextPageNumberDescriptor EndPageNumberOfSection(string locationName)
        {
            return PageNumber(x => x.GetLocation(locationName)?.PageEnd);
        }
        
        public TextPageNumberDescriptor PageNumberWithinSection(string locationName)
        {
            return PageNumber(x => x.CurrentPage + 1 - x.GetLocation(locationName)?.PageStart);
        }
        
        public TextPageNumberDescriptor TotalPagesWithinSection(string locationName)
        {
            return PageNumber(x => x.GetLocation(locationName)?.Length);
        }
        
        public TextSpanDescriptor SectionLink(string? text, string sectionName)
        {
            if (IsNullOrEmpty(sectionName))
                throw new ArgumentException("Section name cannot be null or empty", nameof(sectionName));

            if (IsNullOrEmpty(text))
                return new TextSpanDescriptor(_ => { });

            var textBlockItem = new TextBlockSectionLink
            {
                Text = text,
                SectionName = sectionName
            };

            AddItemToLastTextBlock(textBlockItem);
            return new TextSpanDescriptor(x => textBlockItem.Style = x);
        }
        
        [Obsolete("This element has been renamed since version 2022.3. Please use the SectionLink method.")]
        public void InternalLocation(string? text, string locationName, TextStyle? style = null)
        {
            SectionLink(text, locationName).Style(style);
        }
        
        public TextSpanDescriptor Hyperlink(string? text, string url)
        {
            if (IsNullOrEmpty(url))
                throw new ArgumentException("Url cannot be null or empty", nameof(url));

            if (IsNullOrEmpty(text))
                return new TextSpanDescriptor(_ => { });
            
            var textBlockItem = new TextBlockHyperlink
            {
                Text = text,
                Url = url
            };

            AddItemToLastTextBlock(textBlockItem);
            return new TextSpanDescriptor(x => textBlockItem.Style = x);
        }
        
        [Obsolete("This element has been renamed since version 2022.3. Please use the Hyperlink method.")]
        public void ExternalLocation(string? text, string url, TextStyle? style = null)
        {
            Hyperlink(text, url).Style(style);
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
            
            if (DefaultStyle != null)
                container = container.DefaultTextStyle(DefaultStyle);

            if (TextBlocks.Count == 1)
            {
                container.Element(TextBlocks.First());
                return;
            }
            
            container.Column(column =>
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
        
        [Obsolete("This element has been renamed since version 2022.3. Please use the overload that returns a TextSpanDescriptor object which allows to specify text style.")]
        public static void Text(this IContainer element, object? text, TextStyle style)
        {
            element.Text(text).Style(style);
        }

        [Obsolete("Please use an overload where the text parameter is passed explicitly as a string.")]
        public static TextSpanDescriptor Text(this IContainer element, object? text)
        {
            return element.Text(text?.ToString());
        }

        public static TextSpanDescriptor Text(this IContainer element, string? text)
        {
            var descriptor = (TextSpanDescriptor) null!;
            element.Text(x => descriptor = x.Span(text));
            return descriptor;
        }
    }
}