using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Elements;
using QuestPDF.Elements.Text;
using QuestPDF.Elements.Text.Items;
using QuestPDF.Infrastructure;
using static System.String;

namespace QuestPDF.Fluent
{
    public class TextSpanDescriptor
    {
        private TextBlockSpan TextBlockSpan;

        internal TextSpanDescriptor(TextBlockSpan textBlockSpan)
        {
            TextBlockSpan = textBlockSpan;
        }

        internal void MutateTextStyle<T>(Func<TextStyle, T, TextStyle> handler, T argument)
        {
            TextBlockSpan.Style = handler(TextBlockSpan.Style, argument);
        }
        
        internal void MutateTextStyle(Func<TextStyle, TextStyle> handler)
        {
            TextBlockSpan.Style = handler(TextBlockSpan.Style);
        }
    }

    /// <summary>
    /// Transforms a page number into a custom text format (e.g., roman numerals).
    /// </summary>
    /// <remarks>
    /// When <paramref name="pageNumber"/> is null, the delegate should return a default placeholder text of a typical length.
    /// </remarks>
    public delegate string PageNumberFormatter(int? pageNumber);
    
    public class TextPageNumberDescriptor : TextSpanDescriptor
    {
        internal Action<PageNumberFormatter> AssignFormatFunction { get; }
        
        internal TextPageNumberDescriptor(TextBlockSpan textBlockSpan, Action<PageNumberFormatter> assignFormatFunction) : base(textBlockSpan)
        {
            AssignFormatFunction = assignFormatFunction;
            AssignFormatFunction(x => x?.ToString());
        }

        /// <summary>
        /// Provides the capability to render the page number in a custom text format (e.g., roman numerals).
        /// <a href="https://www.questpdf.com/api-reference/text.html#page-numbers">Lear more</a>
        /// </summary>
        /// <param name="formatter">The function designated to modify the number into text. When given a null input, a typical-sized placeholder text must be produced.</param>
        public TextPageNumberDescriptor Format(PageNumberFormatter formatter)
        {
            AssignFormatFunction(formatter);
            return this;
        }
    }
    
    public class TextDescriptor
    {
        private TextBlock TextBlock { get; } = new();
        private TextStyle? DefaultStyle { get; set; }
        internal HorizontalAlignment? Alignment { get; set; }
        private float Spacing { get; set; } = 0f;

        /// <summary>
        /// Applies a consistent text style for the whole content within this <see cref="TextExtensions.Text">Text</see> element.
        /// </summary>
        /// <param name="style">The TextStyle object to override the default inherited text style.</param>
        public void DefaultTextStyle(TextStyle style)
        {
            DefaultStyle = style;
        }
        
        /// <summary>
        /// Applies a consistent text style for the whole content within this <see cref="TextExtensions.Text">Text</see> element.
        /// </summary>
        /// <param name="handler">Handler to modify the default inherited text style.</param>
        public void DefaultTextStyle(Func<TextStyle, TextStyle> style)
        {
            DefaultStyle = style(TextStyle.Default);
        }
  
        /// <summary>
        /// Aligns text horizontally to the left side.
        /// </summary>
        public void AlignLeft()
        {
            Alignment = HorizontalAlignment.Left;
        }
        
        /// <summary>
        /// Aligns text horizontally to the center, ensuring equal space on both left and right sides.
        /// </summary>
        public void AlignCenter()
        {
            Alignment = HorizontalAlignment.Center;
        }
        
        /// <summary>
        /// Aligns content horizontally to the right side.
        /// </summary>
        public void AlignRight()
        {
            Alignment = HorizontalAlignment.Right;
        }

        /// <summary>
        /// Adjusts the gap between successive paragraphs (separated by line breaks).
        /// </summary>
        public void ParagraphSpacing(float value, Unit unit = Unit.Point)
        {
            Spacing = value.ToPoints(unit);
        }

        [Obsolete("This element has been renamed since version 2022.3. Please use the overload that returns a TextSpanDescriptor object which allows to specify text style.")]
        public void Span(string? text, TextStyle style)
        {
            Span(text).Style(style);
        }
        
