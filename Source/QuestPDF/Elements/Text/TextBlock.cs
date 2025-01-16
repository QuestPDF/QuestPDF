﻿using System;
using System.Collections.Concurrent;
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
    internal sealed class TextBlock : Element, IStateful, IContentDirectionAware, IDisposable
    {
        // content
        public List<ITextBlockItem> Items { get; set; } = new();
        
        // configuration
        public TextHorizontalAlignment? Alignment { get; set; }
        public ContentDirection ContentDirection { get; set; }
        
        public int? LineClamp { get; set; }
        public string LineClampEllipsis { get; set; }

        public float ParagraphSpacing { get; set; }
        public float ParagraphFirstLineIndentation { get; set; }
        
        public TextStyle DefaultTextStyle { get; set; } = TextStyle.Default;
        
        // cache
        private bool RebuildParagraphForEveryPage { get; set; }
        private bool AreParagraphMetricsValid { get; set; }
        private bool AreParagraphItemsTransformedWithSpacingAndIndentation { get; set; }
        
        private SkSize[] LineMetrics { get; set; }
        private float WidthForLineMetricsCalculation { get; set; }
        private float MaximumWidth { get; set; }
        private SkRect[] PlaceholderPositions { get; set; }
        private bool? ContainsOnlyWhiteSpace { get; set; }
        
        // native objects
        private SkParagraph Paragraph { get; set; }
        internal bool ClearInternalCacheAfterFullRender { get; set; } = true;

        public string Text => string.Join(" ", Items.OfType<TextBlockSpan>().Select(x => x.Text));

        ~TextBlock()
        {
            Dispose();
        }

        public void Dispose()
        {
            Paragraph?.Dispose();
            GC.SuppressFinalize(this);
        }

        internal override SpacePlan Measure(Size availableSpace)
        {
            if (Items.Count == 0)
                return SpacePlan.Empty();
            
            if (IsRendered)
                return SpacePlan.Empty();
            
            if (availableSpace.IsNegative())
                return SpacePlan.Wrap("The available space is negative.");

            // if the text block does not contain any items, or all items are null, return SpacePlan.Empty
            // but if the text block contains only whitespace, return SpacePlan.FullRender with zero width and font-based height
            ContainsOnlyWhiteSpace ??= CheckIfContainsOnlyWhiteSpace();

            if (ContainsOnlyWhiteSpace == true)
            {
                var requiredHeight = MeasureHeightOfParagraphContainingOnlyWhiteSpace();
                
                return requiredHeight < availableSpace.Height + Size.Epsilon 
                    ? SpacePlan.FullRender(0, requiredHeight) 
                    : SpacePlan.Wrap("The available vertical space is not sufficient to render even a single line of text.");
            }
            
            Initialize();
            
            if (Size.Equal(availableSpace, Size.Zero))
                return SpacePlan.PartialRender(Size.Zero);
            
            CalculateParagraphMetrics(availableSpace);

            if (MaximumWidth == 0)
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
                return SpacePlan.Wrap("The available space is not sufficient to render even a single line of text.");

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

            if (IsRendered)
                return;
            
            if (ContainsOnlyWhiteSpace == true)
                return;
            
            CalculateParagraphMetrics(availableSpace);

            if (MaximumWidth == 0)
                return;
            
            var (linesToDraw, takenHeight) = DetermineLinesToDraw();
            DrawParagraph();
            
            CurrentLineIndex += linesToDraw;
            CurrentTopOffset += takenHeight;

            if (CurrentLineIndex == LineMetrics.Length)
                IsRendered = true;
            
            if (IsRendered && ClearInternalCacheAfterFullRender)
            {
                Paragraph?.Dispose();
                Paragraph = null;
            }
            
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
                foreach (var textBlockElement in Items.OfType<TextBlockElement>())
                {
                    var placeholder = PlaceholderPositions[textBlockElement.ParagraphBlockIndex];
                    
                    textBlockElement.ConfigureElement(PageContext, Canvas);

                    var offset = new Position(placeholder.Left, placeholder.Top);
                    
                    if (!IsPositionVisible(offset))
                        continue;
                    
                    Canvas.Translate(offset);
                    textBlockElement.Element.Draw(new Size(placeholder.Width, placeholder.Height));
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

            if (!AreParagraphItemsTransformedWithSpacingAndIndentation)
            {
                Items = ApplyParagraphSpacingToTextBlockItems().ToList();
                AreParagraphItemsTransformedWithSpacingAndIndentation = true;
            }
            
            RebuildParagraphForEveryPage = Items.Any(x => x is TextBlockPageNumber);
            BuildParagraph();
            AreParagraphMetricsValid = false;
        }

        private void BuildParagraph()
        {
            Alignment ??= TextHorizontalAlignment.Start;
            
            var paragraphStyle = new ParagraphStyle
            {
                Alignment = MapAlignment(Alignment.Value),
                Direction = MapDirection(ContentDirection),
                MaxLinesVisible = LineClamp ?? 1_000_000,
                LineClampEllipsis = LineClampEllipsis
            };
            
            var builder = SkParagraphBuilderPoolManager.Get(paragraphStyle);

            try
            {
                Paragraph = CreateParagraph(builder);
            }
            finally
            {
                SkParagraphBuilderPoolManager.Return(builder);
            }

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

            SkParagraph CreateParagraph(SkParagraphBuilder builder)
            {
                var currentTextIndex = 0;
                var currentBlockIndex = 0;
            
                if (!Items.Any(x => x is TextBlockSpan))
                    builder.AddText("\u200B", DefaultTextStyle.GetSkTextStyle());
                
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
                        var text = textBlockSpan.Text?.Replace("\r", "") ?? "";
                        builder.AddText(text, textStyle);
                        currentTextIndex += text.Length;
                    }
                    else if (textBlockItem is TextBlockElement textBlockElement)
                    {
                        textBlockElement.ConfigureElement(PageContext, Canvas);
                        textBlockElement.UpdateElementSize();
                        textBlockElement.ParagraphBlockIndex = currentBlockIndex;
                    
                        builder.AddPlaceholder(new SkPlaceholderStyle
                        {
                            Width = textBlockElement.ElementSize.Width,
                            Height = textBlockElement.ElementSize.Height,
                            Alignment = MapInjectedTextAlignment(textBlockElement.Alignment),
                            Baseline = SkPlaceholderStyle.PlaceholderBaseline.Alphabetic,
                            BaselineOffset = 0
                        });

                        currentTextIndex++;
                        currentBlockIndex++;
                    }
                    else if (textBlockItem is TextBlockParagraphSpacing spacing)
                    {
                        builder.AddPlaceholder(new SkPlaceholderStyle
                        {
                            Width = spacing.Width,
                            Height = spacing.Height,
                            Alignment = SkPlaceholderStyle.PlaceholderAlignment.Middle,
                            Baseline = SkPlaceholderStyle.PlaceholderBaseline.Alphabetic,
                            BaselineOffset = 0
                        });

                        currentTextIndex++;
                        currentBlockIndex++;
                    }
                }

                return builder.CreateParagraph();
            }
        }

        private IEnumerable<ITextBlockItem> ApplyParagraphSpacingToTextBlockItems()
        {
            if (ParagraphSpacing < Size.Epsilon && ParagraphFirstLineIndentation < Size.Epsilon)
                return Items;
            
            var result = new List<ITextBlockItem>();
            AddParagraphFirstLineIndentation();
            
            foreach (var textBlockItem in Items)
            {
                if (textBlockItem is not TextBlockSpan textBlockSpan)
                {
                    result.Add(textBlockItem);
                    continue;
                }
                
                if (textBlockItem is TextBlockPageNumber)
                {
                    result.Add(textBlockItem);
                    continue;
                }
                
                var textFragments = textBlockSpan.Text.Split('\n');
                    
                foreach (var textFragment in textFragments)
                {
                    AddClonedTextBlockSpanWithTextFragment(textBlockSpan, textFragment);
                        
                    if (textFragment == textFragments.Last())
                        continue;

                    AddParagraphSpacing();
                    AddParagraphFirstLineIndentation();
                }
            }

            return result;

            void AddClonedTextBlockSpanWithTextFragment(TextBlockSpan originalSpan, string textFragment)
            {
                TextBlockSpan newItem;
                        
                if (originalSpan is TextBlockSectionLink textBlockSectionLink)
                    newItem = new TextBlockSectionLink { SectionName = textBlockSectionLink.SectionName };
            
                else if (originalSpan is TextBlockHyperlink textBlockHyperlink)
                    newItem = new TextBlockHyperlink { Url = textBlockHyperlink.Url };
            
                else if (originalSpan is TextBlockPageNumber textBlockPageNumber)
                    newItem = textBlockPageNumber;

                else
                    newItem = new TextBlockSpan();

                newItem.Text = textFragment;
                newItem.Style = originalSpan.Style;
                
                result.Add(newItem);
            }
            
            void AddParagraphSpacing()
            {
                if (ParagraphSpacing <= Size.Epsilon)
                    return;
                
                // space ensure proper line spacing
                result.Add(new TextBlockSpan() { Text = "\n ", Style = TextStyle.ParagraphSpacing }); 
                result.Add(new TextBlockParagraphSpacing(0, ParagraphSpacing));
                result.Add(new TextBlockSpan() { Text = " \n", Style = TextStyle.ParagraphSpacing });
            }
            
            void AddParagraphFirstLineIndentation()
            {
                if (ParagraphFirstLineIndentation <= Size.Epsilon)
                    return;
                
                result.Add(new TextBlockSpan() { Text = "\n", Style = TextStyle.ParagraphSpacing });
                result.Add(new TextBlockParagraphSpacing(ParagraphFirstLineIndentation, 0));
            }
        }

        private void CalculateParagraphMetrics(Size availableSpace)
        {
            // optimization, invalidate cache when both:
            // 1) the width for which cache was calculated is different from the provided width
            // 2) provided width is different from the target width of the text component
            if (AreDifferent(WidthForLineMetricsCalculation, availableSpace.Width) && AreDifferent(MaximumWidth, availableSpace.Width))
                AreParagraphMetricsValid = false;
            
            if (AreParagraphMetricsValid) 
                return;
            
            WidthForLineMetricsCalculation = availableSpace.Width;
                
            Paragraph.PlanLayout(availableSpace.Width);
            CheckUnresolvedGlyphs();
                
            LineMetrics = Paragraph.GetLineMetrics();
            PlaceholderPositions = Paragraph.GetPlaceholderPositions();
            MaximumWidth = LineMetrics.Any() ? LineMetrics.Max(x => x.Width) : 0;
            
            AreParagraphMetricsValid = true;
            
            static bool AreDifferent(float x, float y) => Math.Abs(x - y) > Size.Epsilon;
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
        
        #region Handling Of Text Blocks With Only With Space
        
        private static ConcurrentDictionary<int, float> ParagraphContainingOnlyWhiteSpaceHeightCache { get; } = new(); // key: TextStyle.Id
        
        private bool CheckIfContainsOnlyWhiteSpace()
        {
            foreach (var textBlockItem in Items)
            {
                // TextBlockPageNumber needs to be checked first, as it derives from TextBlockSpan,
                // and before the generation starts, its Text property is empty 
                if (textBlockItem is TextBlockPageNumber)
                    return false;
                
                if (textBlockItem is TextBlockSpan textBlockSpan && !string.IsNullOrWhiteSpace(textBlockSpan.Text))
                    return false;

                if (textBlockItem is TextBlockElement)
                    return false;
            }
            
            return true;
        }
        
        private float MeasureHeightOfParagraphContainingOnlyWhiteSpace()
        {
            return Items
                .OfType<TextBlockSpan>()
                .Select(x => ParagraphContainingOnlyWhiteSpaceHeightCache.GetOrAdd(x.Style.Id, Measure))
                .DefaultIfEmpty(0)
                .Max();
            
            static float Measure(int textStyleId)
            {
                var paragraphStyle = new ParagraphStyle
                {
                    Alignment = ParagraphStyleConfiguration.TextAlign.Start,
                    Direction = ParagraphStyleConfiguration.TextDirection.Ltr,
                    MaxLinesVisible = 1_000_000,
                    LineClampEllipsis = string.Empty
                };
            
                var builder = SkParagraphBuilderPoolManager.Get(paragraphStyle);

                try
                {
                    var textStyle = TextStyleManager.GetTextStyle(textStyleId).GetSkTextStyle();
                    builder.AddText("\u00A0", textStyle); // non-breaking space

                    var paragraph = builder.CreateParagraph();
                    paragraph.PlanLayout(1000);
                    return paragraph.GetLineMetrics().First().Height;
                }
                finally
                {
                    SkParagraphBuilderPoolManager.Return(builder);
                }
            }
        }
        
        #endregion
        
        #region IStateful
        
        private bool IsRendered { get; set; }
        private int CurrentLineIndex { get; set; }
        private float CurrentTopOffset { get; set; }
    
        public struct TextBlockState
        {
            public bool IsRendered;
            public int CurrentLineIndex;
            public float CurrentTopOffset;
        }
        
        public void ResetState(bool hardReset = false)
        {
            IsRendered = false;
            CurrentLineIndex = 0;
            CurrentTopOffset = 0;
        }

        public object GetState()
        {
            return new TextBlockState
            {
                IsRendered = IsRendered,
                CurrentLineIndex = CurrentLineIndex,
                CurrentTopOffset = CurrentTopOffset
            };
        }

        public void SetState(object state)
        {
            var textBlockState = (TextBlockState) state;
            
            IsRendered = textBlockState.IsRendered;
            CurrentLineIndex = textBlockState.CurrentLineIndex;
            CurrentTopOffset = textBlockState.CurrentTopOffset;
        }
    
        #endregion

        internal override string? GetCompanionHint() => Text.Substring(0, Math.Min(Text.Length, 50));
        internal override string? GetCompanionSearchableContent() => Text;
    }
}