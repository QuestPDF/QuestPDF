using System.Collections.Concurrent;
using System.Collections.Generic;
using QuestPDF.Drawing;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Elements
{
    public struct TextMeasurement
    {
        public float Width { get; set; }
        public SKRect Position { get; set; }
    }
    
    internal class TextElement
    {
        public TextStyle Style { get; set; }
        public string Text { get; set; }


        private static ConcurrentDictionary<string, TextMeasurement> Measurements = new ConcurrentDictionary<string, TextMeasurement>();
        
        public TextMeasurement Measure()
        {
            return Measure(Text, Style);
        }

        public void Draw(ICanvas canvas)
        {
            (canvas as QuestPDF.Drawing.Canvas).SkiaCanvas.DrawText(Text, 0, 0, Style.ToPaint());
        }

        internal static TextMeasurement Measure(string text, TextStyle style)
        {
            var key = $"{text}_{style}";
            
            return Measurements.GetOrAdd(key, x =>
            {
                var rect = new SKRect();
                var width = style.ToPaint().MeasureText(text, ref rect);
                
                return new TextMeasurement
                {
                    Position = rect,
                    Width = width
                };
            });
        }
    }
}