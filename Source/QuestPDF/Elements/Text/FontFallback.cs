using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Elements.Text.Items;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Elements.Text
{
    internal static class FontFallback
    {
        public struct TextRun
        {
            public string Content { get; set; }
            public TextStyle Style { get; set; }
        }

        public class FallbackOption
        {
            public TextStyle Style { get; set; }
            public SKFont Font { get; set; }
            public SKTypeface Typeface { get; set; }
        }

        private static SKFontManager FontManager => SKFontManager.Default;

        public static IEnumerable<TextRun> SplitWithFontFallback(this string text, TextStyle textStyle)
        {
            var fallbackOptions = GetFallbackOptions(textStyle).ToArray();
            
            var spanStartIndex = 0;
            var spanFallbackOption = fallbackOptions[0];
            
            for (var i = 0; i < text.Length; i += char.IsSurrogatePair(text, i) ? 2 : 1)
            {
                var codepoint = char.ConvertToUtf32(text, i);
                var newFallbackOption = MatchFallbackOption(fallbackOptions, codepoint);

                if (newFallbackOption == spanFallbackOption)
                    continue;

                yield return new TextRun
                {
                    Content = text.Substring(spanStartIndex, i - spanStartIndex),
                    Style = spanFallbackOption.Style
                };

                spanStartIndex = i;
                spanFallbackOption = newFallbackOption;
            }
            
            if (spanStartIndex > text.Length)
                yield break;
            
            yield return new TextRun
            {
                Content = text.Substring(spanStartIndex, text.Length - spanStartIndex),
                Style = spanFallbackOption.Style
            };

            static IEnumerable<FallbackOption> GetFallbackOptions(TextStyle? textStyle)
            {
                while (textStyle != null)
                {
                    var font = textStyle.ToFont();
                    
                    yield return new FallbackOption
                    {
                        Style = textStyle,
                        Font = font,
                        Typeface = font.Typeface
                    };

                    textStyle = textStyle.Fallback;
                }
            }

            static FallbackOption MatchFallbackOption(ICollection<FallbackOption> fallbackOptions, int codepoint)
            {
                foreach (var fallbackOption in fallbackOptions)
                {
                    if (fallbackOption.Font.ContainsGlyph(codepoint))
                        return fallbackOption;
                }

                if (Settings.CheckIfAllTextGlyphsAreAvailable)
                    throw CreateNotMatchingFontException(codepoint);

                return fallbackOptions.First();
            }

            static Exception CreateNotMatchingFontException(int codepoint)
            {
                var character = char.ConvertFromUtf32(codepoint);
                var unicode = $"U-{codepoint:X4}";

                var proposedFonts = FindFontsContainingGlyph(codepoint).ToArray();
                var proposedFontsFormatted = proposedFonts.Any() ? string.Join(", ", proposedFonts) : "no fonts available";
                
                return new DocumentDrawingException(
                    $"Could not find an appropriate font fallback for glyph: {unicode} '{character}'. " +
                    $"Font families available on current environment that contain this glyph: {proposedFontsFormatted}. " +
                    $"Possible solutions: " +
                    $"1) Use one of the listed fonts as the primary font in your document. " +
                    $"2) Configure the fallback TextStyle using the 'TextStyle.Fallback' method with one of the listed fonts. " +
                    $"You can disable this check by setting the 'Settings.CheckIfAllTextGlyphsAreAvailable' option to 'false'. " +
                    $"However, this may result with text glyphs being incorrectly rendered without any warning.");
            }
            
            static IEnumerable<string> FindFontsContainingGlyph(int codepoint)
            {
                var fontManager = SKFontManager.Default;

                return fontManager
                    .GetFontFamilies()
                    .Select(fontManager.MatchFamily)
                    .Where(x => x.ContainsGlyph(codepoint))
                    .Select(x => x.FamilyName);
            }
        }

        public static IEnumerable<ITextBlockItem> ApplyFontFallback(this ICollection<ITextBlockItem> textBlockItems)
        {
            foreach (var textBlockItem in textBlockItems)
            {
                if (textBlockItem is TextBlockPageNumber or TextBlockElement)
                {
                    yield return textBlockItem;
                }
                else if (textBlockItem is TextBlockSpan textBlockSpan)
                {
                    if (!Settings.CheckIfAllTextGlyphsAreAvailable && textBlockSpan.Style.Fallback == null)
                    {
                        yield return textBlockSpan;
                        continue;
                    }
                    
                    var textRuns = textBlockSpan.Text.SplitWithFontFallback(textBlockSpan.Style);
                    
                    foreach (var textRun in textRuns)
                    {
                        var newElement = textBlockSpan switch
                        {
                            TextBlockHyperlink hyperlink => new TextBlockHyperlink { Url = hyperlink.Url },
                            TextBlockSectionLink sectionLink => new TextBlockSectionLink { SectionName = sectionLink.SectionName },
                            TextBlockSpan => new TextBlockSpan()
                        };

                        newElement.Text = textRun.Content;
                        newElement.Style = textRun.Style;

                        yield return newElement;
                    }
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }
    }
}