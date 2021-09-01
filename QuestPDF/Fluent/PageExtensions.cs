using System;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class PageDescriptor
    {
        internal Page Page { get; } = new Page();

        public void Size(PageSize pageSize)
        {
            MinSize(pageSize);
            MaxSize(pageSize);
        }
        
        public void ContinuousSize(float width)
        {
            MinSize(new PageSize(width, 0));
            MaxSize(new PageSize(width, Infrastructure.Size.Max.Height));
        }

        public void MinSize(PageSize pageSize)
        {
            Page.MinSize = pageSize;
        }
        
        public void MaxSize(PageSize pageSize)
        {
            Page.MaxSize = pageSize;
        }

        public void MarginLeft(float value)
        {
            Page.MarginLeft = value;
        }
        
        public void MarginRight(float value)
        {
            Page.MarginRight = value;
        }
        
        public void MarginTop(float value)
        {
            Page.MarginTop = value;
        }
        
        public void MarginBottom(float value)
        {
            Page.MarginBottom = value;
        }
        
        public void MarginVertical(float value)
        {
            MarginTop(value);
            MarginBottom(value);
        }
        
        public void MarginHorizontal(float value)
        {
            MarginLeft(value);
            MarginRight(value);
        }
        
        public void Margin(float value)
        {
            MarginVertical(value);
            MarginHorizontal(value);
        }
        
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
        public static IDocumentContainer Page(this IDocumentContainer document, Action<PageDescriptor> handler)
        {
            var descriptor = new PageDescriptor();
            handler(descriptor);
            
            (document as DocumentContainer).Pages.Add(descriptor.Page);
            
            return document;
        }
    }
}