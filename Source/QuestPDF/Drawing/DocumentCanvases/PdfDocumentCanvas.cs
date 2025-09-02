using System;
using System.Diagnostics;
using System.Linq;
using QuestPDF.Drawing.DrawingCanvases;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Drawing.DocumentCanvases
{
    internal sealed class PdfDocumentCanvas : IDocumentCanvas, IDisposable
    {
        private SkWriteStream WriteStream { get; }
        private DocumentMetadata DocumentMetadata { get; }
        private DocumentSettings DocumentSettings { get; }
        private SkPdfTag? SemanticTag { get; set; }
        
        private SkDocument? Document { get; set; }
        private SkCanvas? CurrentPageCanvas { get; set; }
        private ProxyDrawingCanvas DrawingCanvas { get; } = new();
        
        public PdfDocumentCanvas(SkWriteStream stream, DocumentMetadata documentMetadata, DocumentSettings documentSettings)
        {
            WriteStream = stream;
            DocumentMetadata = documentMetadata;
            DocumentSettings = documentSettings;
        }

        private SkDocument CreatePdf()
        {
            // do not extract to another method, as it will cause the SkText objects
            // to be disposed before the SkPdfDocument is created
            using var title = new SkText(DocumentMetadata.Title);
            using var author = new SkText(DocumentMetadata.Author);
            using var subject = new SkText(DocumentMetadata.Subject);
            using var keywords = new SkText(DocumentMetadata.Keywords);
            using var creator = new SkText(DocumentMetadata.Creator);
            using var producer = new SkText(DocumentMetadata.Producer);
            using var language = new SkText(DocumentMetadata.Language);
            
            var internalMetadata = new SkPdfDocumentMetadata
            {
                Title = title,
                Author = author,
                Subject = subject,
                Keywords = keywords,
                Creator = creator,
                Producer = producer,
                Language = language,
                
                CreationDate = new SkDateTime(DocumentMetadata.CreationDate),
                ModificationDate = new SkDateTime(DocumentMetadata.ModifiedDate),
                
                RasterDPI = DocumentSettings.ImageRasterDpi,
                SupportPDFA = DocumentSettings.PdfA,
                CompressDocument = DocumentSettings.CompressDocument,
                
                SemanticNodeRoot = SemanticTag?.Instance ?? IntPtr.Zero
            };
            
            try
            {
                return SkPdfDocument.Create(WriteStream, internalMetadata);
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
            
            // don't dispose WriteStream - its lifetime is managed externally
            SemanticTag?.Dispose();
            
            GC.SuppressFinalize(this);
        }
        
        #endregion
        
        #region IDocumentCanvas
        
        public void SetSemanticTree(SemanticTreeNode? semanticTree)
        {
            if (semanticTree == null)
            {
                SemanticTag?.Dispose();
                SemanticTag = null;
                return;
            }
            
            SemanticTag = Convert(semanticTree);
            
            static SkPdfTag Convert(SemanticTreeNode node)
            {
                var result = SkPdfTag.Create(node.NodeId, node.Type, node.Alt, node.Lang);
                var children = node.Children.Select(Convert).ToArray();
                result.SetChildren(children);
                
                return result;
            }
        }
        
        public void BeginDocument()
        {
            Document ??= CreatePdf();
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