using System;
using QuestPDF.Elements.Text.Calculation;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Text.Items
{
    internal class TextBlockPageNumber : TextBlockSpan
    {
        public const string PageNumberPlaceholder = "123";
        public Func<IPageContext, string> Source { get; set; } = _ => PageNumberPlaceholder;
        
        public override TextBlockSize? Measure()
        {
            SetPageNumber();
            return base.Measure();
        }

        public override void Draw(TextDrawingRequest request)
        {
            SetPageNumber();
            base.Draw(request);
        }

        private void SetPageNumber()
        {
            Text = Source(PageContext) ?? PageNumberPlaceholder;
        }
    }
}