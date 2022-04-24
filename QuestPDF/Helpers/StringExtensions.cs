using System.Collections.Generic;

namespace QuestPDF.Helpers
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Splits strings into multiple chunks determined by the delimiters.
        /// 
        /// Example:
        /// Input  - 'Hello World !'
        /// Output - ['Hello ', 'World ', '!']
        /// </summary>
        public static IEnumerable<string> SplitAndKeep(this string text, char[] delimiters)
        {
            if (string.IsNullOrEmpty(text))
                yield break;

            var start = 0;
            int index;
            while ((index = text.IndexOfAny(delimiters, start)) != -1)
            {
                if (index - start + 1 > 0)
                    yield return text.Substring(start, index - start + 1);

                start = index + 1;
            }

            if(start < text.Length)
                yield return text.Substring(start);
        }
    }
}
