using System;
using System.Linq;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

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
        public IContainer Layer()
        {
            return Layer(false)
                .Artifact(SkSemanticNodeSpecialId.PaginationArtifact)
                .Repeat();
        }
        
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

        /// <summary>
        /// <para>Draws the given <paramref name="backgroundContent" /> underneath the main content.</para>
        /// <para>The Fluent API chain continues with the main content.</para>
        /// <a href="https://www.questpdf.com/api-reference/layers.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// <para>A simplified version of the <see cref="LayerExtensions.Layers">Layers</see> element, limited to exactly two layers.</para>
        /// <para>The main content determines the size of this container and its paging behavior. If it spans multiple pages, the <paramref name="backgroundContent" /> layer is repeated on each of them.</para>
        /// <para>The <paramref name="backgroundContent" /> layer is offered the same space but is neither clipped nor scaled. It is not drawn if it does not fit.</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// container
        ///     .BackgroundLayer(x => x.Image("card-background.jpg"))
        ///     .Element(ComposeBusinessCardText);
        /// </code>
        /// </example>
        /// <param name="backgroundContent">A delegate that populates the layer drawn behind the main content.</param>
        /// <returns>The container for the main content.</returns>
        public static IContainer BackgroundLayer(this IContainer container, Action<IContainer> backgroundContent)
        {
            var result = new Container();

            container.Layers(layers =>
            {
                layers.Layer().Element(backgroundContent);
                layers.PrimaryLayer().Element(result);
            });

            return result;
        }

        /// <summary>
        /// <para>Draws the given <paramref name="foregroundContent" /> on top of the main content, e.g. a watermark, a stamp or a badge.</para>
        /// <para>The Fluent API chain continues with the main content.</para>
        /// <a href="https://www.questpdf.com/api-reference/layers.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// <para>A simplified version of the <see cref="LayerExtensions.Layers">Layers</see> element, limited to exactly two layers.</para>
        /// <para>The main content determines the size of this container and its paging behavior. If it spans multiple pages, the <paramref name="foregroundContent" /> layer is repeated on each of them.</para>
        /// <para>The <paramref name="foregroundContent" /> layer is offered the same space but is neither clipped nor scaled. It is not drawn if it does not fit.</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// page.Content()
        ///     .ForegroundLayer(watermark => watermark
        ///         .AlignCenter().AlignMiddle()
        ///         .Text("DRAFT").FontSize(64).FontColor(Colors.Red.Medium.WithOpacity(0.25f)))
        ///     .Element(ComposeReport);
        /// </code>
        /// </example>
        /// <param name="foregroundContent">A delegate that populates the layer drawn in front of the main content.</param>
        /// <returns>The container for the main content.</returns>
        public static IContainer ForegroundLayer(this IContainer container, Action<IContainer> foregroundContent)
        {
            var result = new Container();

            container.Layers(layers =>
            {
                layers.PrimaryLayer().Element(result);
                layers.Layer().Element(foregroundContent);
            });

            return result;
        }
    }
}