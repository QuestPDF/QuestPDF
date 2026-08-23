using System;
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
    internal sealed class TextBlock : Element, IStateful, IPageContextAware, IContentDirectionAware, IDisposable
    {
        // content
        public List<ITextBlockItem> Items { get; set; } = new();
        
        // configuration
        public TextHorizontalAlignment? Alignment { get; set; }
        public IPageContext PageContext { get; set; }
        public ContentDirection ContentDirection { get; set; }
        
        public int? LineClamp { get; set; }
        public string LineClampEllipsis { get; set; }

        public float ParagraphSpacing { get; set; }
        public float ParagraphFirstLineIndentation { get; set; }
        
        public TextStyle DefaultTextStyle { get; set; } = TextStyle.Default;
        
        // cache
        private bool RebuildParagraphForEveryPage { get; set; }
        private bool AreParagraphMetricsValid { get; set; }
        
        private int LineCount { get; set; }
        private SkLineExtent[] LineExtents { get; set; }
        private float WidthForLineMetricsCalculation { get; set; }
        private float TotalWidth { get; set; }
        private SkRect[] PlaceholderPositions { get; set; }
        private bool? ContainsOnlyWhiteSpace { get; set; }
        
        // native objects
        private SkParagraph Paragraph { get; set; }
        internal bool ClearInternalCacheAfterFullRender { get; set; } = true;

        public string Text => string.Join(" ", Items.OfType<TextBlockSpan>().Select(x => x.Text));

        ~TextBlock()
        {
            if (Paragraph == null)
                return;
            
            this.WarnThatFinalizerIsReached();
            Dispose();
        }

        public void Dispose()
        {
            Paragraph?.Dispose();
            
            foreach (var textBlockItem in Items)
            {
                if (textBlockItem is TextBlockElement textBlockElement)
                    textBlockElement.Element.ReleaseDisposableChildren();
            }
            
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

            if (availableSpace.Width < Size.Epsilon || availableSpace.Height < Size.Epsilon)
                return SpacePlan.Wrap("The available space is not sufficient to render even a single line of text.");
            
            Initialize();

            CalculateParagraphMetrics(availableSpace);
            
            if (LineCount == 0)
                return SpacePlan.FullRender(Size.Zero);

            if (availableSpace.Width < TotalWidth - Size.Epsilon)
                return SpacePlan.Wrap($"The available space is not sufficient to render even a single character.");

            if (TotalWidth == 0)
                return SpacePlan.FullRender(Size.Zero);
            
            var (linesToDraw, takenHeight) = DetermineLinesToDraw(availableSpace);

            if (linesToDraw == 0)
                return SpacePlan.Wrap("The available space is not sufficient to render even a single line of text.");

            var requiredArea = new Size(
                Math.Min(TotalWidth, availableSpace.Width),
                Math.Min(takenHeight, availableSpace.Height));
            
            if (CurrentLineIndex + linesToDraw < LineCount)
                return SpacePlan.PartialRender(requiredArea);

            return SpacePlan.FullRender(requiredArea);
        }

        private (int linesToDraw, float takenHeight) DetermineLinesToDraw(Size availableSpace)
        {
            var firstLine = LineExtents[CurrentLineIndex];
            var totalLines = CountLinesFittingInAvailableHeight();
            
            if (totalLines == 0)
                return (0, 0);
            
            var lastLine = LineExtents[CurrentLineIndex + totalLines - 1];
            
            var takenHeight = lastLine.Bottom - firstLine.Top;

            return (totalLines, takenHeight);

            int CountLinesFittingInAvailableHeight()
            {
                var result = 0;
                var availableHeightLimit = availableSpace.Height + Size.Epsilon;

                for (var lineIndex = CurrentLineIndex; lineIndex < LineCount; lineIndex++)
                {
                    if (LineExtents[lineIndex].Bottom - firstLine.Top >= availableHeightLimit)
                        break;

                    result++;
                }

                return result;
            }
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

            if (LineCount == 0 || TotalWidth == 0)
                return;
            
            var pageStartTop = LineExtents[CurrentLineIndex].Top;
            var (linesToDraw, takenHeight) = DetermineLinesToDraw(availableSpace);
            
            if (linesToDraw == 0)
                return;
            
            DrawParagraph();
            
            CurrentLineIndex += linesToDraw;

            if (CurrentLineIndex == LineCount)
                IsRendered = true;
            
            if (IsRendered && ClearInternalCacheAfterFullRender)
            {
                Paragraph?.Dispose();
                Paragraph = null;
            }
            
            return;
            
            void DrawParagraph()
            {
                var takesMultiplePages = linesToDraw != LineCount;

                if (takesMultiplePages)
                {
                    Canvas.Save();
                    Canvas.Translate(new Position(0, -pageStartTop));
                }

                Canvas.DrawParagraph(Paragraph, CurrentLineIndex, CurrentLineIndex + linesToDraw - 1);
                
                if (takesMultiplePages)
                    Canvas.ClipRectangle(new SkRect(0, pageStartTop, availableSpace.Width, pageStartTop + takenHeight));

                foreach (var textBlockItem in Items)
                {
                    if (textBlockItem is TextBlockElement textBlockElement)
                        DrawInjectedElement(textBlockElement);

                    else if (textBlockItem is TextBlockHyperlink textBlockHyperlink)
                        DrawHyperlink(textBlockHyperlink);

                    else if (textBlockItem is TextBlockSectionLink textBlockSectionLink)
                        DrawSectionLink(textBlockSectionLink);
                }
                
                if (takesMultiplePages)
                    Canvas.Restore();
            }

            void DrawInjectedElement(TextBlockElement textBlockElement)
            {
                var placeholder = PlaceholderPositions[textBlockElement.ParagraphBlockIndex];

                textBlockElement.ConfigureElement(PageContext, Canvas);

                var offset = new Position(placeholder.Left, placeholder.Top);

                if (!IsPositionVisible(placeholder))
                    return;

                Canvas.Translate(offset);
                textBlockElement.Element.Draw(new Size(placeholder.Width, placeholder.Height));
                Canvas.Translate(offset.Reverse());
            }

            void DrawHyperlink(TextBlockHyperlink hyperlink)
            {
                var positions = Paragraph.GetTextRangePositions(hyperlink.ParagraphBeginIndex, hyperlink.ParagraphEndIndex);

                foreach (var position in positions)
                {
                    var offset = new Position(position.Left, position.Top);

                    if (!IsPositionVisible(position))
                        continue;

                    Canvas.Translate(offset);
                    Canvas.DrawHyperlink(new Size(position.Width, position.Height), hyperlink.Url, hyperlink.Text);
                    Canvas.Translate(offset.Reverse());
                }
            }

            void DrawSectionLink(TextBlockSectionLink sectionLink)
            {
                var positions = Paragraph.GetTextRangePositions(sectionLink.ParagraphBeginIndex, sectionLink.ParagraphEndIndex);
                var targetName = PageContext.GetDocumentLocationName(sectionLink.SectionName);

                foreach (var position in positions)
                {
                    var offset = new Position(position.Left, position.Top);

                    if (!IsPositionVisible(position))
                        continue;

                    Canvas.Translate(offset);
                    Canvas.DrawSectionLink(new Size(position.Width, position.Height), targetName, sectionLink.Text);
                    Canvas.Translate(offset.Reverse());
                }
            }

            bool IsPositionVisible(SkRect rect)
            {
                return pageStartTop <= rect.Bottom && rect.Top <= pageStartTop + takenHeight;
            }
        }
        
        private void Initialize()
        {
            if (Paragraph != null && !RebuildParagraphForEveryPage)
                return;

            RebuildParagraphForEveryPage = ContainsItemOfType<TextBlockPageNumber>();
            BuildParagraph();
            AreParagraphMetricsValid = false;
        }

        private bool ContainsItemOfType<T>()
        {
            foreach (var textBlockItem in Items)
            {
                if (textBlockItem is T)
                    return true;
            }

            return false;
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

            if (Paragraph != null)
            {
                Paragraph.Dispose();
                Paragraph = null;
            }
            
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
                var applyParagraphStyling = ParagraphSpacing > Size.Epsilon || ParagraphFirstLineIndentation > Size.Epsilon;

                var currentTextIndex = 0;
                var currentBlockIndex = 0;
            
                if (!ContainsItemOfType<TextBlockSpan>() && ParagraphFirstLineIndentation <= Size.Epsilon)
                    builder.AddText("\u200B", DefaultTextStyle.GetSkTextStyle());

                AddParagraphFirstLineIndentation();

                foreach (var textBlockItem in Items)
                {
                    if (textBlockItem is TextBlockSpan textBlockSpan)
                    {
                        if (textBlockItem is TextBlockPageNumber textBlockPageNumber)
                            textBlockPageNumber.UpdatePageNumberText(PageContext);

                        var spanBeginIndex = currentTextIndex;
                        AddSpanText(textBlockSpan);

                        if (textBlockItem is TextBlockSectionLink textBlockSectionLink)
                        {
                            textBlockSectionLink.ParagraphBeginIndex = spanBeginIndex;
                            textBlockSectionLink.ParagraphEndIndex = currentTextIndex;
                        }
                        else if (textBlockItem is TextBlockHyperlink textBlockHyperlink)
                        {
                            textBlockHyperlink.ParagraphBeginIndex = spanBeginIndex;
                            textBlockHyperlink.ParagraphEndIndex = currentTextIndex;
                        }
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
                }

                return builder.CreateParagraph();

                void AddSpanText(TextBlockSpan textBlockSpan)
                {
                    if (string.IsNullOrEmpty(textBlockSpan.Text))
                        return;

                    var textStyle = textBlockSpan.Style.GetSkTextStyle();
                    var isFirstLineOfSpan = true;

                    foreach (var line in textBlockSpan.Text.AsSpan().SplitLines())
                    {
                        if (!isFirstLineOfSpan)
                            AddLineBreak(textStyle);

                        isFirstLineOfSpan = false;

                        builder.AddText(line, textStyle);
                        currentTextIndex += line.Length;
                    }
                }
                
                void AddLineBreak(SkTextStyle textStyle)
                {
                    if (applyParagraphStyling)
                    {
                        AddParagraphSpacing();
                        AddParagraphFirstLineIndentation();
                        return;
                    }

                    builder.AddText("\n", textStyle);
                    currentTextIndex++;
                }
                
                void AddParagraphSpacing()
                {
                    if (ParagraphSpacing <= Size.Epsilon)
                        return;

                    // the surrounding space characters ensure proper line spacing of the placeholder line
                    AddMarkerText("\n ");
                    AddMarkerPlaceholder(width: 0, height: ParagraphSpacing);
                    AddMarkerText(" \n");
                }

                void AddParagraphFirstLineIndentation()
                {
                    if (ParagraphFirstLineIndentation <= Size.Epsilon)
                        return;

                    AddMarkerText("\n");
                    AddMarkerPlaceholder(width: ParagraphFirstLineIndentation, height: 0);
                }

                void AddMarkerText(string text)
                {
                    builder.AddText(text, TextStyle.ParagraphSpacing.GetSkTextStyle());
                    currentTextIndex += text.Length;
                }

                void AddMarkerPlaceholder(float width, float height)
                {
                    builder.AddPlaceholder(new SkPlaceholderStyle
                    {
                        Width = width,
                        Height = height,
                        Alignment = SkPlaceholderStyle.PlaceholderAlignment.Middle,
                        Baseline = SkPlaceholderStyle.PlaceholderBaseline.Alphabetic,
                        BaselineOffset = 0
                    });

                    currentTextIndex++;
                    currentBlockIndex++;
                }
            }
        }
        
        private void CalculateParagraphMetrics(Size availableSpace)
        {
            if (Math.Abs(WidthForLineMetricsCalculation - availableSpace.Width) > Size.Epsilon)
                AreParagraphMetricsValid = false;
            
            if (AreParagraphMetricsValid) 
                return;
            
            WidthForLineMetricsCalculation = availableSpace.Width;

            Paragraph.PlanLayout(availableSpace.Width);

            CheckUnresolvedGlyphs();

            LineExtents = Paragraph.GetLineExtents();
            LineCount = LineExtents.Length;
            TotalWidth = Paragraph.GetSize().width;
            
            if (ContainsItemOfType<TextBlockElement>())
                PlaceholderPositions = Paragraph.GetPlaceholderPositions();

            AreParagraphMetricsValid = true;
        }
        
        private void CheckUnresolvedGlyphs()
        {
            if (!Settings.CheckIfAllTextGlyphsAreAvailable)
                return;
                
            var unsupportedGlyphs = Paragraph.GetUnresolvedCodepoints();
                   
            if (unsupportedGlyphs.Length == 0)
                return;
                
            var formattedGlyphs = unsupportedGlyphs    
                .Select(codepoint =>
                {
                    var character = char.ConvertFromUtf32(codepoint);
                    return $"U-{codepoint:X4} '{character}'";
                });
                
            var glyphs = string.Join("\n", formattedGlyphs);

            var fontFamilies = Items
                .OfType<TextBlockSpan>()
                .SelectMany(x => x.Style.FontFamilies ?? [])
                .Distinct()
                .Select(x => $"'{x}'")
                .ToList();

            var fontFamiliesFormatted = string.Join(", ", fontFamilies);

            throw new DocumentDrawingException(
                $"Could not find an appropriate font fallback for the following glyphs: \n" +
                $"{glyphs} \n\n" +
                $"Font families used in this text block: [{fontFamiliesFormatted}] \n\n" +
                $"Possible solutions: \n" +
                $"1) (Recommended) Include all necessary font files with your application (e.g. during the publish operation). The QuestPDF library automatically scans the application directory and registers all present font files. \n" +
                $"2) Install fonts that contain missing glyphs in your runtime environment. \n" +
                $"3) Configure the fallback TextStyle using the 'TextStyle.FontFamilyFallback' method. \n" +
                $"4) Register additional application-specific fonts using the 'FontManager.RegisterFont' method. \n\n" +
                $"To suppress this check, set 'Settings.CheckIfAllTextGlyphsAreAvailable' to 'false'. \n" +
                $"Please note that disabling this check allows document generation to continue, but missing glyphs may be rendered as replacement characters or empty areas.");
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

                    using var paragraph = builder.CreateParagraph();
                    paragraph.PlanLayout(1000);
                    
                    var lineExtent = paragraph.GetLineExtents().First();
                    return lineExtent.Bottom - lineExtent.Top;
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
    
        public struct TextBlockState
        {
            public bool IsRendered;
            public int CurrentLineIndex;
        }
        
        public void ResetState(bool hardReset = false)
        {
            IsRendered = false;
            CurrentLineIndex = 0;
        }

        public object GetState()
        {
            return new TextBlockState
            {
                IsRendered = IsRendered,
                CurrentLineIndex = CurrentLineIndex
            };
        }

        public void SetState(object state)
        {
            var textBlockState = (TextBlockState) state;
            
            IsRendered = textBlockState.IsRendered;
            CurrentLineIndex = textBlockState.CurrentLineIndex;
        }
    
        #endregion

        internal override string? GetCompanionHint() => Text.Substring(0, Math.Min(Text.Length, 50));
        internal override string? GetCompanionSearchableContent() => Text;
    }
}
