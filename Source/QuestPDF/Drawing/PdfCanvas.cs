using System;
using System.Diagnostics;
using System.IO;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Drawing
{
    internal sealed class PdfCanvas : IDocumentCanvas, IDisposable
    {
        private SkDocument Document { get; }
        private SkCanvas? CurrentPageCanvas { get; set; }
        private ProxyDrawingCanvas DrawingCanvas { get; } = new();
        
        public PdfCanvas(SkWriteStream stream, DocumentMetadata documentMetadata, DocumentSettings documentSettings)
        {
            Document = CreatePdf(stream, documentMetadata, documentSettings);
        }

        private static SkDocument CreatePdf(SkWriteStream stream, DocumentMetadata documentMetadata, DocumentSettings documentSettings)
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
                CompressDocument = documentSettings.CompressDocument
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
        
        ~PdfCanvas()
        {
            this.WarnThatFinalizerIsReached();
            Dispose();
        }
        
        public void Dispose()
        {
            Document?.Dispose();
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
            Document.EndPage();
        }
        
        public IDrawingCanvas GetDrawingCanvas()
        {
            return DrawingCanvas;
        }
        
        #endregion
    }
}