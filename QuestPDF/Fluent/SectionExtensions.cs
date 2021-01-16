using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class SectionDescriptor
    {
        private Section Section { get; }

        internal SectionDescriptor(Section section)
        {
            Section = section;
        }
        
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
    }
    
    public static class SectionExtensions
    {
        public static void Section(this IContainer element, Action<SectionDescriptor> handler)
        {
            var section = new Section();
            element.Element(section);

            var descriptor = new SectionDescriptor(section);
            handler(descriptor);
        }
    }
}