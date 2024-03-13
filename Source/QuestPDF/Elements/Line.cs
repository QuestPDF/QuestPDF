using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    public interface ILine
    {
        
    }
    
    internal enum LineType
    {
        Vertical,
        Horizontal,
    }

    internal sealed class Line : Element, ILine, ICacheable
    {
        public LineType Type { get; set; } = LineType.Vertical;
        public string Color { get; set; } = Colors.Black;
        public float Size { get; set; } = 1;
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            if (availableSpace.IsNegative())
                return SpacePlan.Wrap();
            
            return Type switch
            {
                LineType.Vertical when availableSpace.Width + Infrastructure.Size.Epsilon >= Size => SpacePlan.FullRender(Size, 0),
                LineType.Horizontal when availableSpace.Height + Infrastructure.Size.Epsilon >= Size => SpacePlan.FullRender(0, Size),
                _ => SpacePlan.Wrap()
            };
        }

        internal override void Draw(Size availableSpace)
        {
            switch (Type)
            {
                case LineType.Vertical:
                    Canvas.DrawRectangle(new Position(-Size / 2, 0), new Size(Size, availableSpace.Height), Color);
                    break;
                case LineType.Horizontal:
                    Canvas.DrawRectangle(new Position(0, -Size / 2), new Size(availableSpace.Width, Size), Color);
                    break;
            }
        }
    }
}