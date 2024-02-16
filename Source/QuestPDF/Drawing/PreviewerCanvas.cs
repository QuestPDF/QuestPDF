using System;
using System.Collections.Generic;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Drawing
{
    internal class PreviewerPageSnapshot
    {
        public SkPicture Picture { get; set; }
        public Size Size { get; set; }

        public PreviewerPageSnapshot(SkPicture picture, Size size)
        {
            Picture = picture;
            Size = size;
        }
        
        public byte[] RenderImage(int zoomLevel)
        {
            var scale = (float)Math.Pow(2, zoomLevel);
            
            using var bitmap = new SkBitmap((int)(Size.Width * scale), (int)(Size.Height * scale));
            using var canvas = SkCanvas.CreateFromBitmap(bitmap);
            canvas.Scale(scale, scale);
            canvas.DrawPicture(Picture);
            return bitmap.EncodeAsJpeg(90).ToBytes();
        }
    }
    
    internal class PreviewerDocumentSnapshot
    {
        public ICollection<PreviewerPageSnapshot> Pictures { get; set; }
        public bool DocumentContentHasLayoutOverflowIssues { get; set; }
    }
    
    internal class PreviewerCanvas : SkiaCanvasBase
    {
        private SkPictureRecorder? PictureRecorder { get; set; }
        private Size? CurrentPageSize { get; set; }

        private ICollection<PreviewerPageSnapshot> PageSnapshots { get; } = new List<PreviewerPageSnapshot>();
        
        public override void BeginDocument()
        {
            PageSnapshots.Clear();
        }

        public override void BeginPage(Size size)
        {
            CurrentPageSize = size;
            PictureRecorder = new SkPictureRecorder();

            Canvas = PictureRecorder.BeginRecording(size.Width, size.Height);
        }

        public override void EndPage()
        {
            var picture = PictureRecorder?.EndRecording();
            
            if (picture != null && CurrentPageSize.HasValue)
                PageSnapshots.Add(new PreviewerPageSnapshot(picture, CurrentPageSize.Value));

            PictureRecorder?.Dispose();
            PictureRecorder = null;
        }

        public override void EndDocument() { }

        public PreviewerDocumentSnapshot GetContent()
        {
            return new PreviewerDocumentSnapshot
            {
                Pictures = PageSnapshots,
                DocumentContentHasLayoutOverflowIssues = DocumentContentHasLayoutOverflowIssues,
            };
        }
    }
}