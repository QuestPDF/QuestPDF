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
        internal TextBlock TextBlock { get; } = new();
        private TextStyle? DefaultStyle { get; set; }

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
            TextBlock.Alignment = TextHorizontalAlignment.Left;
        }
        
        /// <summary>
        /// Aligns text horizontally to the center, ensuring equal space on both left and right sides.
        /// </summary>
        public void AlignCenter()
        {
            TextBlock.Alignment = TextHorizontalAlignment.Center;
        }
        
        /// <summary>
        /// Aligns content horizontally to the right side.
        /// </summary>
        public void AlignRight()
        {
            TextBlock.Alignment = TextHorizontalAlignment.Right;
        }
        
        /// <summary>
        /// <para>
        /// Justifies the text within its container.
        /// </para>
        ///
        /// <para>
        /// This method sets the horizontal alignment of the text to be justified, meaning it aligns along both the left and right margins.
        /// This is achieved by adjusting the spacing between words and characters as necessary so that each line of text stretches from one end of the text column to the other.
        /// This creates a clean, block-like appearance for the text.
        /// </para>
        /// </summary>
        public void Justify()
        {
            TextBlock.Alignment = TextHorizontalAlignment.Justify;
        }
        
        /// <summary>
        /// Aligns the text horizontally to the start of the container. 
        /// This method sets the horizontal alignment of the text to the start (left for left-to-right languages, right for right-to-left languages).
        /// </summary>
        public void AlignStart()
        {
            TextBlock.Alignment = TextHorizontalAlignment.Start;
        }
        
        /// <summary>
        /// Aligns the text horizontally to the end of the container. 
        /// This method sets the horizontal alignment of the text to the end (right for left-to-right languages, start for right-to-left languages).
        /// </summary>
        public void AlignEnd()
        {
            TextBlock.Alignment = TextHorizontalAlignment.End;
        }

        /// <summary>
        /// Sets the maximum number of lines to display. 
        /// </summary>
        public void LineClamp(int maxLines)
        {
            TextBlock.LineClamp = maxLines;
        }
        
        [Obsolete("This method is not supported since the 2024.3 version. Please split your text into separate paragraphs, combine using the Column element that also provides the Spacing capability.")]
        public void ParagraphSpacing(float value, Unit unit = Unit.Point)
        {
            
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
            if (IsNullOrEmpty(text))
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
                descriptor.TextBlock.Alignment = MapAlignment(alignment.Horizontal);
            
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
            if (IsNullOrEmpty(text))
                return new TextSpanDescriptor(new TextBlockSpan());
            
            var textDescriptor = new TextDescriptor();
            
            if (element is Alignment alignment)
                textDescriptor.TextBlock.Alignment = MapAlignment(alignment.Horizontal);
            
            var span = textDescriptor.Span(text);
            textDescriptor.Compose(element);

            return span;
        }
        
        private static TextHorizontalAlignment? MapAlignment(HorizontalAlignment? alignment)
        {
            return alignment switch
            {
                HorizontalAlignment.Left => TextHorizontalAlignment.Left,
                HorizontalAlignment.Center => TextHorizontalAlignment.Center,
                HorizontalAlignment.Right => TextHorizontalAlignment.Right,
                _ => null
            };
        }
    }
}