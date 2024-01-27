using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Elements.Text.Items;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;
using QuestPDF.Skia.Text;

namespace QuestPDF.Elements.Text
{
    internal sealed class TextBlock : Element, IStateResettable, IContentDirectionAware
    {
        public ContentDirection ContentDirection { get; set; }
        
        public HorizontalAlignment? Alignment { get; set; }
        public List<ITextBlockItem> Items { get; set; } = new List<ITextBlockItem>();

        private bool RebuildParagraphForEveryPage { get; set; }
        
        private SkParagraph Paragraph { get; set; }
        private SkSize[] LineMetrics { get; set; }
        private float WidthForLineMetricsCalculation { get; set; }
        private int CurrentLineIndex { get; set; } = 0;
        private float CurrentTopOffset { get; set; } = 0f;
        private SkRect[] PlaceholderPositions { get; set; }
        private float MaximumWidth { get; set; }
        
        public string Text => string.Join(" ", Items.OfType<TextBlockSpan>().Select(x => x.Text));

        ~TextBlock()
        {
            Paragraph?.Dispose();
        }
        
        public void ResetState()
        {
            CurrentLineIndex = 0;
            CurrentTopOffset = 0;
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
            if (Items.Count == 0)
                return SpacePlan.FullRender(Size.Zero);
            
            SetDefaultAlignment();
            Initialize();

            if (Math.Abs(WidthForLineMetricsCalculation - availableSpace.Width) > Size.Epsilon)
            {
                WidthForLineMetricsCalculation = availableSpace.Width;
                Paragraph.PlanLayout(availableSpace.Width);
                LineMetrics = Paragraph.GetLineMetrics();
                PlaceholderPositions = Paragraph.GetPlaceholderPositions();
                MaximumWidth = LineMetrics.Max(x => x.Width);
            }
            
            if (CurrentLineIndex > LineMetrics.Length)
                return SpacePlan.FullRender(Size.Zero);
            
            var totalHeight = 0f;
            var totalLines = 0;
            
            for (var lineIndex = CurrentLineIndex; lineIndex < LineMetrics.Length; lineIndex++)
            {
                var lineMetric = LineMetrics[lineIndex];
                var newTotalHeight = totalHeight + lineMetric.Height;
                
                if (newTotalHeight > availableSpace.Height + Size.Epsilon)
                    break;
                
                totalHeight = newTotalHeight;
                totalLines++;
            }

            if (totalLines == 0)
                return SpacePlan.Wrap();

            var requiredArea = new Size(
                Math.Min(MaximumWidth, availableSpace.Width),
                Math.Min(totalHeight, availableSpace.Height));
            
            if (CurrentLineIndex + totalLines < LineMetrics.Length)
                return SpacePlan.PartialRender(requiredArea);

            return SpacePlan.FullRender(requiredArea);
        }

