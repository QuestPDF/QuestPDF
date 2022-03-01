using QuestPDF.Elements.Text.Calculation;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Text.Items
{
    internal interface ITextBlockItem
    {
        ICanvas Canvas { get; set; }
        IPageContext PageContext { get; set; }
        
        TextBlockSize? Measure();
        void Draw(TextDrawingRequest request);
    }
}