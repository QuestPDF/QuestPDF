using System;
using System.Runtime.CompilerServices;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class ElementExtensions
    {
        static ElementExtensions()
        {
            NativeDependencyCompatibilityChecker.Test();
        }
        
        internal static Container Create(Action<IContainer> factory)
        {
            var container = new Container();
            factory(container);
            return container;
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
        /// <a href="https://www.questpdf.com/api-reference/element.html">Learn more</a>
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
        /// <a href="https://www.questpdf.com/api-reference/element.html">Learn more</a>
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
        
        internal static IContainer NonTrackingElement(this IContainer parent, Func<IContainer, IContainer> handler)
        {
            return handler(parent.Container());
        }
        
        /// <summary>
        /// Constrains its content to maintain a given aspect ratio.
        /// <a href="https://www.questpdf.com/api-reference/aspect-ratio.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// This container enforces strict space constraints. The <see cref="DocumentLayoutException" /> may be thrown if these constraints can't be satisfied.
        /// </remarks>
        /// <param name="ratio">Represents the aspect ratio as a width-to-height division. For instance, a container with a width of 250 points and a height of 200 points has an aspect ratio of 1.25.</param>
        /// <param name="option">Determines the approach the component should adopt when maintaining the specified aspect ratio.</param>
        public static IContainer AspectRatio(this IContainer element, float ratio, AspectRatioOption option = AspectRatioOption.FitWidth)
        {
            return element.Element(new AspectRatio
            {
                Ratio = ratio,
                Option = option
            });
        }

        /// <summary>
        /// Sets a solid background color behind its content.
        /// <a href="https://www.questpdf.com/api-reference/background.html">Learn more</a>
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="colorParam"]/*' />
        public static IContainer Background(this IContainer element, Color color)
        {
            return element.Element(new Background
            {
                Color = color
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
        /// If the container spans multiple pages, its content appears only on the first one.
        /// <a href="https://www.questpdf.com/api-reference/show-once.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// <para>This element is useful if you wish to display a header on every page but want certain elements visible only on the first page.</para>
        /// <para>Depending on the content, certain elements (such as Row or Table) may repeatedly draw their items across multiple pages. Use the ShowOnce element to modify this behavior if it's not desired.</para>
        /// </remarks>
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
        /// <para>Serves as a less-strict approach compared to the <see cref="ElementExtensions.ShowEntire">ShowEntire</see> element.</para>
        /// <para>
        /// It impacts only the very first page of its content's occurence.
        /// If the element fits within its first page, it's rendered as usual.
        /// However, if the element doesn't fit and the available space has less vertical space than required height, the content is entirely shifted to the next page.
        /// </para>
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/ensure-space.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// This is especially useful for elements like tables, where you'd want to display several rows together. By setting the minHeight, you can avoid scenarios where only a single row appears at the page's end, ensuring a more cohesive presentation.
        /// </remarks>
        public static IContainer EnsureSpace(this IContainer element, float minHeight = Elements.EnsureSpace.DefaultMinHeight)
        {
            return element.Element(new EnsureSpace
            {
                MinHeight = minHeight
            });
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
            return element.Element(new Hyperlink
            {
                Url = url
            });
        }
        
        [Obsolete("This element has been renamed since version 2022.3. Please use the Section method.")]
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
            return element
                .DebugPointer(DebugPointerType.Section, sectionName)
                .Element(new Section
                {
                    SectionName = sectionName
                });
        }
        
        [Obsolete("This element has been renamed since version 2022.3. Please use the SectionLink method.")]
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

        #region Canvas [Obsolete]

        private const string CanvasDeprecatedMessage = "The Canvas API has been deprecated since version 2024.3.0. Please use the .Svg(stringContent) API to provide custom content, and consult documentation webpage regarding integrating SkiaSharp with QuestPDF: https://www.questpdf.com/concepts/skia-sharp-integration.html";
        
        [Obsolete(CanvasDeprecatedMessage)]
        public delegate void DrawOnCanvas(object canvas, Size availableSpace);
        
        [Obsolete(CanvasDeprecatedMessage)]
        public static void Canvas(this IContainer element, DrawOnCanvas handler)
        {
            throw new NotImplementedException(CanvasDeprecatedMessage);
        }

        #endregion
    }
}
