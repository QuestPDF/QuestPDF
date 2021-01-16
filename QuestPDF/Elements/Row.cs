using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Row : Element
    {
        public List<RowElement?>? Children { get; set; } = new List<RowElement?>();
        
        public float? ConstantWidthSum { get; set; }
        public float? RelativeWidthSum { get; set; }

        internal override ISpacePlan Measure(Size availableSpace)
        {
            var sizes = Children
                .Select(x =>
                {
                    var space = GetTargetSize(x, availableSpace);
                    return x.Child.Measure(space);
                })
                .ToList();
            
            if (sizes.Any(x => x is Wrap))
                return new Wrap();

            var height = sizes
                .Where(x => x is Size)
                .Cast<Size>()
                .DefaultIfEmpty(Size.Zero)
                .Max(x => x.Height);
            
            if (sizes.All(x => x is FullRender))
                return new FullRender(availableSpace.Width, height);
            
            if (sizes.Any(x => x is PartialRender))
                return new PartialRender(availableSpace.Width, height);

            return new FullRender(Size.Zero);
        }

        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            var targetSpace = Measure(availableSpace) as Size;

            if (targetSpace == null)
                return;
            
            var offset = 0f;

            foreach (var column in Children)
            { 
                var space = GetTargetSize(column, targetSpace);
                
                canvas.Translate(new Position(offset, 0));
                column.Child.Draw(canvas, space);
                canvas.Translate(new Position(-offset, 0));

                offset += space.Width;
            }
        }

        private Size GetTargetSize(RowElement rowElement, Size availableSpace)
        {
            if (rowElement is ConstantRowElement)
                return new Size(rowElement.Width, availableSpace.Height);

            ConstantWidthSum ??= Children
                .Where(x => x is ConstantRowElement)
                .Cast<ConstantRowElement>()
                .Sum(x => x.Width);
            
            RelativeWidthSum ??= Children
                .Where(x => x is RelativeRowElement)
                .Cast<RelativeRowElement>()
                .Sum(x => x.Width);
            
            return new Size((availableSpace.Width - ConstantWidthSum.Value) * rowElement.Width / RelativeWidthSum.Value, availableSpace.Height);
        }
    }
}