        internal override void Draw(Size availableSpace)
        {
            if (Items.Count == 0)
                return;
            
            var (linesToDraw, takenHeight) = CalculateDrawingMetrics();
            DrawParagraph();
            
            CurrentLineIndex += linesToDraw;
            CurrentTopOffset += takenHeight;

            if (CurrentLineIndex == LineMetrics.Length)
                ResetState();
            
            return;

            (int linesToDraw, float takenHeight) CalculateDrawingMetrics()
            {
                var linesToDraw = 0;
                var takenHeight = 0f;
                
                for (var lineIndex = CurrentLineIndex; lineIndex < LineMetrics.Length; lineIndex++)
                {
                    var lineMetric = LineMetrics[lineIndex];
                
                    var newTotalHeight = takenHeight + lineMetric.Height;

                    if (newTotalHeight > availableSpace.Height + Size.Epsilon)
                        break;
                    
                    takenHeight = newTotalHeight;
                    linesToDraw++;
                }

                return (linesToDraw, takenHeight);
            }
            
            void DrawParagraph()
            {
                var takesMultiplePages = linesToDraw != LineMetrics.Length;
                
                if (takesMultiplePages)
                {
                    Canvas.Save();
                    Canvas.ClipRectangle(new SkRect(0, 0, availableSpace.Width, takenHeight));
                    Canvas.Translate(new Position(0, -CurrentTopOffset));
                }
                
                Canvas.DrawParagraph(Paragraph);
                DrawInjectedElements();
                DrawHyperlinks();
                DrawSectionLinks();
                
                if (takesMultiplePages)
                    Canvas.Restore();
            }

            void DrawInjectedElements()
            {
                var elementItems = Items.OfType<TextBlockElement>().ToArray();
                
                for (var placeholderIndex = 0; placeholderIndex < PlaceholderPositions.Length; placeholderIndex++)
                {
                    var placeholder = PlaceholderPositions[placeholderIndex];
                    var associatedElement = elementItems[placeholderIndex];

                    var offset = new Position(placeholder.Left, placeholder.Top);
                    
                    if (!IsPositionVisible(offset))
                        continue;
                    
                    Canvas.Translate(offset);
                    associatedElement.Element.Draw(new Size(placeholder.Width, placeholder.Height));
                    Canvas.Translate(offset.Reverse());
                }
            }
            
            void DrawHyperlinks()
            {
                foreach (var hyperlink in Items.OfType<TextBlockHyperlink>())
                {
                    var positions = Paragraph.GetTextRangePositions(hyperlink.ParagraphBeginIndex, hyperlink.ParagraphBeginIndex + hyperlink.Text.Length);
                    
                    foreach (var position in positions)
                    {
                        var offset = new Position(position.Left, position.Top);
                        
                        if (!IsPositionVisible(offset))
                            continue;
                        
                        Canvas.Translate(offset);
                        Canvas.DrawHyperlink(hyperlink.Url, new Size(position.Width, position.Height));
                        Canvas.Translate(offset.Reverse());
                    }
                }
            }
            
            void DrawSectionLinks()
            {
                foreach (var sectionLink in Items.OfType<TextBlockSectionLink>())
                {
                    var positions = Paragraph.GetTextRangePositions(sectionLink.ParagraphBeginIndex, sectionLink.ParagraphBeginIndex + sectionLink.Text.Length);
                    var targetName = PageContext.GetDocumentLocationName(sectionLink.SectionName);
                    
                    foreach (var position in positions)
                    {
                        var offset = new Position(position.Left, position.Top);
                        
                        if (!IsPositionVisible(offset))
                            continue;
                        
                        Canvas.Translate(offset);
                        Canvas.DrawSectionLink(targetName, new Size(position.Width, position.Height));
                        Canvas.Translate(offset.Reverse());
                    }
                }
            }

            bool IsPositionVisible(Position position)
            {
                return CurrentTopOffset <= position.Y || position.Y <= CurrentTopOffset + takenHeight;
            }
        }
        
        private void Initialize()
        {
            if (Paragraph != null && !RebuildParagraphForEveryPage)
                return;
            
            RebuildParagraphForEveryPage = Items.Any(x => x is TextBlockPageNumber);
            BuildParagraph();
        }

        private void BuildParagraph()
        {
            using var paragraphStyle = new SkParagraphStyle(new ParagraphStyleConfiguration
            {
                Alignment = ParagraphStyleConfiguration.TextAlign.Justify,
                Direction = ParagraphStyleConfiguration.TextDirection.Ltr,
                MaxLinesVisible = 1000000,
                Ellipsis = "..."
            });
            
            using var paragraphBuilder = SkParagraphBuilder.Create(paragraphStyle, FontManager.FontCollection);
            var currentTextIndex = 0;
            
            foreach (var textBlockItem in Items)
            {
                if (textBlockItem is TextBlockSpan textBlockSpan)
                {
                    var style = textBlockSpan.Style;
                
                    var textStyle = new SkTextStyle(new TextStyleConfiguration
                    {
                        FontFamilies = new string[8] { style.FontFamily, null, null, null, null, null, null, null },
                        FontSize = style.Size.Value,
                        FontWeight = (TextStyleConfiguration.FontWeights)style.FontWeight.Value,
                        ForegroundColor = style.Color.ColorToCode()
                    });

                    if (textBlockItem is TextBlockSectionLink textBlockSectionLink)
                        textBlockSectionLink.ParagraphBeginIndex = currentTextIndex;

                    else if (textBlockItem is TextBlockHyperlink textBlockHyperlink)
                        textBlockHyperlink.ParagraphBeginIndex = currentTextIndex;

                    else if (textBlockItem is TextBlockPageNumber textBlockPageNumber)
                        textBlockPageNumber.UpdatePageNumberText(PageContext);
                
                    paragraphBuilder.AddText(textBlockSpan.Text, textStyle);
                    currentTextIndex += textBlockSpan.Text.Length;
                }
                else if (textBlockItem is TextBlockElement textBlockElement)
                {
                    textBlockElement.UpdateElementSize(PageContext, Canvas);
                    paragraphBuilder.AddPlaceholder(new SkPlaceholderStyle
                    {
                        Width = textBlockElement.ElementSize.Width,
                        Height = textBlockElement.ElementSize.Height,
                        Alignment = SkPlaceholderStyle.PlaceholderAlignment.AboveBaseline,
                        Baseline = SkPlaceholderStyle.PlaceholderBaseline.Alphabetic,
                        BaselineOffset = 0
                    });
                }
            }

            Paragraph = paragraphBuilder.CreateParagraph();
        }
    }
}