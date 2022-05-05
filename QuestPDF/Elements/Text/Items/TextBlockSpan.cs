using System;
using System.Collections.Generic;
using QuestPDF.Drawing;
using QuestPDF.Elements.Text.Calculation;
using QuestPDF.Infrastructure;
using Size = QuestPDF.Infrastructure.Size;

namespace QuestPDF.Elements.Text.Items
{
    internal class TextBlockSpan : ITextBlockItem
    {
        private const char Space = ' ';
        
        public string Text { get; set; }
        public TextStyle Style { get; set; } = new();

        private Dictionary<(int startIndex, float availableWidth), TextMeasurementResult?> MeasureCache = new ();

        public virtual TextMeasurementResult? Measure(TextMeasurementRequest request)
        {
            var cacheKey = (request.StartIndex, request.AvailableWidth);
             
            if (!MeasureCache.ContainsKey(cacheKey))
                MeasureCache[cacheKey] = MeasureWithoutCache(request);
            
            return MeasureCache[cacheKey];
        }
        
        internal TextMeasurementResult? MeasureWithoutCache(TextMeasurementRequest request)
        {
            var paint = Style.ToPaint();
            var fontMetrics = Style.ToFontMetrics();

            var startIndex = request.StartIndex;
            
            // if the element is the first one within the line,
            // ignore leading spaces
            if (!request.IsFirstElementInBlock && request.IsFirstElementInLine)
            {
                while (startIndex < Text.Length && Text[startIndex] == Space)
                    startIndex++;
            }
            
            if (Text.Length == 0 || startIndex == Text.Length)
            {
                return new TextMeasurementResult
                {
                    Width = 0,
                    
                    LineHeight = Style.LineHeight ?? 1,
                    Ascent = fontMetrics.Ascent,
                    Descent = fontMetrics.Descent
                };
            }
            
            // start breaking text from requested position
            var text = Text.AsSpan().Slice(startIndex);
            
            var textLength = (int)paint.BreakText(text, request.AvailableWidth + Size.Epsilon);

            if (textLength <= 0)
                return null;
  
            // break text only on spaces
            var wrappedTextLength = WrapText(text, textLength, request.IsFirstElementInLine);

            if (wrappedTextLength == null)
                return null;

            textLength = wrappedTextLength.Value.fragmentLength;

            text = text.Slice(0, textLength);

            var endIndex = startIndex + textLength;

            // measure final text
            var width = paint.MeasureText(text);
            
            return new TextMeasurementResult
            {
                Width = width,
                
                Ascent = fontMetrics.Ascent,
                Descent = fontMetrics.Descent,
     
                LineHeight = Style.LineHeight ?? 1,
                
                StartIndex = startIndex,
                EndIndex = endIndex,
                NextIndex = startIndex + wrappedTextLength.Value.nextIndex,
                TotalIndex = Text.Length
            };
        }
        
        // TODO: consider introduce text wrapping abstraction (basic, english-like, asian-like)
        private (int fragmentLength, int nextIndex)? WrapText(ReadOnlySpan<char> text, int textLength, bool isFirstElementInLine)
        {
            // textLength - length of the part of the text that fits in available width (creating a line)
                
            // entire text fits, no need to wrap
            if (textLength == text.Length)
                return (textLength, textLength + 1);

            // breaking anywhere
            if (Style.WrapAnywhere ?? false)
                return (textLength, textLength);
                
            // current line ends at word, next character is space, perfect place to wrap
            if (text[textLength - 1] != Space && text[textLength] == Space)
                return (textLength, textLength + 1);
                
            // find last space within the available text to wrap
            var lastSpaceIndex = text.Slice(0, textLength).LastIndexOf(Space);

            // text contains space that can be used to wrap
            if (lastSpaceIndex > 0)
                return (lastSpaceIndex, lastSpaceIndex + 1);
                
            // there is no available space to wrap text
            // if the item is first within the line, perform safe mode and chop the word
            // otherwise, move the item into the next line
            return isFirstElementInLine ? (textLength, textLength) : null;
        }
        
        public virtual void Draw(TextDrawingRequest request)
        {
            var fontMetrics = Style.ToFontMetrics();

            var glyphOffsetY = GetVerticalGlyphOffsetForStyle(Style);

            var text = Text.Substring(request.StartIndex, request.EndIndex - request.StartIndex);
            
            request.Canvas.DrawRectangle(new Position(0, request.TotalAscent), new Size(request.TextSize.Width, request.TextSize.Height), Style.BackgroundColor);
            request.Canvas.DrawText(text, new Position(0, glyphOffsetY), Style);

            // draw underline
            if ((Style.HasUnderline ?? false) && fontMetrics.UnderlinePosition.HasValue)
                DrawLine(glyphOffsetY + fontMetrics.UnderlinePosition.Value, fontMetrics.UnderlineThickness ?? 1);
            
            // draw stroke
            if ((Style.HasStrikethrough ?? false) && fontMetrics.StrikeoutPosition.HasValue)
                DrawLine(glyphOffsetY + fontMetrics.StrikeoutPosition.Value, fontMetrics.StrikeoutThickness ?? 1);

            void DrawLine(float offset, float thickness)
            {
                request.Canvas.DrawRectangle(new Position(0, offset - thickness / 2f), new Size(request.TextSize.Width, thickness), Style.Color);
            }
        }

        private static float GetVerticalGlyphOffsetForStyle(TextStyle style)
        {
            if (style.FontPosition == FontPosition.Superscript)
                return (style.Size ?? 12f) * -0.35f;
            if (style.FontPosition == FontPosition.Subscript)
                return (style.Size ?? 12f) * 0.1f;

            return 0;
        }
    }
}