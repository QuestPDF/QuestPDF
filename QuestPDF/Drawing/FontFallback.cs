using System.Collections.Generic;
using System.Linq;
using SkiaSharp;

namespace QuestPDF.Drawing
{
    internal static class FontFallback
    {
        private const int CodepointWhiteSpace = 32;

        public readonly struct TextRun
        {
            public readonly int Start;
            public readonly int End;
            public readonly SKTypeface Typeface;

            public TextRun(int start, int end, SKTypeface typeface)
            {
                Start = start;
                End = end;
                Typeface = typeface;
            }
        }

        private static SKFontManager SKFontManager => SKFontManager.Default;

        /// <summary>
        /// Splits up the given input into seperate runs if the specified typeface is missing some characters and font fallback is needed.
        /// </summary>
        public static IEnumerable<TextRun> BuildRuns(string input, SKTypeface typeface)
        {
            var typefaces = BuildTypefaces(input, typeface).ToArray();
            if (typefaces.Length == 0)
                yield break;

            var lastTypeface = typefaces[0];
            var lastRun = 0;

            for (var i = 1; i < typefaces.Length; i++)
            {
                while (i < typefaces.Length && lastTypeface == typefaces[i])
                    i++;

                if (i >= typefaces.Length)
                    break;

                yield return new TextRun(lastRun, i, lastTypeface);
                lastRun = i;
                lastTypeface = typefaces[i];
            }

            if (lastRun < input.Length - 1)
                yield return new TextRun(lastRun, input.Length, lastTypeface);
        }

        /// <summary>
        /// Returns an enumerable that contains a typeface for each character. If the given typeface doesnt contain a character, a fallback typeface is substituted.
        /// </summary>
        private static IEnumerable<SKTypeface> BuildTypefaces(string input, SKTypeface typeface)
        {
            if (string.IsNullOrEmpty(input))
                yield break;

            var codepoints = BuildCodepoints(input).ToArray();

            for (var i = 0; i < codepoints.Length; i++)
            {
                var codepoint = codepoints[i];

                //No fallback for whitespaces.
                if (codepoint == CodepointWhiteSpace)
                {
                    yield return typeface;
                    continue;
                }

                //GetGlyph returns 0 if the glyph doesnt exist in the typeface.
                var result = typeface.GetGlyph(codepoint);
                if (result != 0)
                {
                    yield return typeface;
                    continue;
                }

                //Font fallback needed
                var fallbackTypeface = SKFontManager.MatchCharacter(typeface.FamilyName, typeface.FontWeight, typeface.FontWidth, typeface.FontSlant, null, codepoint);

                //No fallback found, just return the given typeface
                if (fallbackTypeface == null)
                {
                    yield return typeface;
                    continue;
                }

                yield return fallbackTypeface;
            }
        }

        private static IEnumerable<int> BuildCodepoints(string input)
        {
            if (string.IsNullOrEmpty(input))
                yield break;

            for (var i = 0; i < input.Length; i += char.IsSurrogatePair(input, i) ? 2 : 1)
            {
                var codepoint = char.ConvertToUtf32(input, i);
                yield return codepoint;
            }
        }
    }
}
