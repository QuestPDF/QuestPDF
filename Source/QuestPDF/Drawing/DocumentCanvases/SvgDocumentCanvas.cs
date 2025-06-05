using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using QuestPDF.Drawing.DrawingCanvases;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Drawing.DocumentCanvases
{
    internal sealed class SvgDocumentCanvas : IDocumentCanvas, IDisposable
    {
        private SkCanvas? CurrentPageCanvas { get; set; }
        private ProxyDrawingCanvas DrawingCanvas { get; } = new();
        
        private MemoryStream WriteStream { get; set; }
        private SkWriteStream SkiaStream { get; set; }
        internal ICollection<string> Images { get; } = new List<string>();
        
        #region IDisposable
        
        ~SvgDocumentCanvas()
        {
            this.WarnThatFinalizerIsReached();
            Dispose();
        }
        
        public void Dispose()
        {
            CurrentPageCanvas?.Dispose();
            WriteStream?.Dispose();
            SkiaStream?.Dispose();
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
            CurrentPageCanvas?.Dispose();
            WriteStream?.Dispose();
            SkiaStream?.Dispose();
        }

        public void BeginPage(Size size)
        {
            WriteStream?.Dispose();
            SkiaStream?.Dispose();
            
            WriteStream = new MemoryStream();
            SkiaStream = new SkWriteStream(WriteStream);
            CurrentPageCanvas = SkSvgCanvas.CreateSvg(size.Width, size.Height, SkiaStream);
            
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
            
            SkiaStream.Flush();

            var data = WriteStream.ToArray();
            var svgImage = Encoding.UTF8.GetString(data);
            Images.Add(svgImage);
            
            SkiaStream.Dispose();
            WriteStream.Dispose();
        }
        
        public IDrawingCanvas GetDrawingCanvas()
        {
            return DrawingCanvas;
        }
        
        #endregion
    }
}