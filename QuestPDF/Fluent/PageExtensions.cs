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

        public void Size(float width, float height, Unit unit = Unit.Inch)
        {
            var pageSize = new PageSize(width, height, unit);
            
            MinSize(pageSize);
            MaxSize(pageSize);
        }
        
        public void Size(PageSize pageSize)
        {
            MinSize(pageSize);
            MaxSize(pageSize);
        }
        
        public void ContinuousSize(float width, Unit unit = Unit.Point)
        {
            MinSize(new PageSize(width.ToPoints(unit), 0));
            MaxSize(new PageSize(width.ToPoints(unit), Infrastructure.Size.Max.Height));
        }

        public void MinSize(PageSize pageSize)
        {
            Page.MinSize = pageSize;
        }
        
        public void MaxSize(PageSize pageSize)
        {
            Page.MaxSize = pageSize;
        }

        public void MarginLeft(float value, Unit unit = Unit.Point)
        {
            Page.MarginLeft = value.ToPoints(unit);
        }
        
        public void MarginRight(float value, Unit unit = Unit.Point)
        {
            Page.MarginRight = value.ToPoints(unit);
        }
        
        public void MarginTop(float value, Unit unit = Unit.Point)
        {
            Page.MarginTop = value.ToPoints(unit);
        }
        
        public void MarginBottom(float value, Unit unit = Unit.Point)
        {
            Page.MarginBottom = value.ToPoints(unit);
        }
        
        public void MarginVertical(float value, Unit unit = Unit.Point)
        {
            MarginTop(value, unit);
            MarginBottom(value, unit);
        }
        
        public void MarginHorizontal(float value, Unit unit = Unit.Point)
        {
            MarginLeft(value, unit);
            MarginRight(value, unit);
        }
        
        public void Margin(float value, Unit unit = Unit.Point)
        {
            MarginVertical(value, unit);
            MarginHorizontal(value, unit);
        }
        
        public void DefaultTextStyle(TextStyle textStyle)
        {
            Page.DefaultTextStyle = textStyle;
        }
        
        public void Background(string color)
        {
            Page.BackgroundColor = color;
        }
        
        public IContainer Background()
        {
            var container = new Container();
            Page.Background = container;
            return container;
        }
        
        public IContainer Foreground()
        {
            var container = new Container();
            Page.Foreground = container;
            return container;
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