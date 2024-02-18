using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Drawing.Exceptions;
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
        
        public TextHorizontalAlignment? Alignment { get; set; }
        public int? LineClamp { get; set; }
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
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            if (Items.Count == 0)
                return SpacePlan.FullRender(Size.Zero);
            
            Initialize();
            CalculateParagraphMetrics(availableSpace);

            if (MaximumWidth == 0)
                return SpacePlan.FullRender(Size.Zero);
            
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
            
            CalculateParagraphMetrics(availableSpace);

            if (MaximumWidth == 0)
                return;
            
            var (linesToDraw, takenHeight) = DetermineLinesToDraw();
            DrawParagraph();
            
            CurrentLineIndex += linesToDraw;
            CurrentTopOffset += takenHeight;

            if (CurrentLineIndex == LineMetrics.Length)
                ResetState();
            
            return;

            (int linesToDraw, float takenHeight) DetermineLinesToDraw()
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
                    
                    associatedElement.ConfigureElement(PageContext, Canvas);

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
            var paragraphStyle = new ParagraphStyleConfiguration
            {
                Alignment = MapAlignment(Alignment ?? TextHorizontalAlignment.Start),
                Direction = MapDirection(ContentDirection),
                MaxLinesVisible = LineClamp ?? 1_000_000
            };
            
            using var paragraphBuilder = SkParagraphBuilder.Create(paragraphStyle, FontManager.FontCollection);
            var currentTextIndex = 0;
            
            foreach (var textBlockItem in Items)
            {
                if (textBlockItem is TextBlockSpan textBlockSpan)
                {
                    if (textBlockItem is TextBlockSectionLink textBlockSectionLink)
                        textBlockSectionLink.ParagraphBeginIndex = currentTextIndex;

                    else if (textBlockItem is TextBlockHyperlink textBlockHyperlink)
                        textBlockHyperlink.ParagraphBeginIndex = currentTextIndex;

                    else if (textBlockItem is TextBlockPageNumber textBlockPageNumber)
                        textBlockPageNumber.UpdatePageNumberText(PageContext);
                
                    var textStyle = textBlockSpan.Style.GetSkTextStyle();
                    paragraphBuilder.AddText(textBlockSpan.Text, textStyle);
                    currentTextIndex += textBlockSpan.Text.Length;
                }
                else if (textBlockItem is TextBlockElement textBlockElement)
                {
                    textBlockElement.ConfigureElement(PageContext, Canvas);
                    textBlockElement.UpdateElementSize();
                    
                    paragraphBuilder.AddPlaceholder(new SkPlaceholderStyle
                    {
                        Width = textBlockElement.ElementSize.Width,
                        Height = textBlockElement.ElementSize.Height,
                        Alignment = MapInjectedTextAlignment(textBlockElement.Alignment),
                        Baseline = SkPlaceholderStyle.PlaceholderBaseline.Alphabetic,
                        BaselineOffset = 0
                    });
                }
            }

            Paragraph = paragraphBuilder.CreateParagraph();
            
            static ParagraphStyleConfiguration.TextAlign MapAlignment(TextHorizontalAlignment alignment)
            {
                return alignment switch
                {
                    TextHorizontalAlignment.Left => ParagraphStyleConfiguration.TextAlign.Left,
                    TextHorizontalAlignment.Center => ParagraphStyleConfiguration.TextAlign.Center,
                    TextHorizontalAlignment.Right => ParagraphStyleConfiguration.TextAlign.Right,
                    TextHorizontalAlignment.Justify => ParagraphStyleConfiguration.TextAlign.Justify,
                    TextHorizontalAlignment.Start => ParagraphStyleConfiguration.TextAlign.Start,
                    TextHorizontalAlignment.End => ParagraphStyleConfiguration.TextAlign.End,
                    _ => throw new Exception()
                };
            }

            static ParagraphStyleConfiguration.TextDirection MapDirection(ContentDirection direction)
            {
                return direction switch
                {
                    ContentDirection.LeftToRight => ParagraphStyleConfiguration.TextDirection.Ltr,
                    ContentDirection.RightToLeft => ParagraphStyleConfiguration.TextDirection.Rtl,
                    _ => throw new Exception()
                };
            }
            
            static SkPlaceholderStyle.PlaceholderAlignment MapInjectedTextAlignment(TextInjectedElementAlignment alignment)
            {
                return alignment switch
                {
                    TextInjectedElementAlignment.AboveBaseline => SkPlaceholderStyle.PlaceholderAlignment.AboveBaseline,
                    TextInjectedElementAlignment.BelowBaseline => SkPlaceholderStyle.PlaceholderAlignment.BelowBaseline,
                    TextInjectedElementAlignment.Top => SkPlaceholderStyle.PlaceholderAlignment.Top,
                    TextInjectedElementAlignment.Bottom => SkPlaceholderStyle.PlaceholderAlignment.Bottom,
                    TextInjectedElementAlignment.Middle => SkPlaceholderStyle.PlaceholderAlignment.Middle,
                    _ => throw new Exception()
                };
            }
        }
        
        private void CalculateParagraphMetrics(Size availableSpace)
        {
            // SkParagraph seems to require a bigger space buffer to calculate metrics correctly
            const float epsilon = 1f;
            
            if (Math.Abs(WidthForLineMetricsCalculation - availableSpace.Width) < epsilon) 
                return;
            
            WidthForLineMetricsCalculation = availableSpace.Width;
                
            Paragraph.PlanLayout(availableSpace.Width + epsilon);
            CheckUnresolvedGlyphs();
                
            LineMetrics = Paragraph.GetLineMetrics();
            PlaceholderPositions = Paragraph.GetPlaceholderPositions();
            MaximumWidth = LineMetrics.Any() ? LineMetrics.Max(x => x.Width) : 0;
        }
        
        private void CheckUnresolvedGlyphs()
        {
            if (!Settings.CheckIfAllTextGlyphsAreAvailable)
                return;
                
            var unsupportedGlyphs = Paragraph.GetUnresolvedCodepoints();
                   
            if (!unsupportedGlyphs.Any())
                return;
                
            var formattedGlyphs = unsupportedGlyphs    
                .Select(codepoint =>
                {
                    var character = char.ConvertFromUtf32(codepoint);
                    return $"U-{codepoint:X4} '{character}'";
                });
                
            var glyphs = string.Join("\n", formattedGlyphs);

            throw new DocumentDrawingException(
                $"Could not find an appropriate font fallback for the following glyphs: \n" +
                $"${glyphs} \n\n" +
                $"Possible solutions: \n" +
                $"1) Install fonts that contain missing glyphs in your runtime environment. \n" +
                $"2) Configure the fallback TextStyle using the 'TextStyle.FontFamilyFallback' method. \n" +
                $"3) Register additional application specific fonts using the 'FontManager.RegisterFont' method. \n\n" +
                $"You can disable this check by setting the 'Settings.CheckIfAllTextGlyphsAreAvailable' option to 'false'. \n" +
                $"However, this may result with text glyphs being incorrectly rendered without any warning.");
        }
    }
}