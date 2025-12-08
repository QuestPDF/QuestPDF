using System;
using System.Linq;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public sealed class LayersDescriptor
    {
        internal Layers Layers { get; } = new Layers();

        internal LayersDescriptor()
        {
            
        }
        
        private IContainer Layer(bool isPrimary)
        {
            var container = new Container();
            
            var element = new Layer
            {
                IsPrimary = isPrimary,
                Child = container
            };
            
            Layers.Children.Add(element);
            return container;
        }

        /// <summary>
        /// Specifies an additional layer for the container.
        /// </summary>
        /// <remarks>
        /// <para>The order of code execution determines the drawing order:</para>
        /// <para>If the layer is defined before the primary layer, it's drawn underneath the primary content (as a background).</para>
        /// <para>If defined after the primary layer, it's drawn in front of the primary content (as a watermark).</para>
        /// </remarks>
        public IContainer Layer() => Layer(false);
        
        /// <summary>
        /// Sets the primary content for the container. 
        /// </summary>
        /// <remarks>
        /// Exactly one primary layer should be defined.
        /// </remarks>
        public IContainer PrimaryLayer() => Layer(true);

        internal void Validate()
        {
            var primaryLayers = Layers.Children.Count(x => x.IsPrimary);

            if (primaryLayers == 0)
                throw new DocumentComposeException("The Layers component needs to have exactly one primary layer. It has none.");
            
            if (primaryLayers != 1)
                throw new DocumentComposeException($"The Layers component needs to have exactly one primary layer. It has {primaryLayers}.");
        }
    }
    
    public static class LayerExtensions
    {
        /// <summary>
        /// <para>Adds content either underneath (as a background) or on top of (as a watermark) the main content.</para>
        /// <para>The main layer supports paging, can span multiple pages, and determines the container's target length.</para>
        /// <para>Additional layers can also span multiple pages and are repeated on each one.</para>
        /// <a href="https://www.questpdf.com/api-reference/layers.html">Learn more</a>
        /// </summary>
        /// <param name="handler">Handler for defining content of the container, including exactly one primary layer and any additional layers in a specified order.</param>
        public static void Layers(this IContainer element, Action<LayersDescriptor> handler)
        {
            var descriptor = new LayersDescriptor();

            handler(descriptor);
            descriptor.Validate();
            
            element.Element(descriptor.Layers);
        }
    }
}