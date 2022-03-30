using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Elements.Text.Calculation;
using QuestPDF.Infrastructure;
using SkiaSharp.HarfBuzz;
using Size = QuestPDF.Infrastructure.Size;

namespace QuestPDF.Elements.Text.Items
{
    internal class TextBlockSpan : ITextBlockItem
    {
        public ICanvas Canvas { get; set; }
        public IPageContext PageContext { get; set; }
        
        public string Text { get; set; }
        public TextStyle Style { get; set; }
        
        protected TextBlockSize? Size { get; set; }
        protected bool RequiresShaping { get; set; }
        
        public virtual TextBlockSize? Measure()
        {
            Size ??= Text == " " ? GetSizeForSpace() : GetSizeForWord();
            return Size;
        }

        private TextBlockSize GetSizeForWord()
        {
            var paint = Style.ToPaint();
            var fontMetrics = Style.ToFontMetrics();

            var shaper = Style.ToShaper();
            
            // shaper returns positions of all glyphs,
            // by adding a space, it is possible to capture width of the last original character
            var result = shaper.Shape(Text + " ", paint); 
            
            // when text is left-to-right: last value corresponds to text width
            // when text is right-to-left: glyphs are in the reverse order, first value represents text width
            var width = Math.Max(result.Points.First().X, result.Points.Last().X);

            RequiresShaping = result.Points.Length != Text.Length + 1;
            
            return new TextBlockSize
            {
                Width = width,
                
                Ascent = fontMetrics.Ascent,
                Descent = fontMetrics.Descent,
     
                LineHeight = Style.LineHeight ?? 1
            };
        }
        
        private TextBlockSize GetSizeForSpace()
        {
            var paint = Style.ToPaint();
            var fontMetrics = Style.ToFontMetrics();
            
            return new TextBlockSize
            {
                Width = paint.MeasureText(" "),
                
                Ascent = fontMetrics.Ascent,
                Descent = fontMetrics.Descent,
     
                LineHeight = Style.LineHeight ?? 1
            };
        }

        public virtual void Draw(TextDrawingRequest request)
        {
            var fontMetrics = Style.ToFontMetrics();

            Canvas.DrawRectangle(new Position(0, request.TotalAscent), new Size(request.TextSize.Width, request.TextSize.Height), Style.BackgroundColor);

            if (!string.IsNullOrWhiteSpace(Text))
            {
                var offset = GetGlyphOffsetForStyle(Style);

                if (RequiresShaping)
                    Canvas.DrawShapedText(Text, offset, Style);
                else
                    Canvas.DrawText(Text, offset, Style);
            }

            // draw underline
            if ((Style.HasUnderline ?? false) && fontMetrics.UnderlinePosition.HasValue)
                DrawLine(fontMetrics.UnderlinePosition.Value, fontMetrics.UnderlineThickness ?? 1);
            
            // draw stroke
            if ((Style.HasStrikethrough ?? false) && fontMetrics.StrikeoutPosition.HasValue)
                DrawLine(fontMetrics.StrikeoutPosition.Value, fontMetrics.StrikeoutThickness ?? 1);

            void DrawLine(float offset, float thickness)
            {
                Canvas.DrawRectangle(new Position(0, offset - thickness / 2f), new Size(request.TextSize.Width, thickness), Style.Color);
            }
        }

        private static Position GetGlyphOffsetForStyle(TextStyle style)
        {
            if (style.FontVariant == FontVariant.Superscript)
                return new Position(0, (style.Size ?? 12f) * -0.35f);
            if (style.FontVariant == FontVariant.Subscript)
                return new Position(0, (style.Size ?? 12f) * 0.1f);

            return Position.Zero;
        }
    }
}