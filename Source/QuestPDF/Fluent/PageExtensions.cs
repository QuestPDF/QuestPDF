using System;
using System.Diagnostics.CodeAnalysis;
using QuestPDF.Drawing;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public sealed class PageDescriptor
    {
        internal Page Page { get; } = new Page();

        internal PageDescriptor()
        {
            
        }
        
        #region Size
        
        /// <summary>
        /// Configures the dimensions of every page within the set.
        /// </summary>
        public void Size(float width, float height, Unit unit = Unit.Point)
        {
            var pageSize = new PageSize(width, height, unit);
            
            MinSize(pageSize);
            MaxSize(pageSize);
        }
        
        /// <summary>
        /// Configures the dimensions of every page within the set.
        /// </summary>
        public void Size(PageSize pageSize)
        {
            MinSize(pageSize);
            MaxSize(pageSize);
        }
        
        /// <summary>
        /// Enables the continuous page size mode, allowing the page's height to adjust according to content while retaining a constant specified <paramref name="width" />.
        /// </summary>
        /// <example>
        /// This configuration is suitable for producing PDFs intended for printing on the paper roll.
        /// </example>
        public void ContinuousSize(float width, Unit unit = Unit.Point)
        {
            MinSize(new PageSize(width.ToPoints(unit), 0));
            MaxSize(new PageSize(width.ToPoints(unit), Infrastructure.Size.Max.Height));
        }

        /// <summary>
        /// Enables the flexible page size mode, where the output page's dimensions can vary based on its content.
        /// Specifies the smallest possible page size.
        /// </summary>
        /// <remarks>
        /// Note that with this setting, individual pages within the document may have different sizes.
        /// </remarks>
        public void MinSize(PageSize pageSize)
        {
            Page.MinSize = pageSize;
        }
        
        /// <summary>
        /// Enables the flexible page size mode, where the output page's dimensions can vary based on its content.
        /// Specifies the largest possible page size.
        /// </summary>
        /// <remarks>
        /// Note that with this setting, individual pages within the document may have different sizes.
        /// </remarks>
        public void MaxSize(PageSize pageSize)
        {
            Page.MaxSize = pageSize;
        }
        
        #endregion
        
        #region Margin

        /// <summary>
        /// Adds empty space to the left of the primary layer (header + content + footer).
        /// </summary>
        public void MarginLeft(float value, Unit unit = Unit.Point)
        {
            Page.MarginLeft = value.ToPoints(unit);
        }
        
        /// <summary>
        /// Adds empty space to the right of the primary layer (header + content + footer).
        /// </summary>
        public void MarginRight(float value, Unit unit = Unit.Point)
        {
            Page.MarginRight = value.ToPoints(unit);
        }
        
        /// <summary>
        /// Adds empty space above the primary layer (header + content + footer).
        /// </summary>
        public void MarginTop(float value, Unit unit = Unit.Point)
        {
            Page.MarginTop = value.ToPoints(unit);
        }
        
        /// <summary>
        /// Adds empty space below the primary layer (header + content + footer).
        /// </summary>
        public void MarginBottom(float value, Unit unit = Unit.Point)
        {
            Page.MarginBottom = value.ToPoints(unit);
        }
        
        /// <summary>
        /// Adds empty space vertically (top and bottom) around the primary layer (header + content + footer).
        /// </summary>
        public void MarginVertical(float value, Unit unit = Unit.Point)
        {
            MarginTop(value, unit);
            MarginBottom(value, unit);
        }
        
        /// <summary>
        /// Adds empty space horizontally (left and right) around the primary layer (header + content + footer).
        /// </summary>
        public void MarginHorizontal(float value, Unit unit = Unit.Point)
        {
            MarginLeft(value, unit);
            MarginRight(value, unit);
        }
        
        /// <summary>
        /// Adds empty space around the primary layer (header + content + footer).
        /// </summary>
        public void Margin(float value, Unit unit = Unit.Point)
        {
            MarginVertical(value, unit);
            MarginHorizontal(value, unit);
        }
        
        #endregion
        
        #region Properties

        /// <summary>
        /// Applies a default text style to all <see cref="TextExtensions.Text">Text</see> elements within the page set.
        /// </summary>
        /// <remarks>
        /// Use this method to achieve consistent text styling across entire document.
        /// </remarks>
        /// <param name="handler">A delegate to adjust the global text style attributes.</param>
        public void DefaultTextStyle(TextStyle textStyle)
        {
            Page.DefaultTextStyle = textStyle;
        }
        
        /// <summary>
        /// Applies a default text style to all <see cref="MediaTypeNames.Text">Text</see> elements within the page set.
        /// </summary>
        /// <remarks>
        /// Use this method to achieve consistent text styling across entire document.
        /// </remarks>
        /// <param name="handler">A handler to adjust the global text style attributes.</param>
        public void DefaultTextStyle(Func<TextStyle, TextStyle> handler)
        {
            DefaultTextStyle(handler(TextStyle.Default));
        }
        
        /// <summary>
        /// Applies a left-to-right (LTR) content direction to all elements within the page set.
        /// <a href="https://www.questpdf.com/api-reference/page/settings.html#content-direction">Learn more</a>
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="contentDirection.ltr.remarks"]/*' />
        public void ContentFromLeftToRight()
        {
            Page.ContentDirection = ContentDirection.LeftToRight;
        }
        
        /// <summary>
        /// Applies a right-to-left (RTL) content direction to all elements within the page set.
        /// <a href="https://www.questpdf.com/api-reference/page/settings.html#content-direction">Learn more</a>
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="contentDirection.rtl.remarks"]/*' />
        public void ContentFromRightToLeft()
        {
            Page.ContentDirection = ContentDirection.RightToLeft;
        }
        
        /// <summary>
        /// Sets a background color of the page, which is white by default.
        /// </summary>
        /// <remarks>
        /// When working with file formats that support the alpha channel, it is possible to set the color to <see cref="Colors.Transparent" /> if necessary.
        /// </remarks>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="colorParam"]/*' />
        public void PageColor(Color color)
        {
            Page.BackgroundColor = color;
        }
        
        [Obsolete("This element has been renamed since version 2022.3. Please use the PageColor method.")]
        [ExcludeFromCodeCoverage]
        public void Background(Color color)
        {
            PageColor(color);
        }
        
        #endregion
        
        #region Slots
        
        /// <summary>
        /// Represents a layer drawn behind the primary layer (header + content + footer), serving as a background.
        /// </summary>
        /// <remarks>
        /// Unaffected by the Margin configuration, it always occupies entire page.
        /// </remarks>
        public IContainer Background()
        {
            if (Page.Background is not Empty)
                throw new DocumentComposeException("The 'Page.Background' layer has already been defined. Please call this method only once.");
            
            var container = new Container();
            Page.Background = container;
            return container;
        }
        
        /// <summary>
        /// Represents a layer drawn in front of the primary layer (header + content + footer), serving as a watermark.
        /// </summary>
        /// <remarks>
        /// Unaffected by the Margin configuration, it always occupies entire page.
        /// </remarks>
        public IContainer Foreground()
        {
            if (Page.Foreground is not Empty)
                throw new DocumentComposeException("The 'Page.Foreground' layer has already been defined. Please call this method only once.");
            
            var container = new Container();
            Page.Foreground = container;
            return container;
        }
        
        /// <summary>
        /// Represents the segment at the very top of the page, just above the main content.
        /// </summary>
        /// <remarks>
        /// This container does not support paging capability. It's expected to be fully displayed on every page.
        /// </remarks>
        public IContainer Header()
        {
            if (Page.Header is not Empty)
                throw new DocumentComposeException("The 'Page.Header' layer has already been defined. Please call this method only once.");
            
            var container = new Container();
            Page.Header = container;
            return container;
        }
        
        /// <summary>
        /// Represents the primary content, located in-between the header and footer.
        /// </summary>
        /// <remarks>
        /// This container does support paging capability and determines the final lenght of the document.
        /// </remarks>
        public IContainer Content()
        {
            if (Page.Content is not Empty)
                throw new DocumentComposeException("The 'Page.Content' layer has already been defined. Please call this method only once.");
            
            var container = new Container();
            Page.Content = container;
            return container;
        }
        
        /// <summary>
        /// Represents the section at the very bottom of the page, just below the main content.
        /// </summary>
        /// <remarks>
        /// This container does not support paging capability. It's expected to be fully displayed on every page.
        /// </remarks>
        public IContainer Footer()
        {
            if (Page.Footer is not Empty)
                throw new DocumentComposeException("The 'Page.Footer' layer has already been defined. Please call this method only once.");
            
            var container = new Container();
            Page.Footer = container;
            return container;
        }
        
        #endregion
    }
    
    public static class PageExtensions
    {
        /// <summary>
        /// Creates a new page set with consistent attributes, such as margin, color, and watermark.
        /// The length of each set depends on its content.
        /// </summary>
        /// <remarks>
        /// By leveraging multiple page sets, you can produce documents containing pages of distinct sizes and characteristics.
        /// </remarks>
        /// <param name="handler">Delegate to define page content (layout and visual elements).</param>
        /// <returns>Continuation of the Document API chain, permitting the definition of other page sets.</returns>
        public static IDocumentContainer Page(this IDocumentContainer document, Action<PageDescriptor> handler)
        {
            var descriptor = new PageDescriptor();
            handler(descriptor);
            
            (document as DocumentContainer).Pages.Add(descriptor.Page);
            
            return document;
        }
    }
}