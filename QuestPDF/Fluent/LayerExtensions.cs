using System;
using System.Linq;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class LayersDescriptor
    {
        internal Layers Layers { get; } = new Layers();
        
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

        public IContainer Layer() => Layer(false).RepeatContentWhenPaging();
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
        public static void Layers(this IContainer element, Action<LayersDescriptor> handler)
        {
            var descriptor = new LayersDescriptor();

            handler(descriptor);
            descriptor.Validate();
            
            element.Element(descriptor.Layers);
        }
    }
}