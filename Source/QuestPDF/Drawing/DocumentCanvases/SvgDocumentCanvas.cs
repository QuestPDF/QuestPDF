using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        
        private SkWriteStream WriteStream { get; set; }
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
        }

        public void BeginPage(Size size)
        {
            WriteStream?.Dispose();
            WriteStream = new SkWriteStream();
            CurrentPageCanvas = SkSvgCanvas.CreateSvg(size.Width, size.Height, WriteStream);
            
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
            
            using var data = WriteStream.DetachData();
            var svgImage = Encoding.UTF8.GetString(data.ToBytes());
            Images.Add(svgImage);
            
            WriteStream.Dispose();
        }
        
        public IDrawingCanvas GetDrawingCanvas()
        {
            return DrawingCanvas;
        }
        
        #endregion
    }
}