        /// <summary>
        /// Appends the given text to the current paragraph.
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.returns.spanDescriptor"]/*' />
        public TextSpanDescriptor Span(string? text)
        {
            if (text == null)
                return new TextSpanDescriptor(new TextBlockSpan());

            var textSpan = new TextBlockSpan() { Text = text };
            TextBlock.Items.Add(textSpan);
            return new TextSpanDescriptor(textSpan);
        }

        /// <summary>
        /// Appends a line with the provided text followed by an environment-specific newline character.
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.returns.spanDescriptor"]/*' />
        public TextSpanDescriptor Line(string? text)
        {
            text ??= string.Empty;
            return Span(text + Environment.NewLine);
        }

        /// <summary>
        /// Appends a blank line.
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.returns.spanDescriptor"]/*' />
        public TextSpanDescriptor EmptyLine()
        {
            return Span(Environment.NewLine);
        }
        
        private TextPageNumberDescriptor PageNumber(Func<IPageContext, int?> pageNumber)
        {
            var textBlockItem = new TextBlockPageNumber();
            TextBlock.Items.Add(textBlockItem);
            
            return new TextPageNumberDescriptor(textBlockItem, x => textBlockItem.Source = context => x(pageNumber(context)));
        }

        /// <summary>
        /// Appends text showing the current page number.
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.returns.pageNumberDescriptor"]/*' />
        public TextPageNumberDescriptor CurrentPageNumber()
        {
            return PageNumber(x => x.CurrentPage);
        }
        
        /// <summary>
        /// Appends text showing the total number of pages in the document.
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.returns.pageNumberDescriptor"]/*' />
        public TextPageNumberDescriptor TotalPages()
        {
            return PageNumber(x => x.DocumentLength);
        }

        [Obsolete("This element has been renamed since version 2022.3. Please use the BeginPageNumberOfSection method.")]
        public void PageNumberOfLocation(string sectionName, TextStyle? style = null)
        {
            BeginPageNumberOfSection(sectionName).Style(style);
        }
        
        /// <summary>
        /// Appends text showing the number of the first page of the specified <see cref="ElementExtensions.Section">Section</see>.
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="param.sectionName"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.returns.pageNumberDescriptor"]/*' />
        public TextPageNumberDescriptor BeginPageNumberOfSection(string sectionName)
        {
            return PageNumber(x => x.GetLocation(sectionName)?.PageStart);
        }
        
        /// <summary>
        /// Appends text showing the number of the last page of the specified <see cref="ElementExtensions.Section">Section</see>.
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="param.sectionName"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.returns.pageNumberDescriptor"]/*' />
        public TextPageNumberDescriptor EndPageNumberOfSection(string sectionName)
        {
            return PageNumber(x => x.GetLocation(sectionName)?.PageEnd);
        }
        
        /// <summary>
        /// Appends text showing the page number relative to the beginning of the given <see cref="ElementExtensions.Section">Section</see>.
        /// </summary>
        /// <example>
        /// For a section spanning pages 20 to 50, page 35 will show as 15.
        /// </example>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="param.sectionName"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.returns.pageNumberDescriptor"]/*' />
        public TextPageNumberDescriptor PageNumberWithinSection(string sectionName)
        {
            return PageNumber(x => x.CurrentPage + 1 - x.GetLocation(sectionName)?.PageStart);
        }
        
        /// <summary>
        /// Appends text showing the total number of pages within the given <see cref="ElementExtensions.Section">Section</see>.
        /// </summary>
        /// <example>
        /// For a section spanning pages 20 to 50, the total is 30 pages.
        /// </example>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="param.sectionName"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.returns.pageNumberDescriptor"]/*' />
        public TextPageNumberDescriptor TotalPagesWithinSection(string sectionName)
        {
            return PageNumber(x => x.GetLocation(sectionName)?.Length);
        }
        
        /// <summary>
        /// Creates a clickable text that navigates the user to a specified <see cref="ElementExtensions.Section">Section</see>.
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="param.sectionName"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.returns.spanDescriptor"]/*' />
        public TextSpanDescriptor SectionLink(string? text, string sectionName)
        {
            if (IsNullOrEmpty(sectionName))
                throw new ArgumentException("Section name cannot be null or empty", nameof(sectionName));

            if (IsNullOrEmpty(text))
                return new TextSpanDescriptor(new TextBlockSpan());

            var textBlockItem = new TextBlockSectionLink
            {
                Text = text,
                SectionName = sectionName
            };

            TextBlock.Items.Add(textBlockItem);
            return new TextSpanDescriptor(textBlockItem);
        }
        
