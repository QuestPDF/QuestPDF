using QuestPDF.Drawing;

namespace QuestPDF.Infrastructure
{
    public interface IDocument
    {
#if NETCOREAPP2_1_OR_GREATER
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default; 
        public DocumentSettings GetSettings() => DocumentSettings.Default;
#else
        DocumentMetadata GetMetadata();
        DocumentSettings GetSettings();
#endif
        
        void Compose(IDocumentContainer container);
    }
}