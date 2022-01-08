using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class DecorationDescriptor
    {
        internal Decoration Decoration { get; } = new Decoration();
        
        public IContainer Header()
        {
            var container = new Container();
            Decoration.Header = container;
            return container;
        }
        
        public void Header(Action<IContainer> handler)
        {
            handler?.Invoke(Header());
        }
        
        public IContainer Content()
        {
            var container = new Container();
            Decoration.Content = container;
            return container;
        }
        
        public void Content(Action<IContainer> handler)
        {
            handler?.Invoke(Content());
        }
        
        public IContainer Footer()
        {
            var container = new Container();
            Decoration.Footer = container;
            return container;
        }
        
        public void Footer(Action<IContainer> handler)
        {
            handler?.Invoke(Footer());
        }
    }
    
    public static class DecorationExtensions
    {
        public static void Decoration(this IContainer element, Action<DecorationDescriptor> handler)
        {
            var descriptor = new DecorationDescriptor();
            handler(descriptor);
            
            element.Element(descriptor.Decoration);
        }
    }
}