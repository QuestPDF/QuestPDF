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

        #region Size
        
        public void Size(float width, float height, Unit unit = Unit.Point)
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
        
        #endregion
        
        #region Margin

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
        
        #endregion
        
        #region Properties
        
        public void DefaultTextStyle(TextStyle textStyle)
        {
            Page.DefaultTextStyle = textStyle;
        }
        
        public void DefaultTextStyle(Func<TextStyle, TextStyle> handler)
        {
            DefaultTextStyle(handler(TextStyle.Default));
        }
        
        public void ContentFromLeftToRight()
        {
            Page.ContentDirection = ContentDirection.LeftToRight;
        }
        
        public void ContentFromRightToLeft()
        {
            Page.ContentDirection = ContentDirection.RightToLeft;
        }
        
        public void PageColor(string color)
        {
            ColorValidator.Validate(color);
            Page.BackgroundColor = color;
        }
        
        [Obsolete("This element has been renamed since version 2022.3. Please use the PageColor method.")]
        public void Background(string color)
        {
            PageColor(color);
        }
        
        #endregion
        
        #region Slots
        
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
        
        #endregion
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