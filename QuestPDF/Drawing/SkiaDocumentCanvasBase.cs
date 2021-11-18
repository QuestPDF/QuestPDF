using System.IO;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Drawing
{
    internal class SkiaDocumentCanvasBase : SkiaCanvasBase
    {
        private SKDocument Document { get; }

        public SkiaDocumentCanvasBase(SKDocument document)
        {
            Document = document;
        }

        ~SkiaDocumentCanvasBase()
        {
            Document.Dispose();
        }
        
        public override void BeginDocument()
        {
            
        }

        public override void EndDocument()
        {
            Canvas.Dispose();
            
            Document.Close();
            Document.Dispose();
        }

        public override void BeginPage(Size size)
        {
            Canvas = Document.BeginPage(size.Width, size.Height);
        }

        public override void EndPage()
        {
            Document.EndPage();
            Canvas.Dispose();
        }
    }
}