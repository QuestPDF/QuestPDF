using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class PageDescriptor
    {
        internal Page Page { get; } = new Page();

        public IContainer Header()
        {
            var container = new Container();
            Page.Header = container;
            return container;
        }
        
        public IContainer Content()
        {
            var container = new Container();
            Page.Content = container;
            return container;
        }

        public IContainer Footer()
        {
            var container = new Container();
            Page.Footer = container;
            return container;
        }
    }
    
    public static class PageExtensions
    {
        public static void Page(this IContainer document, Action<PageDescriptor> handler)
        {
            var descriptor = new PageDescriptor();
            handler(descriptor);
            document.Component(descriptor.Page);
        }
    }
}