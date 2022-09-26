using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Drawing
{
    internal class SkiaCaptureCanvas : SkiaCanvasBase
    {
        private SKPictureRecorder? PictureRecorder { get; set; }
        private Size? CurrentPageSize { get; set; }
        public SKPicture? CurrentPicture { get; set; }
        
        public override void BeginDocument()
        {
            
        }

        public override void BeginPage(Size size)
        {
            CurrentPageSize = size;
            PictureRecorder = new SKPictureRecorder();

            Canvas = PictureRecorder.BeginRecording(new SKRect(0, 0, size.Width, size.Height));
        }

        public override void EndPage()
        {
            CurrentPicture = PictureRecorder?.EndRecording();
            
            PictureRecorder?.Dispose();
            PictureRecorder = null;
        }

        public override void EndDocument() { }
    }
}