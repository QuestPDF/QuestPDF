using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Text
{
    internal class TextLineElement
    {
        public ITextElement Element { get; set; }
        public TextMeasurementResult Measurement { get; set; }
    }

    internal class TextLine
    {
        public ICollection<TextLineElement> Elements { get; set; }

        public float TextHeight => Elements.Max(x => x.Measurement.Height);
        public float LineHeight => Elements.Max(x => x.Measurement.LineHeight * x.Measurement.Height);
        
        public float Ascent => Elements.Min(x => x.Measurement.Ascent) - (LineHeight - TextHeight) / 2;
        public float Descent => Elements.Max(x => x.Measurement.Descent) + (LineHeight - TextHeight) / 2;

        public float Width => Elements.Sum(x => x.Measurement.Width);
    }
    
    internal class TextBlock : Element, IStateResettable
    {
        public HorizontalAlignment Alignment { get; set; } = HorizontalAlignment.Left;
        public List<ITextElement> Children { get; set; } = new List<ITextElement>();

        public Queue<ITextElement> RenderingQueue { get; set; }
        public int CurrentElementIndex { get; set; }

        public void ResetState()
        {
            RenderingQueue = new Queue<ITextElement>(Children);
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

            if (width > availableSpace.Width || height > availableSpace.Height)
                return new Wrap();

            var fullyRenderedItemsCount = lines
                .SelectMany(x => x.Elements)
                .GroupBy(x => x.Element)
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

                var emptySpace = availableSpace.Width - line.Width;

                if (Alignment == HorizontalAlignment.Center)
                    emptySpace /= 2f;

                if (Alignment != HorizontalAlignment.Left)
                    Canvas.Translate(new Position(emptySpace, 0));
                
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
                
                    item.Element.Draw(textDrawingRequest);
                
                    Canvas.Translate(new Position(item.Measurement.Width, 0));
                    widthOffset += item.Measurement.Width;
                }
            
                if (Alignment != HorizontalAlignment.Right)
                    Canvas.Translate(new Position(emptySpace, 0));
                
                Canvas.Translate(new Position(-line.Width - emptySpace, line.Ascent));

                Canvas.Translate(new Position(0, line.LineHeight));
                heightOffset += line.LineHeight;
            }
            
            Canvas.Translate(new Position(0, -heightOffset));
            
            lines
                .SelectMany(x => x.Elements)
                .GroupBy(x => x.Element)
                .Where(x => x.Any(y => y.Measurement.IsLast))
                .Select(x => x.Key)
                .ToList()
                .ForEach(x => RenderingQueue.Dequeue());

            var lastElementMeasurement = lines.Last().Elements.Last().Measurement;
            CurrentElementIndex = lastElementMeasurement.IsLast ? 0 : (lastElementMeasurement.EndIndex + 1);
            
            if (!RenderingQueue.Any())
                ResetState();
        }

        public IEnumerable<TextLine> DivideTextItemsIntoLines(float availableWidth, float availableHeight)
        {
            var queue = new Queue<ITextElement>(RenderingQueue);
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
                        AvailableWidth = availableWidth - currentWidth
                    };
                
                    var measurementResponse = currentElement.Measure(measurementRequest);
                
                    if (measurementResponse == null)
                        break;
                    
                    currentLineElements.Add(new TextLineElement
                    {
                        Element = currentElement,
                        Measurement = measurementResponse
                    });

                    currentWidth += measurementResponse.Width;
                    currentItemIndex = measurementResponse.EndIndex;
                    
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