using System;
using System.Diagnostics.CodeAnalysis;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public sealed class DecorationDescriptor
    {
        internal Decoration Decoration { get; } = new Decoration();

        internal DecorationDescriptor()
        {
            
        }
        
        /// <summary>
        /// Returns a container for the section positioned before (above) the primary main content.
        /// </summary>
        /// <remarks>
        /// This container is fully visible on each page and does not support paging.
        /// </remarks>
        public IContainer Before()
        {
            if (Decoration.Before is not (Empty or DebugPointer))
                throw new DocumentComposeException("The 'Decoration.Before' layer has already been defined. Please call this method only once.");

            var container = new Container();
            Decoration.Before = container;
            return container.DebugPointer(DebugPointerType.ElementStructure, "Before").Repeat();
        }
        
        /// <summary>
        /// Provides a handler to the section that appears before (above) the main content.
        /// </summary>
        /// <remarks>
        /// This container is fully visible on each page and does not support paging.
        /// </remarks>
        public void Before(Action<IContainer> handler)
        {
            handler?.Invoke(Before());
        }
        
        /// <summary>
        /// Returns a container for the main section.
        /// </summary>
        /// <remarks>
        /// This container does support paging.
        /// </remarks>
        public IContainer Content()
        {
            if (Decoration.Content is not (Empty or DebugPointer))
                throw new DocumentComposeException("The 'Decoration.Content' layer has already been defined. Please call this method only once.");
            
            var container = new Container();
            Decoration.Content = container;
            return container.DebugPointer(DebugPointerType.ElementStructure, "Content");
        }
        
        /// <summary>
        /// Provides a handler to define content of the main section.
        /// </summary>
        /// <remarks>
        /// This container does support paging.
        /// </remarks>
        public void Content(Action<IContainer> handler)
        {
            handler?.Invoke(Content());
        }
        
        /// <summary>
        /// Returns a container for the section positioned after (below) the main content.
        /// </summary>
        /// <remarks>
        /// This container is fully visible on each page and does not support paging.
        /// </remarks>
        public IContainer After()
        {
            if (Decoration.After is not (Empty or DebugPointer))
                throw new DocumentComposeException("The 'Decoration.After' layer has already been defined. Please call this method only once.");
            
            var container = new Container();
            Decoration.After = container;
            return container.DebugPointer(DebugPointerType.ElementStructure, "After").Repeat();
        }
        
        /// <summary>
        /// Provides a handler to the section that appears after (below) the main content.
        /// </summary>
        /// <remarks>
        /// This container is fully visible on each page and does not support paging.
        /// </remarks>
        public void After(Action<IContainer> handler)
        {
            handler?.Invoke(After());
        }

        #region Obsolete

        [Obsolete("This element has been renamed since version 2022.2. Please use the 'Before' method.")]
        [ExcludeFromCodeCoverage]
        public IContainer Header()
        {
            var container = new Container();
            Decoration.Before = container;
            return container;
        }
        
        [Obsolete("This element has been renamed since version 2022.2. Please use the 'Before' method.")]
        [ExcludeFromCodeCoverage]
        public void Header(Action<IContainer> handler)
        {
            handler?.Invoke(Header());
        }
        
        [Obsolete("This element has been renamed since version 2022.2. Please use the 'After' method.")]
        [ExcludeFromCodeCoverage]
        public IContainer Footer()
        {
            var container = new Container();
            Decoration.After = container;
            return container;
        }
        
        [Obsolete("This element has been renamed since version 2022.2. Please use the 'After' method.")]
        [ExcludeFromCodeCoverage]
        public void Footer(Action<IContainer> handler)
        {
            handler?.Invoke(Footer());
        }

        #endregion
    }
    
    public static class DecorationExtensions
    {
        /// <summary>
        /// <para>Divides the container's space into three distinct sections: before, content, and after.</para>
        /// <para>The "before" section is rendered above the main content, while the "after" section is rendered below it.</para>
        /// <para>If the main "content" spans across multiple pages, both the "before" and "after" sections are consistently rendered on every page.</para>
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/decoration.html">Learn more</a>
        /// </summary>
        /// <example>
        /// A typical use-case for this method is to render a table that spans multiple pages, with a consistent caption or header on each page.
        /// </example>
        /// <param name="handler">The action to configure the content.</param>
        public static void Decoration(this IContainer element, Action<DecorationDescriptor> handler)
        {
            var descriptor = new DecorationDescriptor();
            handler(descriptor);
            
            element.Element(descriptor.Decoration);
        }
    }
}