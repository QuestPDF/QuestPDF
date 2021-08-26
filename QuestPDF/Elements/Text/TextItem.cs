using System;
using System.Collections.Generic;
using QuestPDF.Drawing;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;
using Size = QuestPDF.Infrastructure.Size;

namespace QuestPDF.Elements.Text
{
    internal class TextMeasurementRequest
    {
        public ICanvas Canvas { get; set; }
        public IPageContext PageContext { get; set; }
        
        public int StartIndex { get; set; }
        public float AvailableWidth { get; set; }
    }
    
    internal class TextMeasurementResult
    {
        public float Width { get; set; }
        public float Height => Math.Abs(Descent) + Math.Abs(Ascent);

        public float Ascent { get; set; }
        public float Descent { get; set; }

        public float LineHeight { get; set; }
        
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public int TotalIndex { get; set; }

        public bool IsLast => EndIndex == TotalIndex;
    }

    internal class TextDrawingRequest
    {
        public ICanvas Canvas { get; set; }
        public IPageContext PageContext { get; set; }
        
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        
        public float TotalAscent { get; set; }
        public Size TextSize { get; set; }
    }

    internal interface ITextElement
    {
        TextMeasurementResult? Measure(TextMeasurementRequest request);
        void Draw(TextDrawingRequest request);
    }
    
    internal class TextItem : ITextElement
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
            var paint = Style.ToPaint();
            var fontMetrics = Style.ToFontMetrics();
            
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

                breakingIndex += 1;
            }

            text = text.Substring(0, breakingIndex);
            
            // measure final text
            var width = paint.MeasureText(text);
            
            return new TextMeasurementResult
            {
                Width = width,
                
                Ascent = fontMetrics.Ascent,
                Descent = fontMetrics.Descent,
     
                LineHeight = Style.LineHeight,
                
                StartIndex = request.StartIndex,
                EndIndex = request.StartIndex + breakingIndex,
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

    internal class PageNumberTextItem : ITextElement
    {
        public TextStyle Style { get; set; } = new TextStyle();
        public string SlotName { get; set; }
        
        public TextMeasurementResult? Measure(TextMeasurementRequest request)
        {
            return GetItem(request.PageContext).MeasureWithoutCache(request);
        }

        public void Draw(TextDrawingRequest request)
        {
            GetItem(request.PageContext).Draw(request);
        }

        private TextItem GetItem(IPageContext context)
        {
            var pageNumberPlaceholder = 123;
            
            var pageNumber = context.GetRegisteredLocations().Contains(SlotName)
                ? context.GetLocationPage(SlotName)
                : pageNumberPlaceholder;
            
            return new TextItem
            {
                Style = Style,
                Text = pageNumber.ToString()
            };
        }
    }

    internal class InternalLinkTextItem : ITextElement
    {
        public TextStyle Style { get; set; } = new TextStyle();
        public string Text { get; set; }
        public string LocationName { get; set; }
        
        public TextMeasurementResult? Measure(TextMeasurementRequest request)
        {
            return GetItem().MeasureWithoutCache(request);
        }

        public void Draw(TextDrawingRequest request)
        {
            request.Canvas.Translate(new Position(0, request.TotalAscent));
            request.Canvas.DrawLocationLink(LocationName, new Size(request.TextSize.Width, request.TextSize.Height));
            request.Canvas.Translate(new Position(0, -request.TotalAscent));
            
            GetItem().Draw(request);
        }

        private TextItem GetItem()
        {
            return new TextItem
            {
                Style = Style,
                Text = Text
            };
        }
    }
    
    internal class ExternalLinkTextItem : ITextElement
    {
        public TextStyle Style { get; set; } = new TextStyle();
        public string Text { get; set; }
        public string Url { get; set; }
        
        public TextMeasurementResult? Measure(TextMeasurementRequest request)
        {
            return GetItem().MeasureWithoutCache(request);
        }

        public void Draw(TextDrawingRequest request)
        {
            request.Canvas.Translate(new Position(0, request.TotalAscent));
            request.Canvas.DrawExternalLink(Url, new Size(request.TextSize.Width, request.TextSize.Height));
            request.Canvas.Translate(new Position(0, -request.TotalAscent));
            
            GetItem().Draw(request);
        }

        private TextItem GetItem()
        {
            return new TextItem
            {
                Style = Style,
                Text = Text
            };
        }
    }
    
    internal class ElementTextItem : ITextElement
    {
        public Element Element { get; set; } = Empty.Instance;
        
        public TextMeasurementResult? Measure(TextMeasurementRequest request)
        {
            Element.HandleVisitor(x => (x as IStateResettable)?.ResetState());
            Element.HandleVisitor(x => x.Initialize(request.PageContext, request.Canvas));

            var measurement = Element.Measure(new Size(request.AvailableWidth, Size.Max.Height));

            if (measurement is Wrap || measurement is PartialRender)
                return null;

            var elementSize = measurement as Size;
            
            return new TextMeasurementResult
            {
                Width = elementSize.Width,
                
                Ascent = -elementSize.Height,
                Descent = 0,
                
                LineHeight = 1,
                
                StartIndex = 0,
                EndIndex = 0,
                TotalIndex = 0
            };
        }

        public void Draw(TextDrawingRequest request)
        {
            Element.HandleVisitor(x => (x as IStateResettable)?.ResetState());
            Element.HandleVisitor(x => x.Initialize(request.PageContext, request.Canvas));
            
            request.Canvas.Translate(new Position(0, request.TotalAscent));
            Element.Draw(new Size(request.TextSize.Width, -request.TotalAscent));
            request.Canvas.Translate(new Position(0, -request.TotalAscent));
        }
    }
}