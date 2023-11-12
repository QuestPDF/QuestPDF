using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Drawing
{
    internal class SkiaDocumentCanvasBase : SkiaCanvasBase
    {
        private SKDocument? Document { get; }

        protected SkiaDocumentCanvasBase(SKDocument document)
        {
            Document = document;
        }

        ~SkiaDocumentCanvasBase()
        {
            Document?.Dispose();
        }
        
        public override void BeginDocument()
        {
            
        }

        public override void EndDocument()
        {
            Canvas?.Dispose();
            
            Document.Close();
            Document.Dispose();
        }

        public override void BeginPage(Size size)
        {
            Canvas = Document.BeginPage(size.Width, size.Height);
            
            base.BeginPage(size);
        }

        public override void EndPage()
        {
            base.EndPage();
            
            Document.EndPage();
            Canvas.Dispose();
        }
    }
}