using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;
using SkiaSharp.HarfBuzz;

namespace QuestPDF.Elements
{
    internal class TextRun : Element
    {
        public List<TextElement> Elements = new List<TextElement>();

        internal override ISpacePlan Measure(Size availableSpace)
        {
            var measurements = Elements.Select(x => x.Measure()).ToList();
            
            var metrics = Elements
                .Select(x => x.Style.ToPaint())
                .Select(x => x.FontMetrics)
                .ToList();
            
            var lineHeight = metrics.Max(x => -x.Ascent) + metrics.Max(x => x.Descent);
            
            var width = measurements.Sum(x => x.Width);

            return new FullRender(width, lineHeight);
        }

        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            var measurements = Elements
                .Select(x => x.Measure())
                .ToList();
            
            var metrics = Elements
                .Select(x => x.Style.ToPaint())
                .Select(x => x.FontMetrics)
                .ToList();

            var offset = metrics.Max(x => -x.Ascent);
            var lineHeight = metrics.Max(x => -x.Ascent) + metrics.Max(x => x.Descent);

            
            using var typeface = SKTypeface.FromFamilyName("Helvetica");
            using var shaper = new SKShaper(typeface);
            var shaped = shaper.Shape("Podstawowy łaciński — ✔️ ❤️ ☆ Tabela znaków Unicode", Elements.First().Style.ToPaint());
            
            foreach (var textElement in Elements)
            {
                var size = textElement.Measure();
                
                canvas.DrawRectangle(new Position(0, 0), new Size(size.Width, lineHeight), textElement.Style.BackgroundColor);

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