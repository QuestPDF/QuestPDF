using System.Collections.Generic;
using QuestPDF.Drawing;
using QuestPDF.Elements.Text.Calculation;
using QuestPDF.Infrastructure;
using Size = QuestPDF.Infrastructure.Size;

namespace QuestPDF.Elements.Text.Items
{
    internal class TextBlockSpan : ITextBlockItem
    {
        public string Text { get; set; }
        public TextStyle Style { get; set; } = new TextStyle();

        private Dictionary<(int startIndex, float availableWidth), TextMeasurementResult?> MeasureCache =
            new Dictionary<(int startIndex, float availableWidth), TextMeasurementResult?>();

        public TextMeasurementResult? Measure(TextMeasurementRequest request)
        {
            var cacheKey = (request.StartIndex, request.AvailableWidth);
            
            if (!MeasureCache.ContainsKey(cacheKey))
                MeasureCache[cacheKey] = MeasureWithoutCache(request);
            
            return MeasureCache[cacheKey];
        }
        
        internal TextMeasurementResult? MeasureWithoutCache(TextMeasurementRequest request)
        {
            const char space = ' ';
            
            var paint = Style.ToPaint();
            var fontMetrics = Style.ToFontMetrics();

            var startIndex = request.StartIndex;
            
            while (startIndex + 1 < Text.Length && Text[startIndex] == space)
                startIndex++;
            
            if (Text.Length == 0)
            {
                return new TextMeasurementResult
                {
                    Width = 0,
                    
                    LineHeight = Style.LineHeight,
                    Ascent = fontMetrics.Ascent,
                    Descent = fontMetrics.Descent
                };
            }
            
            // start breaking text from requested position
            var text = Text.Substring(startIndex);
            
            var breakingIndex = (int)paint.BreakText(text, request.AvailableWidth);

            if (breakingIndex <= 0)
                return null;
            
            // break text only on spaces
            if (breakingIndex < text.Length)
            {
                var lastSpaceIndex = text.Substring(0, breakingIndex).LastIndexOf(space) - 1;

                if (lastSpaceIndex <= 0)
                {
                    if (!request.IsFirstLineElement)
                        return null;
                }
                else
                {
                    breakingIndex = lastSpaceIndex + 1;
                }
            }

            text = text.Substring(0, breakingIndex);

            var endIndex = startIndex + breakingIndex;
            var nextIndex = endIndex;

            while (nextIndex + 1 < Text.Length && Text[nextIndex] == space)
                nextIndex++;
            
            // measure final text
            var width = paint.MeasureText(text);
            
            return new TextMeasurementResult
            {
                Width = width,
                
                Ascent = fontMetrics.Ascent,
                Descent = fontMetrics.Descent,
     
                LineHeight = Style.LineHeight,
                
                StartIndex = startIndex,
                EndIndex = endIndex,
                NextIndex = nextIndex,
                TotalIndex = Text.Length
            };
        }
        
        public void Draw(TextDrawingRequest request)
        {
            var fontMetrics = Style.ToFontMetrics();

            var text = Text.Substring(request.StartIndex, request.EndIndex - request.StartIndex);
            
            request.Canvas.DrawRectangle(new Position(0, request.TotalAscent), new Size(request.TextSize.Width, request.TextSize.Height), Style.BackgroundColor);
            request.Canvas.DrawText(text, Position.Zero, Style);

            // draw underline
            if (Style.IsUnderlined && fontMetrics.UnderlinePosition.HasValue)
                DrawLine(fontMetrics.UnderlinePosition.Value, fontMetrics.UnderlineThickness.Value);
            
            // draw stroke
            if (Style.IsStroked && fontMetrics.StrikeoutPosition.HasValue)
                DrawLine(fontMetrics.StrikeoutPosition.Value, fontMetrics.StrikeoutThickness.Value);

            void DrawLine(float offset, float thickness)
            {
                request.Canvas.DrawRectangle(new Position(0, offset - thickness / 2f), new Size(request.TextSize.Width, thickness), Style.Color);
            }
        }
    }
}