using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Elements.Text.Calculation;
using QuestPDF.Elements.Text.Items;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Text
{
    internal class TextBlock : Element, IStateResettable
    {
        public HorizontalAlignment Alignment { get; set; } = HorizontalAlignment.Left;
        public List<ITextBlockItem> Children { get; set; } = new List<ITextBlockItem>();

        public Queue<ITextBlockItem> RenderingQueue { get; set; }
        public int CurrentElementIndex { get; set; }

        public void ResetState()
        {
            RenderingQueue = new Queue<ITextBlockItem>(Children);
            CurrentElementIndex = 0;
        }

        internal override ISpacePlan Measure(Size availableSpace)
        {
            if (!RenderingQueue.Any())
                return new FullRender(Size.Zero);
            
            var lines = DivideTextItemsIntoLines(availableSpace.Width, availableSpace.Height);

            if (!lines.Any())
                return new PartialRender(Size.Zero);
            
            var width = lines.Max(x => x.Width);
            var height = lines.Sum(x => x.LineHeight);

            if (width > availableSpace.Width + Size.Epsilon || height > availableSpace.Height + Size.Epsilon)
                return new Wrap();

            var fullyRenderedItemsCount = lines
                .SelectMany(x => x.Elements)
                .GroupBy(x => x.Item)
                .Count(x => x.Any(y => y.Measurement.IsLast));
            
            if (fullyRenderedItemsCount == RenderingQueue.Count)
                return new FullRender(width, height);
            
            return new PartialRender(width, height);
        }

        internal override void Draw(Size availableSpace)
        {
            var lines = DivideTextItemsIntoLines(availableSpace.Width, availableSpace.Height).ToList();
            
            if (!lines.Any())
                return;
            
            var heightOffset = 0f;
            var widthOffset = 0f;
            
            foreach (var line in lines)
            {
                widthOffset = 0f;

                var alignmentOffset = GetAlignmentOffset(line.Width);

                Canvas.Translate(new Position(alignmentOffset, 0));
                Canvas.Translate(new Position(0, -line.Ascent));
            
                foreach (var item in line.Elements)
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
                
                    item.Item.Draw(textDrawingRequest);
                
                    Canvas.Translate(new Position(item.Measurement.Width, 0));
                    widthOffset += item.Measurement.Width;
                }
            
                Canvas.Translate(new Position(-alignmentOffset, 0));
                Canvas.Translate(new Position(-line.Width, line.Ascent));
                Canvas.Translate(new Position(0, line.LineHeight));
                
                heightOffset += line.LineHeight;
            }
            
            Canvas.Translate(new Position(0, -heightOffset));
            
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
                if (Alignment == HorizontalAlignment.Left)
                    return 0;

                var emptySpace = availableSpace.Width - lineWidth;

                if (Alignment == HorizontalAlignment.Right)
                    return emptySpace;

                if (Alignment == HorizontalAlignment.Center)
                    return emptySpace / 2;

                throw new ArgumentException();
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
                        IsFirstLineElement = !currentLineElements.Any()
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

                return new TextLine
                {
                    Elements = currentLineElements
                };
            }
        }
    }
}