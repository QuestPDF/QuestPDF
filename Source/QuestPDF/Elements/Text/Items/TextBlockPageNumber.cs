using System;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Text.Items
{
    internal sealed class TextBlockPageNumber : TextBlockSpan
    {
        public const string PageNumberPlaceholder = "123";
        public Func<IPageContext, string> Source { get; set; } = _ => PageNumberPlaceholder;

        public void UpdatePageNumberText(IPageContext context)
        {
            Text = Source(context) ?? PageNumberPlaceholder;
        }
    }
}