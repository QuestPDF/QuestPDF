using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class GridDescriptor
    {
        internal Grid Grid { get; } = new Grid();
        
        public void Spacing(float value)
        {
            VerticalSpacing(value);
            HorizontalSpacing(value);
        }
        
        public void VerticalSpacing(float value)
        {
            Grid.VerticalSpacing = value;
        }
         
        public void HorizontalSpacing(float value)
        {
            Grid.HorizontalSpacing = value;
        }
        
        public void Columns(int value = Grid.DefaultColumnsCount)
        {
            Grid.ColumnsCount = value;
        }
        
        public void Alignment(HorizontalAlignment alignment)
        {
            Grid.Alignment = alignment;
        }

        public void AlignLeft() => Alignment(HorizontalAlignment.Left);
        public void AlignCenter() => Alignment(HorizontalAlignment.Center);
        public void AlignRight() => Alignment(HorizontalAlignment.Right);
        
        public IContainer Item(int columns = 1)
        {
            var container = new Container();
            
            var element = new GridElement
            {
                Columns = columns,
                Child = container
            };
            
            Grid.Children.Add(element);
            return container;
        }
    }
    
    public static class GridExtensions
    {
        public static void Grid(this IContainer element, Action<GridDescriptor> handler)
        {
            var descriptor = new GridDescriptor();
            
            if (element is Alignment alignment)
                descriptor.Alignment(alignment.Horizontal);
            
            handler(descriptor);
            element.Component(descriptor.Grid);
        }
    }
}