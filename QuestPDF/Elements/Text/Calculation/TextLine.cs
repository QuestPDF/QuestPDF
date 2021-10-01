using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Elements.Text.Items;

namespace QuestPDF.Elements.Text.Calculation
{
    internal class TextLine
    {
        public ICollection<TextLineElement> Elements { get; private set; }

        public float TextHeight { get; private set; }
        public float LineHeight { get; private set; }
        
        public float Ascent { get; private set; }
        public float Descent { get; private set; }

        public float Width { get; private set; }
        
        public static TextLine From(ICollection<TextLineElement> elements)
        {
            if (elements.Count == 0)
            {
                return new TextLine
                {
                    Elements = elements
                };
            }
            
            var textHeight = elements.Max(x => x.Measurement.Height);
            var lineHeight = elements.Max(x => x.Measurement.LineHeight * x.Measurement.Height);
            
            return new TextLine
            {
                Elements = elements,
                
                TextHeight = textHeight,
                LineHeight = lineHeight,
                
                Ascent = elements.Min(x => x.Measurement.Ascent) - (lineHeight - textHeight) / 2,
                Descent = elements.Max(x => x.Measurement.Descent) + (lineHeight - textHeight) / 2,
                
                Width = elements.Sum(x => x.Measurement.Width)
            };
        }
    }
}