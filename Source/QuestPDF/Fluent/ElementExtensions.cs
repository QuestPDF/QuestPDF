using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Fluent
{
    /// <summary>
    /// Provides extension methods for manipulating and enhancing elements within a container.
    /// </summary>
    public static class ElementExtensions
    {
        static ElementExtensions()
        {
            SkNativeDependencyCompatibilityChecker.Test();
        }
        
        internal static T Element<T>(this IContainer element, T child) where T : IElement
        {
            if (element?.Child != null && element.Child is Empty == false)
            {
                var message = "You should not assign multiple child elements to a single-child container. " +
                              "This may happen when a container variable is used outside of its scope/closure OR the container is used in multiple fluent API chains OR the container is used incorrectly in a loop. " +
                              "This exception is thrown to help you detect that some part of the code is overriding fragments of the document layout with a new content - essentially destroying existing content.";

                throw new DocumentComposeException(message);
            }

            if (element != child as Element)
                element.Child = child as Element;
            
            if (child is Element childElement)
                childElement.CodeLocation = SourceCodePath.CreateFromCurrentStackTrace();

            return child;
        }
        
        /// <summary>
        /// Passes the Fluent API chain to the provided <paramref name="handler"/> method.
        /// <a href="https://www.questpdf.com/concepts/code-patterns/content-styling.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// <para>This method is particularly useful for code refactoring, improving its structure and readability.</para>
        /// <para>Extracting implementation of certain layout structures into separate methods, allows you to accurately describe their purpose and reuse them code in various parts of the application.</para>
        /// </remarks>
        /// <param name="handler">A delegate that takes the current container and populates it with content.</param>
        public static void Element(
            this IContainer parent, 
            Action<IContainer> handler,
            [CallerArgumentExpression("handler")] string handlerName = null,
            [CallerMemberName] string parentName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var handlerContainer = parent
                .Container()
                .Element(new SourceCodePointer
                {
                    MethodName = handlerName,
                    CalledFrom = parentName,
                    FilePath = sourceFilePath,
                    LineNumber = sourceLineNumber
                });
            
            handler(handlerContainer);
        }
        
        /// <summary>
        /// Passes the Fluent API chain to the provided <paramref name="handler"/> method.
        /// <a href="https://www.questpdf.com/concepts/code-patterns/content-styling.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// <para>This method is particularly useful for code refactoring, improving its structure and readability.</para>
        /// <para>Extracting implementation of certain layout structures into separate methods, allows you to accurately describe their purpose and reuse them code in various parts of the application.</para>
        /// </remarks>
        /// <param name="handler">A method that accepts the current container, optionally populates it with content, and returns a subsequent container to continue the Fluent API chain.</param>
        /// <returns>The container returned by the <paramref name="handler"/> method.</returns>
        public static IContainer Element(
            this IContainer parent, 
            Func<IContainer, IContainer> handler,
            [CallerArgumentExpression("handler")] string handlerName = null,
            [CallerMemberName] string parentName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var handlerContainer = parent
                .Element(new SourceCodePointer
                {
                    MethodName = handlerName,
                    CalledFrom = parentName,
                    FilePath = sourceFilePath,
                    LineNumber = sourceLineNumber
                });
            
            return handler(handlerContainer);
        }
        
        public static void Element(this IContainer parent, IContainer child)
        {
            parent.Child = child as IElement;
        }
        
        internal static IContainer NonTrackingElement(this IContainer parent, Func<IContainer, IContainer> handler)
        {
            return handler(parent.Container());
        }
        
        /// <summary>
        /// Constrains its content to maintain a given width-to-height ratio.
        /// <a href="https://www.questpdf.com/api-reference/aspect-ratio.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// This container enforces strict space constraints. The <see cref="DocumentLayoutException" /> may be thrown if these constraints can't be satisfied.
        /// </remarks>
        /// <param name="ratio">Represents the aspect ratio as a width-to-height division. For instance, a container with a width of 250 points and a height of 200 points has an aspect ratio of 1.25.</param>
        /// <param name="option">Determines the approach the component should adopt when maintaining the specified aspect ratio.</param>
        public static IContainer AspectRatio(this IContainer element, float ratio, AspectRatioOption option = AspectRatioOption.FitWidth)
        {
            if (ratio <= 0)
                throw new ArgumentOutOfRangeException(nameof(ratio), "The aspect ratio must be greater than zero.");
            
            return element.Element(new AspectRatio
            {
                Ratio = ratio,
                Option = option
            });
        }

        /// <summary>
        /// Draws a basic placeholder useful for prototyping.
        /// <a href="https://www.questpdf.com/api-reference/placeholder.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// You can control the size of the Placeholder by chaining other elements before its invocation, e.g.:
        /// <code>
        /// .Width(200)
        /// .Height(100)
        /// .Placeholder("Sample text");
        /// </code>
        /// </remarks>
        /// <param name="text">When provided, the placeholder displays this text. If omitted, a simple image icon is shown instead.</param>
        public static void Placeholder(this IContainer element, string? text = null)
        {
            element.Component(new Placeholder
            {
                Text = text ?? string.Empty
            });
        }

        /// <summary>
        /// <para>The ShowOnce element modifies how content is displayed across multiple pages.</para>
        /// <para>
        /// By default, all elements are fully rendered only once and never repeated.
        /// However, in some contexts such as page headers and footers, as well as decoration before and after slots, the content is repeated on every page.
        /// To prevent this, you can use the ShowOnce element.
        /// </para>
        /// <a href="https://www.questpdf.com/api-reference/show-once.html">Learn more</a>
        /// </summary>
        /// <example>
        /// <para>Combine this element with SkipOnce to achieve more complex behaviors, e.g.:</para>
        /// <para><c>.SkipOnce().ShowOnce()</c> ensures the child element is displayed only on the second page.</para>
        /// <para><c>.SkipOnce().SkipOnce()</c> starts displaying the child element from the third page onwards.</para>
        /// <para><c>.ShowOnce().SkipOnce()</c> draws nothing, as the order of invocation is important.</para>
        /// </example>
        public static IContainer ShowOnce(this IContainer element)
        {
            return element.Element(new ShowOnce());
        }

        /// <summary>
        /// If the container spans multiple pages, its content is omitted on the first page and then displayed on the second and subsequent pages.
        /// <a href="https://www.questpdf.com/api-reference/skip-once.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// A common use-case for this element is when displaying a consistent header across pages but needing to conditionally show/hide specific fragments on the first page.
        /// </remarks>
        /// <example>
        /// <para>Combine this element with ShowOnce to achieve more complex behaviors, e.g.:</para>
        /// <para><c>.SkipOnce().ShowOnce()</c> ensures the child element is displayed only on the second page.</para>
        /// <para><c>.SkipOnce().SkipOnce()</c> starts displaying the child element from the third page onwards.</para>
        /// <para><c>.ShowOnce().SkipOnce()</c> draws nothing, as the order of invocation is important.</para>
        /// </example>
        public static IContainer SkipOnce(this IContainer element)
        {
            return element.Element(new SkipOnce());
        }

        /// <summary>
        /// Ensures its content is displayed entirely on a single page by disabling the default paging capability.
        /// <a href="https://www.questpdf.com/api-reference/show-entire.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// <para>While many library elements inherently support paging, allowing content to span multiple pages, this element restricts that behavior.</para>
        /// <para>Employ this when a single-page display is crucial.</para>
        /// <para>Be cautious: its strict space constraints can trigger the <see cref="DocumentLayoutException" /> if content exceeds the page's capacity.</para>
        /// </remarks>
        public static IContainer ShowEntire(this IContainer element)
        {
            return element.Element(new ShowEntire());
        }

        /// <summary>
        /// <para>
        /// Ensures that the container's content occupies at least a specified minimum height on its first page of occurrence.
        /// If there is enough space, the content is rendered as usual. However, if a page break is required,
        /// this method ensures that a minimum amount of space is available before rendering the content.
        /// If the required space is not available, the content is moved to the next page.
        /// </para>
        /// <para>
        /// This rule applies only to the first page where the content appears. If the content spans multiple pages,
        /// all subsequent pages are rendered without this restriction. 
        /// </para>
        /// <para>
        /// This method is particularly useful for structured elements like tables, where rendering only a small fragment 
        /// at the bottom of a page could negatively impact readability. By ensuring a minimum height, 
        /// you can prevent undesired content fragmentation.
        /// </para>
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/ensure-space.html">Learn more</a>
        /// </summary>
        /// <param name="minHeight">The minimum height, in points, that the element should occupy before a page break.</param>
        public static IContainer EnsureSpace(this IContainer element, float minHeight = Elements.EnsureSpace.DefaultMinHeight)
        {
            if (minHeight < 0)
                throw new ArgumentOutOfRangeException(nameof(minHeight), "The EnsureSpace minimum height cannot be negative.");
            
            return element.Element(new EnsureSpace
            {
                MinHeight = minHeight
            });
        }
        
        /// <summary>
        /// <para>
        /// Attempts to keep the container's content together on its first page of occurrence.
        /// If the content does not fit entirely on that page, it is moved to the next page. 
        /// If it spans multiple pages, all subsequent pages are rendered as usual without restriction.
        /// </para>
        /// <para>
        /// This method is useful for ensuring that content remains visually coherent and is not arbitrarily split.
        /// </para>
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/prevent-page-break.html">Learn more</a>
        /// </summary>
        public static IContainer PreventPageBreak(this IContainer element)
        {
            return element.Element(new PreventPageBreak());
        }

        /// <summary>
        /// Inserts a break that pushes the subsequent content to start on a new page.
        /// <a href="https://www.questpdf.com/api-reference/page-break.html">Learn more</a>
        /// </summary>
        public static void PageBreak(this IContainer element)
        {
            element.Element(new PageBreak());
        }
        
        /// <summary>
        /// A neutral layout structure that neither contributes to nor alters its content.
        /// </summary>
        /// <remarks>
        /// By default, certain FluentAPI calls may be batched together for optimized performance. Introduce this element if you wish to separate and prevent such optimizations.
        /// </remarks>
        public static IContainer Container(this IContainer element)
        {
            return element.Element(new Container());
        }
        
        [Obsolete("This element has been renamed since version 2022.3. Please use the Hyperlink method.")]
        [ExcludeFromCodeCoverage]
        public static IContainer ExternalLink(this IContainer element, string url)
        {
            return element.Hyperlink(url);
        }
        
        /// <summary>
        /// Creates a clickable area that redirects the user to a designated webpage.
        /// <a href="https://www.questpdf.com/api-reference/hyperlink.html">Learn more</a>
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="param.url"]/*' />
        public static IContainer Hyperlink(this IContainer element, string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("The URL cannot be null or whitespace.", nameof(url));
            
            return element.Element(new Hyperlink
            {
                Url = url
            });
        }
        
        [Obsolete("This element has been renamed since version 2022.3. Please use the Section method.")]
        [ExcludeFromCodeCoverage]
        public static IContainer Location(this IContainer element, string locationName)
        {
            return element.Section(locationName);
        }
        
        /// <summary>
        /// Defines a named fragment of the document that can span multiple pages.
        /// <a href="https://www.questpdf.com/api-reference/section.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// <para>Several other elements interact with sections:</para>
        /// <para>Use <see cref="ElementExtensions.SectionLink">SectionLink</see> to create a clickable area redirecting user to the first page of the associated section.</para>
        /// <para>The <see cref="TextExtensions.Text(IContainer, Action&lt;TextDescriptor&gt;)">Text</see> element can display section properties, such as the starting page, ending page, and length.</para>
        /// </remarks>
        /// <param name="sectionName">An internal text key representing the section. It should be unique and won't appear in the final document.</param>
        public static IContainer Section(this IContainer element, string sectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
                throw new ArgumentException("The section name cannot be null or whitespace.", nameof(sectionName));
            
            return element
                .DebugPointer(DebugPointerType.Section, sectionName)
                .Element(new Section
                {
                    SectionName = sectionName
                });
        }
        
        [Obsolete("This element has been renamed since version 2022.3. Please use the SectionLink method.")]
        [ExcludeFromCodeCoverage]
        public static IContainer InternalLink(this IContainer element, string locationName)
        {
            return element.SectionLink(locationName);
        }
        
        /// <summary>
        /// Creates a clickable area that navigates the user to a designated section.
        /// <a href="https://www.questpdf.com/api-reference/section-link.html">Learn more</a>
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="param.sectionName"]/*' />
        public static IContainer SectionLink(this IContainer element, string sectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
                throw new ArgumentException("The section name cannot be null or whitespace.", nameof(sectionName));
            
            return element.Element(new SectionLink
            {
                SectionName = sectionName
            });
        }
        
        /// <summary>
        /// Conditionally draws or hides its inner content.
        /// <a href="https://www.questpdf.com/api-reference/show-if.html">Learn more</a>
        /// </summary>
        /// <param name="condition">If the value is <see langword="true"/>, its content is visible. Otherwise, it's hidden.</param>
        public static IContainer ShowIf(this IContainer element, bool condition)
        {
            return condition ? element : new Container();
        }
        
        /// <summary>
        /// Conditionally draws or hides its inner content depending on drawing context.
        /// Please use carefully as certain predicates may produce unstable layouts resulting with unexpected content or exceptions.
        /// <a href="https://www.questpdf.com/api-reference/show-if.html">Learn more</a>
        /// </summary>
        /// <param name="predicate">If the predicate returns <see langword="true"/>, its content is visible. Otherwise, it's hidden.</param>
        public static IContainer ShowIf(this IContainer element, Predicate<ShowIfContext> predicate)
        {
            return element.Element(new ShowIf
            {
                VisibilityPredicate = predicate
            });
        }
        
        /// <summary>
        /// Removes size constraints and grants its content virtually unlimited space.
        /// <a href="https://www.questpdf.com/api-reference/unconstrained.html">Learn more</a>
        /// </summary>
        public static IContainer Unconstrained(this IContainer element)
        {
            return element.Element(new Unconstrained());
        }
        
        /// <summary>
        /// Applies a default text style to all nested <see cref="MediaTypeNames.Text">Text</see> elements.
        /// <a href="https://www.questpdf.com/api-reference/default-text-style.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// If multiple text elements have a similar style, using this element can help simplify your code.
        /// </remarks>
        /// <param name="textStyle">A TextStyle object used to override specific properties.</param>
        public static IContainer DefaultTextStyle(this IContainer element, TextStyle textStyle)
        {
            return element.Element(new DefaultTextStyle
            {
                TextStyle = textStyle
            });
        }
        
        /// <summary>
        /// Applies a default text style to all nested <see cref="MediaTypeNames.Text">Text</see> elements.
        /// <a href="https://www.questpdf.com/api-reference/default-text-style.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// If multiple text elements have a similar style, using this element can help simplify your code.
        /// </remarks>
        /// <param name="handler">A handler to modify the default text style.</param>
        public static IContainer DefaultTextStyle(this IContainer element, Func<TextStyle, TextStyle> handler)
        {
            return element.Element(new DefaultTextStyle
            {
                TextStyle = handler(TextStyle.Default)
            });
        }
        
        /// <summary>
        /// Renders the element exclusively on the first page. Any portion of the element that doesn't fit is omitted.
        /// <a href="https://www.questpdf.com/api-reference/stop-paging.html">Learn more</a>
        /// </summary>
        public static IContainer StopPaging(this IContainer element)
        {
            return element.Element(new StopPaging());
        }
        
        /// <summary>
        /// Adjusts its content to fit within the available space by scaling it down proportionally if necessary.
        /// <a href="https://www.questpdf.com/api-reference/scale-to-fit.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// <para>This container determines the best scale value through multiple scaling operations. With complex content, this may impact performance.</para>
        /// <para>Pairing with certain elements, such as <see cref="ElementExtensions.AspectRatio">AspectRatio</see>, might still lead to a <see cref="DocumentLayoutException" />.</para>
        /// </remarks>
        public static IContainer ScaleToFit(this IContainer element)
        {
            return element.Element(new ScaleToFit());
        }

        /// <summary>
        /// Repeats its content across multiple pages.
        /// <a href="https://www.questpdf.com/api-reference/repeat.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// In certain layout structures, the content visibility may depend on other elements.
        /// By default, most elements are rendered only once.
        /// Use this element to repeat the content across multiple pages.
        /// </remarks>
        public static IContainer Repeat(this IContainer element)
        {
            return element.Element(new RepeatContent());
        }
        
        /// <summary>
        /// <para>
        /// Delays the creation of document content and reduces its lifetime, significantly lowering memory usage in large documents containing thousands of pages. 
        /// This approach also enhances garbage collection efficiency in memory-constrained environments.
        /// </para>
        /// <para>
        /// The provided <paramref name="contentBuilder"/> delegate is invoked later in the document generation process.
        /// For optimal performance, divide your document into smaller sections, each encapsulated within its own Lazy element.
        /// Further optimizations can be achieved by nesting Lazy elements within each other. 
        /// However, note that this technique may increase the overall document generation time due to deferred content processing.
        /// </para>
        /// <a href="https://www.questpdf.com/api-reference/lazy.html">Learn more</a>
        /// </summary>
        public static void Lazy(this IContainer element, Action<IContainer> contentBuilder)
        {
            element.Element(new Lazy
            {
                ContentSource = contentBuilder,
                IsCacheable = false
            });
        }
        
        /// <summary>
        /// Functions similarly to the Lazy element but enables the library to use caching mechanisms for the content.
        /// This can help optimize managed memory usage, although native memory usage may remain high.
        /// Use LazyWithCache only when the increased generation time associated with the Lazy element is unacceptable.
        /// <a href="https://www.questpdf.com/api-reference/lazy.html">Learn more</a>
        /// </summary>
        public static void LazyWithCache(this IContainer element, Action<IContainer> contentBuilder)
        {
            element.Element(new Lazy
            {
                ContentSource = contentBuilder,
                IsCacheable = true
            });
        }
        
        /// <summary>
        /// By default, the library draws content in the order it is defined, which may not always be the desired behavior.
        /// This element allows you to alter the rendering order, ensuring that the content is displayed in the correct sequence.
        /// The default z-index is 0, unless a different value is inherited from a parent container.
        /// <a href="https://www.questpdf.com/api-reference/z-index.html">Learn more</a>
        /// </summary>
        /// <param name="indexValue">The z-index value. Higher values are rendered above lower values.</param>
        public static IContainer ZIndex(this IContainer element, int indexValue)
        {
            return element.Element(new ZIndex
            {
                Depth = indexValue
            });
        }
        
        /// <summary>
        /// Observes the rendering process of its content and captures its position and size on each page.
        /// The captured data can be then used in the Dynamic component to build and position other elements.
        /// <a href="https://www.questpdf.com/concepts/code-patterns/capture-content-position.html">Learn more</a>
        /// </summary>
        public static IContainer CaptureContentPosition(this IContainer element, string id)
        {
            return element.Element(new ElementPositionLocator
            {
                Id = id
            });
        }
        
        #region Canvas [Obsolete]

        private const string CanvasDeprecatedMessage = "The Canvas API has been deprecated since version 2024.3.0. Please use the .Svg(stringContent) API to provide custom content, and consult documentation webpage regarding integrating SkiaSharp with QuestPDF: https://www.questpdf.com/api-reference/skiasharp-integration.html";
        
        [Obsolete(CanvasDeprecatedMessage)]
        public delegate void DrawOnCanvas(object canvas, Size availableSpace);
        
        [Obsolete(CanvasDeprecatedMessage)]
        [ExcludeFromCodeCoverage]
        public static void Canvas(this IContainer element, DrawOnCanvas handler)
        {
            throw new NotImplementedException(CanvasDeprecatedMessage);
        }

        #endregion
    }
}
