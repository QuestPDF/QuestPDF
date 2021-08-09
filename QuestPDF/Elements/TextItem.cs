using System;
using QuestPDF.Drawing;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class TextItem : Element
    {
        public string Value { get; set; }
        public TextStyle Style { get; set; } = new TextStyle();
        
        internal override ISpacePlan Measure(Size availableSpace)
        {
            var paint = Style.ToPaint();
            var metrics = paint.FontMetrics;

            var width = paint.MeasureText(Value);
            var height = Math.Abs(metrics.Descent) + Math.Abs(metrics.Ascent);

            if (availableSpace.Width < width || availableSpace.Height < height)
                return new Wrap();
            
            return new TextRender(width, height)
            {
                Descent = metrics.Descent,
                Ascent = metrics.Ascent
            };
        }

        internal override void Draw(Size availableSpace)
        {
            var paint = Style.ToPaint();
            var metrics = paint.FontMetrics;
            
            var size = Measure(availableSpace) as Size;
            
            if (size == null)
                return;

            Canvas.DrawRectangle(new Position(0, metrics.Ascent), new Size(size.Width, size.Height), Style.BackgroundColor);
            Canvas.DrawText(Value, Position.Zero, Style);

            // draw underline
            if (Style.IsUnderlined && metrics.UnderlinePosition.HasValue)
                DrawLine(metrics.UnderlinePosition.Value, metrics.UnderlineThickness.Value);
            
            // draw stroke
            if (Style.IsStroked && metrics.StrikeoutPosition.HasValue)
                DrawLine(metrics.StrikeoutPosition.Value, metrics.StrikeoutThickness.Value);

            void DrawLine(float offset, float thickness)
            {
                Canvas.DrawRectangle(new Position(0, offset - thickness / 2f), new Size(size.Width, thickness), Style.Color);
            }
        }
    }
}