using System.Collections.Generic;
using QuestPDF.Drawing;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Elements.Text.Items;
using QuestPDF.Fluent;
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

        private static SKFontManager FontManager => SKFontManager.Default;

        public static IEnumerable<TextRun> SplitWithFontFallback(this string text, TextStyle textStyle)
        {
            var partStartIndex = 0;
            var partTextStyle = textStyle;

            for (var i = 0; i < text.Length; i += char.IsSurrogatePair(text, i) ? 2 : 1)
            {
                var codepoint = char.ConvertToUtf32(text, i);
                var font = partTextStyle.ToFont();
                var typeface = font.Typeface;

                if (font.ContainsGlyph(codepoint))
                    continue;
                
                var fallbackTypeface = FontManager.MatchCharacter(typeface.FamilyName, typeface.FontWeight, typeface.FontWidth, typeface.FontSlant, null, codepoint);

                if (fallbackTypeface == null)
                    throw new DocumentDrawingException($"Could not find an appropriate font fallback for text: '{text}'");

                yield return new TextRun
                {
                    Content = text.Substring(partStartIndex, i - partStartIndex),
                    Style = partTextStyle
                };

                partStartIndex = i;
                partTextStyle = textStyle.FontFamily(fallbackTypeface.FamilyName).Weight((FontWeight)fallbackTypeface.FontWeight);
            }
            
            if (partStartIndex > text.Length)
                yield break;
            
            yield return new TextRun
            {
                Content = text.Substring(partStartIndex, text.Length - partStartIndex),
                Style = partTextStyle
            };
        }

        public static IEnumerable<ITextBlockItem> ApplyFontFallback(this ICollection<ITextBlockItem> textBlockItems)
        {
            foreach (var textBlockItem in textBlockItems)
            {
                if (textBlockItem is TextBlockSpan textBlockSpan)
                {
                    var textRuns = textBlockSpan.Text.SplitWithFontFallback(textBlockSpan.Style);

                    foreach (var textRun in textRuns)
                    {
                        yield return new TextBlockSpan
                        {
                            Text = textRun.Content,
                            Style = textRun.Style
                        };
                    }
                }
                else
                {
                    yield return textBlockItem;
                }
            }
        }
    }
}