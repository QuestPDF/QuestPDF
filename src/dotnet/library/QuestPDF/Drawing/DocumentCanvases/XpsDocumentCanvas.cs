using System;
using System.Diagnostics;
using QuestPDF.Drawing.DrawingCanvases;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Drawing.DocumentCanvases
{
    internal sealed class XpsDocumentCanvas : IDocumentCanvas, IDisposable
    {
        private SkDocument Document { get; }
        private SkCanvas? CurrentPageCanvas { get; set; }
        private ProxyDrawingCanvas DrawingCanvas { get; } = new();
        
        public XpsDocumentCanvas(SkWriteStream stream, DocumentSettings documentSettings)
        {
            Document = CreateXps(stream, documentSettings);
        }
        
        private static SkDocument CreateXps(SkWriteStream stream, DocumentSettings documentSettings)
        {
            try
            {
                return SkXpsDocument.Create(stream, documentSettings.ImageRasterDpi);
            }
            catch (TypeInitializationException exception)
            {
                throw new InitializationException("XPS", exception);
            }
        }
        
        #region IDisposable
        
        ~XpsDocumentCanvas()
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
        
        public void SetSemanticTree(SemanticTreeNode? semanticTree)
        {
            
        }
        
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