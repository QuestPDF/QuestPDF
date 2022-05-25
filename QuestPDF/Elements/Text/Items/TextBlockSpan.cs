using System;
using System.Collections.Generic;
using System.Diagnostics;
using QuestPDF.Drawing;
using QuestPDF.Elements.Text.Calculation;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;
using SkiaSharp.HarfBuzz;
using Size = QuestPDF.Infrastructure.Size;

namespace QuestPDF.Elements.Text.Items
{
    internal class TextBlockSpan : ITextBlockItem
    {
        public string Text { get; set; }
        public TextStyle Style { get; set; } = new();
        public TextShapingResult? TextShapingResult { get; set; }

        private Dictionary<(int startIndex, float availableWidth), TextMeasurementResult?> MeasureCache = new ();
        protected virtual bool EnableTextCache => true; 

        public virtual TextMeasurementResult? Measure(TextMeasurementRequest request)
        {
            var cacheKey = (request.StartIndex, request.AvailableWidth);
             
            if (!MeasureCache.ContainsKey(cacheKey))
                MeasureCache[cacheKey] = MeasureWithoutCache(request);
            
            return MeasureCache[cacheKey];
        }

        internal TextMeasurementResult? MeasureWithoutCache(TextMeasurementRequest request)
        {
            if (!EnableTextCache)
                TextShapingResult = null;
            
            TextShapingResult ??= Style.ToTextShaper().Shape(Text);
            
            var paint = Style.ToPaint();
            var fontMetrics = Style.ToFontMetrics();
            var spaceCodepoint = paint.ToFont().Typeface.GetGlyphs(" ")[0];

            var startIndex = request.StartIndex;
            
            // if the element is the first one within the line,
            // ignore leading spaces
            if (!request.IsFirstElementInBlock && request.IsFirstElementInLine)
            {
                while (startIndex < TextShapingResult.Glyphs.Length && Text[startIndex] == spaceCodepoint)
                    startIndex++;
            }

            if (TextShapingResult.Glyphs.Length == 0 || startIndex == TextShapingResult.Glyphs.Length)
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
            var endIndex = TextShapingResult.BreakText(startIndex, request.AvailableWidth);

            if (endIndex < 0)
                return null;
  
            // break text only on spaces
            var wrappedText = WrapText(startIndex, endIndex, request.IsFirstElementInLine);

            if (wrappedText == null)
                return null;
            
            // measure final text
            var width = TextShapingResult.MeasureWidth(startIndex, wrappedText.Value.endIndex);
            
            return new TextMeasurementResult
            {
                Width = width,
                
                Ascent = fontMetrics.Ascent,
                Descent = fontMetrics.Descent,
     
                LineHeight = Style.LineHeight ?? 1,
                
                StartIndex = startIndex,
                EndIndex = wrappedText.Value.endIndex,
                NextIndex = wrappedText.Value.nextIndex,
                TotalIndex = TextShapingResult.Glyphs.Length - 1
            };
        }
        
        // TODO: consider introducing text wrapping abstraction (basic, english-like, asian-like)
        private (int endIndex, int nextIndex)? WrapText(int startIndex, int endIndex, bool isFirstElementInLine)
        {
            var spaceCodepoint = Style.ToPaint().ToFont().Typeface.GetGlyphs(" ")[0];
            
            // textLength - length of the part of the text that fits in available width (creating a line)

            // entire text fits, no need to wrap
            if (endIndex == TextShapingResult.Glyphs.Length - 1)
                return (endIndex, endIndex);

            // breaking anywhere
            if (Style.WrapAnywhere ?? false)
                return (endIndex, endIndex + 1);
                
            // current line ends at word, next character is space, perfect place to wrap
            if (TextShapingResult.Glyphs[endIndex].Codepoint != spaceCodepoint && TextShapingResult.Glyphs[endIndex + 1].Codepoint == spaceCodepoint)
                return (endIndex, endIndex + 2);
                
            // find last space within the available text to wrap
            var lastSpaceIndex = endIndex;

            while (lastSpaceIndex >= startIndex)
            {
                if (TextShapingResult.Glyphs[lastSpaceIndex].Codepoint == spaceCodepoint)
                    break;

                lastSpaceIndex--;
            }

            // text contains space that can be used to wrap
            if (lastSpaceIndex >= startIndex)
                return (lastSpaceIndex - 1, lastSpaceIndex + 1);
                
            // there is no available space to wrap text
            // if the item is first within the line, perform safe mode and chop the word
            // otherwise, move the item into the next line
            return isFirstElementInLine ? (endIndex, endIndex + 1) : null;
        }
        
        public virtual void Draw(TextDrawingRequest request)
        {
            var fontMetrics = Style.ToFontMetrics();

            var glyphOffsetY = GetGlyphOffset();
            
            var textDrawingCommand = TextShapingResult.PositionText(request.StartIndex, request.EndIndex, Style);

            if (Style.BackgroundColor != Colors.Transparent)
                request.Canvas.DrawRectangle(new Position(0, request.TotalAscent), new Size(request.TextSize.Width, request.TextSize.Height), Style.BackgroundColor);
            
            if (textDrawingCommand.HasValue)
                request.Canvas.DrawText(textDrawingCommand.Value.SkTextBlob, new Position(textDrawingCommand.Value.TextOffsetX, glyphOffsetY), Style);

            // draw underline
            if ((Style.HasUnderline ?? false) && fontMetrics.UnderlinePosition.HasValue)
            {
                var underlineOffset = Style.FontPosition == FontPosition.Superscript ? 0 : glyphOffsetY;
                DrawLine(fontMetrics.UnderlinePosition.Value + underlineOffset, fontMetrics.UnderlineThickness ?? 1);
            }
            
            // draw stroke
            if ((Style.HasStrikethrough ?? false) && fontMetrics.StrikeoutPosition.HasValue)
            {
                var strikeoutThickness = fontMetrics.StrikeoutThickness ?? 1;
                strikeoutThickness *= Style.FontPosition == FontPosition.Normal ? 1f : 0.625f;
                
                DrawLine(fontMetrics.StrikeoutPosition.Value + glyphOffsetY, strikeoutThickness);
            }
            
            void DrawLine(float offset, float thickness)
            {
                request.Canvas.DrawRectangle(new Position(0, offset), new Size(request.TextSize.Width, thickness), Style.Color);
            }

            float GetGlyphOffset()
            {
                var fontSize = Style.Size ?? 12f;

                var offsetFactor = Style.FontPosition switch
                {
                    FontPosition.Normal => 0,
                    FontPosition.Subscript => 0.1f,
                    FontPosition.Superscript => -0.35f,
                    _ => throw new ArgumentOutOfRangeException()
                };

                return fontSize * offsetFactor;
            }
        }
    }
}