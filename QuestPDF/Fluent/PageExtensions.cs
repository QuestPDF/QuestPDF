using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class PageDescriptor
    {
        private Page Page { get; }
        
        internal PageDescriptor(Page page)
        {
            Page = page;
        }
        
        public IContainer Header()
        {
            var container = new Container();
            Page.Header = container;
            return container;
        }

        public void Header(Action<IContainer> handler)
        {
            handler?.Invoke(Header());
        }
        
        public IContainer Content()
        {
            var container = new Container();
            Page.Content = container;
            return container;
        }

        public void Content(Action<IContainer> handler)
        {
            handler?.Invoke(Content());
        }
        
        public IContainer Footer()
        {
            var container = new Container();
            Page.Footer = container;
            return container;
        }

        public void Footer(Action<IContainer> handler)
        {
            handler?.Invoke(Footer());
        }
    }
    
    public static class PageExtensions
    {
        public static void Page(this IContainer document, Action<PageDescriptor> handler)
        {
            var page = new Page();
            document.Element(page);
            
            var descriptor = new PageDescriptor(page);
            handler(descriptor);
        }
    }
}