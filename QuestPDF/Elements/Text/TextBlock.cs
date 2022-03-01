using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using QuestPDF.Drawing;
using QuestPDF.Elements.Text.Calculation;
using QuestPDF.Elements.Text.Items;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Text
{
    internal class TextBlock : Element, IStateResettable
    {
        public HorizontalAlignment Alignment { get; set; } = HorizontalAlignment.Left;
        public List<ITextBlockItem> Items { get; set; } = new();

        public string Text => string.Join(" ", Items.Where(x => x is TextBlockSpan).Cast<TextBlockSpan>().Select(x => x.Text));
        
        private int CurrentElementIndex { get; set; }

        internal override void Initialize(IPageContext pageContext, ICanvas canvas)
        {
            Items = SplitElementsBySpace().ToList();
            
            Items.ForEach(x =>
            {
                x.PageContext = pageContext;
                x.Canvas = canvas;
            });
            
            base.Initialize(pageContext, canvas);
        }
        
        public void ResetState()
        {
            CurrentElementIndex = 0;
        }

        private IEnumerable<ITextBlockItem> SplitElementsBySpace()
        {
            foreach (var item in Items)
            {
                if (item is TextBlockSpan span)
                {
                    span.Text ??= "";
                    
                    foreach (var spanItem in Regex.Split(span.Text, "( )|([^ ]+)"))
                    {
                        yield return new TextBlockSpan
                        {
                            Text = spanItem,
                            Style = span.Style
                        };
                    }
                }
            }
        }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            if (!Items.Any())
                return SpacePlan.FullRender(Size.Zero);

            var lines = DivideTextItemsIntoLines(availableSpace.Width, availableSpace.Height).ToList();

            if (!lines.Any())
                return SpacePlan.Wrap();

            var width = lines.Max(x => x.Width);
            var height = lines.Sum(x => x.LineHeight);

            if (width > availableSpace.Width + Size.Epsilon || height > availableSpace.Height + Size.Epsilon)
                return SpacePlan.Wrap();
            
            if (CurrentElementIndex + lines.Sum(x => x.Elements.Count) == Items.Count)
                return SpacePlan.FullRender(width, height);

            return SpacePlan.PartialRender(width, height);
        }

        internal override void Draw(Size availableSpace)
        {
            var lines = DivideTextItemsIntoLines(availableSpace.Width, availableSpace.Height).ToList();

            if (!lines.Any())
                return;

            var heightOffset = 0f;
            var widthOffset = 0f;

            var isLastPart = lines.Sum(x => x.Elements.Count) + CurrentElementIndex == Items.Count;

            foreach (var sourceLine in lines)
            {
                widthOffset = 0f;

                var isLastLine = sourceLine == lines.Last();

                var line = sourceLine;

                if (!(isLastPart && isLastLine))
                {
                    var spans = sourceLine.Elements
                        .Where(x => x.Item is TextBlockSpan)
                        .SkipWhile(x => string.IsNullOrWhiteSpace((x.Item as TextBlockSpan).Text))
                        .Reverse()
                        .SkipWhile(x => string.IsNullOrWhiteSpace((x.Item as TextBlockSpan).Text))
                        .Reverse()
                        .ToList();

                    var wordsWidth = spans
                        .Where(x => !string.IsNullOrWhiteSpace((x.Item as TextBlockSpan).Text))
                        .Sum(x => x.Measurement.Width);

                    var spaceSpans = spans.Where(x => string.IsNullOrWhiteSpace((x.Item as TextBlockSpan).Text)).ToList();
                    var spaceWidth = (availableSpace.Width - wordsWidth) / spaceSpans.Count;
                    spaceSpans.ForEach(x => x.Measurement.Width = spaceWidth);
                
                    line = TextLine.From(spans);
                }
                
                var alignmentOffset = GetAlignmentOffset(line.Width);

                Canvas.Translate(new Position(alignmentOffset, 0));
                Canvas.Translate(new Position(0, -line.Ascent));

                foreach (var item in line.Elements)
                {
                    var textDrawingRequest = new TextDrawingRequest
                    {
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
            CurrentElementIndex += lines.Sum(x => x.Elements.Count);
            
            if (CurrentElementIndex == Items.Count)
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

        private IEnumerable<TextLine> DivideTextItemsIntoLines(float availableWidth, float availableHeight)
        {
            var currentItemIndex = CurrentElementIndex;
            var currentHeight = 0f;
            
            while (true)
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
                    if (currentItemIndex == Items.Count)
                        break;

                    var currentElement = Items[currentItemIndex];
                    var textBlockSize = currentElement.Measure();

                    if (textBlockSize == null)
                        break;
                    
                    if (currentWidth + textBlockSize.Width > availableWidth + Size.Epsilon)
                        break;

                    currentLineElements.Add(new TextLineElement
                    {
                        Item = currentElement,
                        Measurement = textBlockSize
                    });

                    currentWidth += textBlockSize.Width;
                    currentItemIndex ++;
                }

                return TextLine.From(currentLineElements);
            }
        }
    }
}