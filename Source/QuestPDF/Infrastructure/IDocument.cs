using QuestPDF.Drawing;

namespace QuestPDF.Infrastructure
{
    public interface IDocument
    {
        DocumentMetadata GetMetadata();
        void Compose(IDocumentContainer container);
    }
}