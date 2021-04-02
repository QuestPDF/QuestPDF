using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class SectionDescriptor
    {
        internal Section Section { get; } = new Section();
        
        public IContainer Header()
        {
            var container = new Container();
            Section.Header = container;
            return container;
        }
        
        public void Header(Action<IContainer> handler)
        {
            handler?.Invoke(Header());
        }
        
        public IContainer Content()
        {
            var container = new Container();
            Section.Content = container;
            return container;
        }
        
        public void Content(Action<IContainer> handler)
        {
            handler?.Invoke(Content());
        }
        
        public IContainer Footer()
        {
            var container = new Container();
            Section.Footer = container;
            return container;
        }
        
        public void Footer(Action<IContainer> handler)
        {
            handler?.Invoke(Footer());
        }
    }
    
    public static class SectionExtensions
    {
        public static void Section(this IContainer element, Action<SectionDescriptor> handler)
        {
            var descriptor = new SectionDescriptor();
            handler(descriptor);
            
            element.Component(descriptor.Section);
        }
    }
}