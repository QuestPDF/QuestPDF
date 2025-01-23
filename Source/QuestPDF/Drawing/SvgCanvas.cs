using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Drawing
{
    internal sealed class SvgCanvas : SkiaCanvasBase, IDisposable
    {
        internal SkWriteStream WriteStream { get; set; }
        internal ICollection<string> Images { get; } = new List<string>();
        
        ~SvgCanvas()
        {
            Dispose();
        }

        public void Dispose()
        {
            Canvas?.Dispose();
            WriteStream?.Dispose();
            GC.SuppressFinalize(this);
        }
        
        public override void BeginDocument()
        {
            
        }

        public override void EndDocument()
        {
            Canvas?.Dispose();
            WriteStream?.Dispose();
        }

        public override void BeginPage(Size size)
        {
            WriteStream?.Dispose();
            WriteStream = new SkWriteStream();
            Canvas = SkSvgCanvas.CreateSvg(size.Width, size.Height, WriteStream);
        }

        public override void EndPage()
        {
            Canvas.Save();
            Canvas.Dispose();
            
            using var data = WriteStream.DetachData();
            var svgImage = Encoding.UTF8.GetString(data.ToBytes());
            Images.Add(svgImage);
            
            WriteStream.Dispose();
        }
    }
}