        [Obsolete("This element has been renamed since version 2022.3. Please use the SectionLink method.")]
        public void InternalLocation(string? text, string locationName, TextStyle? style = null)
        {
            SectionLink(text, locationName).Style(style);
        }
        
        /// <summary>
        /// Creates a clickable text that redirects the user to a specific webpage.
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="param.url"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.returns.spanDescriptor"]/*' />
        public TextSpanDescriptor Hyperlink(string? text, string url)
        {
            if (IsNullOrEmpty(url))
                throw new ArgumentException("Url cannot be null or empty", nameof(url));

            if (IsNullOrEmpty(text))
                return new TextSpanDescriptor(new TextBlockSpan());
            
            var textBlockItem = new TextBlockHyperlink
            {
                Text = text,
                Url = url
            };

            TextBlock.Items.Add(textBlockItem);
            return new TextSpanDescriptor(textBlockItem);
        }
        
        [Obsolete("This element has been renamed since version 2022.3. Please use the Hyperlink method.")]
        public void ExternalLocation(string? text, string url, TextStyle? style = null)
        {
            Hyperlink(text, url).Style(style);
        }
        
        /// <summary>
        /// Embeds custom content within the text.
        /// </summary>
        /// <remarks>
        /// The container must fit within one line and can not span multiple pages.
        /// </remarks>
        /// <returns>A container for the embedded content. Populate using the Fluent API.</returns>
        public IContainer Element()
        {
            var container = new Container();
                
            TextBlock.Items.Add(new TextBlockElement
            {
                Element = container
            });
            
            return container.AlignBottom().MinimalBox();
        }
        
        /// <summary>
        /// Embeds custom content within the text.
        /// </summary>
        /// <remarks>
        /// The container must fit within one line and can not span multiple pages.
        /// </remarks>
        /// <param name="handler">Delegate to populate the embedded container with custom content.</param>
        public void Element(Action<IContainer> handler)
        {
            var container = new Container();
            handler(container.AlignBottom().MinimalBox());
                
            TextBlock.Items.Add(new TextBlockElement
            {
                Element = container
            });
        }
        
        internal void Compose(IContainer container)
        {
            TextBlock.Alignment ??= Alignment;
            
            if (DefaultStyle != null)
                container = container.DefaultTextStyle(DefaultStyle);

            container.Element(TextBlock);
        }
    }
    
    public static class TextExtensions
    {
        /// <summary>
        /// Draws rich formatted text.
        /// </summary>
        /// <param name="content">Handler to define the content of the text elements (e.g.: paragraphs, spans, hyperlinks, page numbers).</param>
        public static void Text(this IContainer element, Action<TextDescriptor> content)
        {
            var descriptor = new TextDescriptor();
            
            if (element is Alignment alignment)
                descriptor.Alignment = alignment.Horizontal;
            
            content?.Invoke(descriptor);
            descriptor.Compose(element);
        }
        
        [Obsolete("This method has been deprecated since version 2022.3. Please use the overload that returns a TextSpanDescriptor object which allows to specify text style.")]
        public static void Text(this IContainer element, object? text, TextStyle style)
        {
            element.Text(text).Style(style);
        }

        [Obsolete("This method has been deprecated since version 2022.12. Please use an overload where the text parameter is passed explicitly as a string.")]
        public static TextSpanDescriptor Text(this IContainer element, object? text)
        {
            return element.Text(text?.ToString());
        }

        /// <summary>
        /// Draws the provided text on the page
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="text.returns.spanDescriptor"]/*' />
        public static TextSpanDescriptor Text(this IContainer element, string? text)
        {
            var textDescriptor = new TextDescriptor();
            
            if (element is Alignment alignment)
                textDescriptor.Alignment = alignment.Horizontal;
            
            var span = textDescriptor.Span(text);
            textDescriptor.Compose(element);

            return span;
        }
    }
}