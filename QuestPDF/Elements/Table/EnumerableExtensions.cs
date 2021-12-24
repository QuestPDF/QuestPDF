using System;
using System.Collections.Generic;

namespace QuestPDF.Elements.Table
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> Scan<T>(this IEnumerable<T> input, Func<T, T, T> accumulate)
        {
            using var enumerator = input.GetEnumerator();
            
            if (!enumerator.MoveNext())
                yield break;
            
            var state = enumerator.Current;
            yield return state;
            
            while (enumerator.MoveNext())
            {
                state = accumulate(state, enumerator.Current);
                yield return state;
            }
        }
    }
}