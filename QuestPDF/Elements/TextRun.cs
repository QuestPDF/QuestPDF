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
            var height = measurements.Max(x => x.Position.Bottom) + measurements.Max(x => x.Position.Height);


            return new FullRender(width, height);
        }

        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            var measurements = Elements.Select(x => x.Measure()).ToList();

            var top = measurements.Max(x => x.Position.Height);
            var bottom = measurements.Max(x => x.Position.Bottom);
            
            var offset = measurements.Min(x => x.Position.Top);
            canvas.Translate(new Position(0, -offset));
            
            foreach (var textElement in Elements)
            {
                var size = textElement.Measure();
                
                canvas.DrawRectangle(new Position(0, -top), new Size(size.Width, bottom + top), textElement.Style.BackgroundColor);
                //canvas.DrawRectangle(new Position(size.Position.Left, size.Position.Top), new Size(size.Position.Width, size.Position.Height), textElement.Style.BackgroundColor);
                textElement.Draw(canvas);
                canvas.Translate(new Position(size.Width, 0));
            }
            
            canvas.Translate(new Position(-measurements.Sum(x => x.Width), offset));
        }
    }
}