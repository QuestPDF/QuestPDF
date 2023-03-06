using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Elements.Text.Calculation;
using QuestPDF.Elements.Text.Items;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Text
{
    internal class TextBlock : Element, IStateResettable, IContentDirectionAware
    {
        public ContentDirection ContentDirection { get; set; }
        
        public HorizontalAlignment? Alignment { get; set; }
        public List<ITextBlockItem> Items { get; set; } = new List<ITextBlockItem>();

        public string Text => string.Join(" ", Items.OfType<TextBlockSpan>().Select(x => x.Text));

        private Queue<ITextBlockItem> RenderingQueue { get; set; }
        private int CurrentElementIndex { get; set; }

        private bool FontFallbackApplied { get; set; } = false;

        public void ResetState()
        {
            ApplyFontFallback();
            InitializeQueue();
            CurrentElementIndex = 0;

            void InitializeQueue()
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (RenderingQueue == null)
                {
                    RenderingQueue = new Queue<ITextBlockItem>(Items);
                    return;
                }
                
                RenderingQueue.Clear();
            
                foreach (var item in Items)
                    RenderingQueue.Enqueue(item);
            }

            void ApplyFontFallback()
            {
                if (FontFallbackApplied)
                    return;

                Items = Items.ApplyFontFallback().ToList();
                FontFallbackApplied = true;
            }
        }
        
        void SetDefaultAlignment()
        {
            if (Alignment.HasValue)
                return;

            Alignment = ContentDirection == ContentDirection.LeftToRight
                ? HorizontalAlignment.Left
                : HorizontalAlignment.Right;
        }

        internal override SpacePlan Measure(Size availableSpace)
        {
            SetDefaultAlignment();
            
            if (!RenderingQueue.Any())
                return SpacePlan.FullRender(Size.Zero);
            
            var lines = DivideTextItemsIntoLines(availableSpace.Width, availableSpace.Height).ToList();

            if (!lines.Any())
                return SpacePlan.Wrap();
            
            var width = lines.Max(x => x.Width);
            var height = lines.Sum(x => x.LineHeight);

            if (width > availableSpace.Width + Size.Epsilon || height > availableSpace.Height + Size.Epsilon)
                return SpacePlan.Wrap();

            var fullyRenderedItemsCount = lines
                .SelectMany(x => x.Elements)
                .GroupBy(x => x.Item)
                .Count(x => x.Any(y => y.Measurement.IsLast));
            
            if (fullyRenderedItemsCount == RenderingQueue.Count)
                return SpacePlan.FullRender(width, height);
            
            return SpacePlan.PartialRender(width, height);
        }

        internal override void Draw(Size availableSpace)
        {
            SetDefaultAlignment();
            
            var lines = DivideTextItemsIntoLines(availableSpace.Width, availableSpace.Height).ToList();
            
            if (!lines.Any())
                return;
            
            var topOffset = 0f;

            foreach (var line in lines)
            {
                var leftOffset = GetAlignmentOffset(line.Width);

                foreach (var item in line.Elements.Where(x => x.Measurement.Width > 0.0))
                {
                    var textDrawingRequest = new TextDrawingRequest
                    {
                        Canvas = Canvas,
                        PageContext = PageContext,
                        
                        StartIndex = item.Measurement.StartIndex,
                        EndIndex = item.Measurement.EndIndex,
                        
                        TextSize = new Size(item.Measurement.Width, line.LineHeight),
                        TotalAscent = line.Ascent
                    };
                
                    var canvasOffset = ContentDirection == ContentDirection.LeftToRight
                        ? new Position(leftOffset, topOffset - line.Ascent)
                        : new Position(availableSpace.Width - leftOffset - item.Measurement.Width, topOffset - line.Ascent);
                    
                    Canvas.Translate(canvasOffset);
                    item.Item.Draw(textDrawingRequest);
                    Canvas.Translate(canvasOffset.Reverse());
                    
                    leftOffset += item.Measurement.Width;
                }
                
                topOffset += line.LineHeight;
            }

            lines
                .SelectMany(x => x.Elements)
                .GroupBy(x => x.Item)
                .Where(x => x.Any(y => y.Measurement.IsLast))
                .Select(x => x.Key)
                .ToList()
                .ForEach(x => RenderingQueue.Dequeue());

            var lastElementMeasurement = lines.Last().Elements.Last().Measurement;
            CurrentElementIndex = lastElementMeasurement.IsLast ? 0 : lastElementMeasurement.NextIndex;
            
            if (!RenderingQueue.Any())
                ResetState();
            
            float GetAlignmentOffset(float lineWidth)
            {
                var emptySpace = availableSpace.Width - lineWidth;

                return Alignment switch
                {
                    HorizontalAlignment.Left => ContentDirection == ContentDirection.LeftToRight ? 0 : emptySpace,
                    HorizontalAlignment.Center => emptySpace / 2,
                    HorizontalAlignment.Right => ContentDirection == ContentDirection.LeftToRight ? emptySpace : 0,
                    _ => 0
                };
            }
        }

        public IEnumerable<TextLine> DivideTextItemsIntoLines(float availableWidth, float availableHeight)
        {
            var queue = new Queue<ITextBlockItem>(RenderingQueue);
            var currentItemIndex = CurrentElementIndex;
            var currentHeight = 0f;

            while (queue.Any())
            {
                var line = GetNextLine();
                
                if (!line.Elements.Any())
                    yield break;
                
                if (currentHeight + line.LineHeight > availableHeight + Size.Epsilon)
                    yield break;

                currentHeight += line.LineHeight;
                yield return line;
            }

            TextLine GetNextLine()
            {
                var currentWidth = 0f;

                var currentLineElements = new List<TextLineElement>();
            
                while (true)
                {
                    if (!queue.Any())
                        break;

                    var currentElement = queue.Peek();
                    
                    var measurementRequest = new TextMeasurementRequest
                    {
                        Canvas = Canvas,
                        PageContext = PageContext,
                        
                        StartIndex = currentItemIndex,
                        AvailableWidth = availableWidth - currentWidth,
                        
                        IsFirstElementInBlock = currentElement == Items.First(),
                        IsFirstElementInLine = !currentLineElements.Any()
                    };
                
                    var measurementResponse = currentElement.Measure(measurementRequest);
                
                    if (measurementResponse == null)
                        break;
                    
                    currentLineElements.Add(new TextLineElement
                    {
                        Item = currentElement,
                        Measurement = measurementResponse
                    });

                    currentWidth += measurementResponse.Width;
                    currentItemIndex = measurementResponse.NextIndex;
                    
                    if (!measurementResponse.IsLast)
                        break;

                    currentItemIndex = 0;
                    queue.Dequeue();
                }

                return TextLine.From(currentLineElements);
            }
        }
    }
}