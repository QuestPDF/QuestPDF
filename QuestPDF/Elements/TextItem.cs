using System;
using System.Drawing;
using System.Runtime.InteropServices;
using QuestPDF.Drawing;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;
using SkiaSharp;
using Size = QuestPDF.Infrastructure.Size;

namespace QuestPDF.Elements
{
    internal class TextMeasurementRequest
    {
        public int StartIndex { get; set; }
        public float AvailableWidth { get; set; }
    }
    
    internal class TextMeasurementResult
    {
        public float Width { get; set; }
        public float Height => Math.Abs(Descent) + Math.Abs(Ascent);
        
        public float Ascent { get; set; }
        public float Descent { get; set; }

        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        
        public int TotalIndex { get; set; }

        public bool HasContent => StartIndex < EndIndex;
        public bool IsLast => EndIndex == TotalIndex;
    }

    public class TextDrawingRequest
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        
        public float TotalAscent { get; set; }
        public Size TextSize { get; set; }
    }
    
    internal class TextItem : Element, IStateResettable
    {
        public string Text { get; set; }

        public TextStyle Style { get; set; } = new TextStyle();
        internal int PointerIndex { get; set; }
        
        public void ResetState()
        {
            PointerIndex = 0;
        }
        
        internal override ISpacePlan Measure(Size availableSpace)
        {
            return new FullRender(Size.Zero);

            // if (VirtualPointer >= Text.Length)
            //     return new FullRender(Size.Zero);
            //
            // var paint = Style.ToPaint();
            // var metrics = paint.FontMetrics;
            //
            // var length = (int)paint.BreakText(Text, availableSpace.Width);
            // length = VirtualPointer + Text.Substring(VirtualPointer, length).LastIndexOf(" ");
            //
            // var textFragment = Text.Substring(VirtualPointer, length);
            //
            // var width = paint.MeasureText(textFragment);
            // var height = Math.Abs(metrics.Descent) + Math.Abs(metrics.Ascent);
            //
            // if (availableSpace.Width < width || availableSpace.Height < height)
            //     return new Wrap();
            //
            // VirtualPointer += length;
            //
            // return new TextRender(width, height)
            // {
            //     Descent = metrics.Descent,
            //     Ascent = metrics.Ascent
            // };
        }

        internal override void Draw(Size availableSpace)
        {
            
        }
        
        internal void Draw(TextDrawingRequest request)
        {
            var fontMetrics = Style.ToPaint().FontMetrics;

            var text = Text.Substring(request.StartIndex, request.EndIndex - request.StartIndex);
            
            Canvas.DrawRectangle(new Position(0, request.TotalAscent), new Size(request.TextSize.Width, request.TextSize.Height), Style.BackgroundColor);
            Canvas.DrawText(text, Position.Zero, Style);

            // draw underline
            if (Style.IsUnderlined && fontMetrics.UnderlinePosition.HasValue)
                DrawLine(fontMetrics.UnderlinePosition.Value, fontMetrics.UnderlineThickness.Value);
            
            // draw stroke
            if (Style.IsStroked && fontMetrics.StrikeoutPosition.HasValue)
                DrawLine(fontMetrics.StrikeoutPosition.Value, fontMetrics.StrikeoutThickness.Value);

            void DrawLine(float offset, float thickness)
            {
                Canvas.DrawRectangle(new Position(0, offset - thickness / 2f), new Size(request.TextSize.Width, thickness), Style.Color);
            }
        }

        internal TextMeasurementResult? MeasureText(TextMeasurementRequest request)
        {
            var paint = Style.ToPaint();
            
            // start breaking text from requested position
            var text = Text.Substring(request.StartIndex);
            var breakingIndex = (int)paint.BreakText(text, request.AvailableWidth);

            if (breakingIndex <= 0)
                return null;
            
            // break text only on spaces
            if (breakingIndex < text.Length)
            {
                breakingIndex = text.Substring(0, breakingIndex).LastIndexOf(" ");

                if (breakingIndex <= 0)
                    return null;
            }

            text = text.Substring(0, breakingIndex);
            
            // measure final text
            var width = paint.MeasureText(text);
            
            return new TextMeasurementResult
            {
                Width = width,
                
                Ascent = paint.FontMetrics.Ascent,
                Descent = paint.FontMetrics.Descent,
     
                StartIndex = request.StartIndex,
                EndIndex = request.StartIndex + breakingIndex,
                TotalIndex = Text.Length
            };
        }
    }
}