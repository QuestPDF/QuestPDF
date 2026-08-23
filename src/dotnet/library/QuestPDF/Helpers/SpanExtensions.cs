using System;

namespace QuestPDF.Helpers
{
    internal static class SpanExtensions
    {
        public static LineSplitEnumerator SplitLines(this ReadOnlySpan<char> text) => new(text);

        public ref struct LineSplitEnumerator
        {
            private ReadOnlySpan<char> RemainingText;
            private bool IsCompleted;

            public ReadOnlySpan<char> Current { get; private set; }

            public LineSplitEnumerator(ReadOnlySpan<char> text)
            {
                RemainingText = text;
                IsCompleted = false;
                Current = default;
            }

            public LineSplitEnumerator GetEnumerator() => this;

            public bool MoveNext()
            {
                if (IsCompleted)
                    return false;

                var lineBreakIndex = RemainingText.IndexOfAny('\r', '\n');

                if (lineBreakIndex < 0)
                {
                    Current = RemainingText;
                    RemainingText = default;
                    IsCompleted = true;
                    return true;
                }

                Current = RemainingText.Slice(0, lineBreakIndex);

                var lineBreak = RemainingText.Slice(lineBreakIndex);
                var lineBreakLength = lineBreak.StartsWith("\r\n".AsSpan()) ? 2 : 1;
                RemainingText = lineBreak.Slice(lineBreakLength);

                return true;
            }
        }
    }
}
