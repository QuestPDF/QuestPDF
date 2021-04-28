using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class TextRun : Element
    {
        public List<TextElement> Elements = new List<TextElement>();

        internal override ISpacePlan Measure(Size availableSpace)
        {
            var measurements = Elements.Select(x => x.Measure()).ToList();
            
            var width = measurements.Sum(x => x.Width);
            //var height = measurements.Max(x => x.Height);
            //var height = measurements.Max(x => x.Position.Bottom) + measurements.Max(x => x.Position.Height);
            var height = Elements.Max(x => x.Style.Size * x.Style.LineHeight);
    
            return new FullRender(width, height);
        }

        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            var measurements = Elements.Select(x => x.Measure()).ToList();

            var lineHeight = Elements.Max(x => x.Style.Size * x.Style.LineHeight);
            var textHeight = Elements.Max(x => x.Style.Size);
            
            var lineHeightOffset = (lineHeight - textHeight) / 2;
            var baselineOffset = measurements.Max(x => -x.Position.Top);
            var offset = lineHeightOffset + baselineOffset;
            
            
            foreach (var textElement in Elements)
            {
                var size = textElement.Measure();
                
                //canvas.DrawRectangle(new Position(0, 0), new Size(size.Width, lineHeight), textElement.Style.BackgroundColor);
                //canvas.DrawRectangle(new Position(size.Position.Left, size.Position.Top), new Size(size.Position.Width, size.Position.Height), textElement.Style.BackgroundColor);
                
                canvas.Translate(new Position(0, offset));
                textElement.Draw(canvas);
                canvas.Translate(new Position(0, -offset));
                
                canvas.Translate(new Position(size.Width, 0));
            }
            
            canvas.Translate(new Position(-measurements.Sum(x => x.Width), 0));
            
            
            
            //
        }
    }
}