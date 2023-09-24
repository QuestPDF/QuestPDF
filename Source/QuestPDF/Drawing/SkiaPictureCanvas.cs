using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Drawing
{
    internal class PreviewerPicture
    {
        public SKPicture Picture { get; set; }
        public Size Size { get; set; }

        public PreviewerPicture(SKPicture picture, Size size)
        {
            Picture = picture;
            Size = size;
        }
    }
    
    internal class SkiaPictureCanvas : SkiaCanvasBase
    {
        private SKPictureRecorder? PictureRecorder { get; set; }
        private Size? CurrentPageSize { get; set; }

        public IList<PreviewerPicture> Pictures { get; } = new List<PreviewerPicture>();
        
        public override void BeginDocument()
        {
            Pictures.Clear();
        }

        public override void BeginPage(Size size)
        {
            CurrentPageSize = size;
            PictureRecorder = new SKPictureRecorder();

            Canvas = PictureRecorder.BeginRecording(new SKRect(0, 0, size.Width, size.Height));
        }

        public override void EndPage()
        {
            var picture = PictureRecorder?.EndRecording();
            
            if (picture != null && CurrentPageSize.HasValue)
                Pictures.Add(new PreviewerPicture(picture, CurrentPageSize.Value));

            PictureRecorder?.Dispose();
            PictureRecorder = null;
        }

        public override void EndDocument() { }
    }
}