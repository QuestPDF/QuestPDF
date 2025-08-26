using System;
using System.Diagnostics;
using QuestPDF.Drawing.DrawingCanvases;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Drawing.DocumentCanvases
{
    internal sealed class PdfDocumentCanvas : IDocumentCanvas, IDisposable
    {
        private SkDocument Document { get; }
        private SkCanvas? CurrentPageCanvas { get; set; }
        private ProxyDrawingCanvas DrawingCanvas { get; } = new();
        
        // TODO: is there a better way to pass semantic-related data? Skia requires it BEFORE content generation
        public PdfDocumentCanvas(SkWriteStream stream, DocumentMetadata documentMetadata, DocumentSettings documentSettings, SkPdfTag? pdfTag)
        {
            Document = CreatePdf(stream, documentMetadata, documentSettings, pdfTag);
        }

        private static SkDocument CreatePdf(SkWriteStream stream, DocumentMetadata documentMetadata, DocumentSettings documentSettings, SkPdfTag? pdfTag)
        {
            // do not extract to another method, as it will cause the SkText objects
            // to be disposed before the SkPdfDocument is created
            using var title = new SkText(documentMetadata.Title);
            using var author = new SkText(documentMetadata.Author);
            using var subject = new SkText(documentMetadata.Subject);
            using var keywords = new SkText(documentMetadata.Keywords);
            using var creator = new SkText(documentMetadata.Creator);
            using var producer = new SkText(documentMetadata.Producer);
            using var language = new SkText(documentMetadata.Language);
            
            var internalMetadata = new SkPdfDocumentMetadata
            {
                Title = title,
                Author = author,
                Subject = subject,
                Keywords = keywords,
                Creator = creator,
                Producer = producer,
                Language = language,
                
                CreationDate = new SkDateTime(documentMetadata.CreationDate),
                ModificationDate = new SkDateTime(documentMetadata.ModifiedDate),
                
                RasterDPI = documentSettings.ImageRasterDpi,
                SupportPDFA = documentSettings.PdfA,
                CompressDocument = documentSettings.CompressDocument,
                
                SemanticNodeRoot = pdfTag?.Instance ?? IntPtr.Zero
            };
            
            try
            {
                return SkPdfDocument.Create(stream, internalMetadata);
            }
            catch (TypeInitializationException exception)
            {
                throw new InitializationException("PDF", exception);
            }
        }
        
        #region IDisposable
        
        ~PdfDocumentCanvas()
        {
            this.WarnThatFinalizerIsReached();
            Dispose();
        }
        
        public void Dispose()
        {
            Document?.Dispose();
            CurrentPageCanvas?.Dispose();
            DrawingCanvas?.Dispose();
            
            GC.SuppressFinalize(this);
        }
        
        #endregion
        
        #region IDocumentCanvas
        
        public void BeginDocument()
        {
            
        }

        public void EndDocument()
        {
            Document?.Close();
            Document?.Dispose();
        }

        public void BeginPage(Size size)
        {
            CurrentPageCanvas = Document?.BeginPage(size.Width, size.Height);
            
            DrawingCanvas.Target = new SkiaDrawingCanvas(size.Width, size.Height);
            DrawingCanvas.SetZIndex(0);
        }

        public void EndPage()
        {
            Debug.Assert(CurrentPageCanvas != null);
            
            using var documentPageSnapshot = DrawingCanvas.GetSnapshot();
            documentPageSnapshot.DrawOnSkCanvas(CurrentPageCanvas);
            
            CurrentPageCanvas.Save();
            CurrentPageCanvas.Dispose();
            CurrentPageCanvas = null;
            
            Document.EndPage();
        }
        
        public IDrawingCanvas GetDrawingCanvas()
        {
            return DrawingCanvas;
        }
        
        #endregion
    }
}