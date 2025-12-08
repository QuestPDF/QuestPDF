using System;
using System.Diagnostics.CodeAnalysis;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public sealed class GridDescriptor
    {
        internal Grid Grid { get; } = new Grid();

        internal GridDescriptor()
        {
            
        }
        
        public void Spacing(float value, Unit unit = Unit.Point)
        {
            VerticalSpacing(value, unit);
            HorizontalSpacing(value, unit);
        }
        
        public void VerticalSpacing(float value, Unit unit = Unit.Point)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "The Grid vertical spacing cannot be negative.");
            
            Grid.VerticalSpacing = value.ToPoints(unit);
        }
         
        public void HorizontalSpacing(float value, Unit unit = Unit.Point)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "The Grid horizontal spacing cannot be negative.");
            
            Grid.HorizontalSpacing = value.ToPoints(unit);
        }
        
        public void Columns(int value = Grid.DefaultColumnsCount)
        {
            if (value < 1)
                throw new ArgumentOutOfRangeException(nameof(value), "The Grid columns count cannot be less than 1.");
            
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
            if (columns < 1)
                throw new ArgumentOutOfRangeException(nameof(columns), "The Grid item cannot span less than 1 column.");
            
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
        [Obsolete("This element has been deprecated since version 2022.11. Please use the Table element, or the combination of the Row and Column elements.")]
        [ExcludeFromCodeCoverage]
        public static void Grid(this IContainer element, Action<GridDescriptor> handler)
        {
            var descriptor = new GridDescriptor();
            
            if (element is Alignment alignment && alignment.Horizontal.HasValue)
                descriptor.Alignment(alignment.Horizontal.Value);
            
            handler(descriptor);
            element.Component(descriptor.Grid);
        }
    }
}