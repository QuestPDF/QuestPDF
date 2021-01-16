using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;
using Size = QuestPDF.Infrastructure.Size;

namespace QuestPDF.Elements
{
    internal class Text : Element
    {
        public string? Value { get; set; }
        public TextStyle? Style { get; set; } = new TextStyle();

        private float LineHeight => Style.Size * Style.LineHeight;

        internal override ISpacePlan Measure(Size availableSpace)
        {
            var lines = BreakLines(availableSpace.Width);
            
            var realWidth = lines
                .Select(line => Style.BreakText(line, availableSpace.Width).FragmentWidth)
                .DefaultIfEmpty(0)
                .Max();
            
            var realHeight = lines.Count * LineHeight;
            
            if (realHeight > availableSpace.Height + Size.Epsilon)
                return new Wrap();
            
            return new FullRender(realWidth, realHeight);
        }

        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            var lines = BreakLines(availableSpace.Width);
            
            var offsetTop = 0f;
            var offsetLeft = GetLeftOffset();

            canvas.Translate(new Position(0, Style.Size));
            
            foreach (var line in lines)
            {
                canvas.DrawText(line, new Position(offsetLeft, offsetTop), Style);
                offsetTop += LineHeight;
            }
            
            canvas.Translate(new Position(0, -Style.Size));

            float GetLeftOffset()
            {
                return Style.Alignment switch
                {
                    HorizontalAlignment.Left => 0,
                    HorizontalAlignment.Center => availableSpace.Width / 2,
                    HorizontalAlignment.Right => availableSpace.Width,
                    _ => throw new NotSupportedException()
                };
            }
        }
        
        #region Word Wrap

        private List<string> BreakLines(float maxWidth)
        {
            var lines = new List<string> ();

            var remainingText = Value.Trim();

            while(true)
            {
                if (string.IsNullOrEmpty(remainingText))
                    break;
                
                var breakPoint = BreakLinePoint(remainingText, maxWidth);
                
                if (breakPoint == 0)
                    break;
                
                var lastLine = remainingText.Substring(0, breakPoint).Trim();
                lines.Add(lastLine);
                
                remainingText = remainingText.Substring(breakPoint).Trim();
            }

            return lines;
        }

        private int BreakLinePoint(string text, float width)
        {
            var index = 0;
            var lengthBreak = Style.BreakText(text, width).LineIndex;
            
            while (index <= text.Length)
            {
                var next = text.IndexOfAny (new [] { ' ', '\n' }, index);
                
                if (next <= 0)
                    return index == 0 || lengthBreak == text.Length ? lengthBreak : index;

                if (next > lengthBreak)
                    return index;

                if (text[next] == '\n')
                    return next;

                index = next + 1;
            }

            return index;
        }

        #endregion
    }
}