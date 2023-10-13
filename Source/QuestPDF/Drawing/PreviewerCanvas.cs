using System.Collections.Generic;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer.LayoutInspection;
using SkiaSharp;

namespace QuestPDF.Drawing
{
    internal class PreviewerPageSnapshot
    {
        public SKPicture Picture { get; set; }
        public Size Size { get; set; }

        public PreviewerPageSnapshot(SKPicture picture, Size size)
        {
            Picture = picture;
            Size = size;
        }
    }
    
    internal class PreviewerDocumentSnapshot
    {
        public ICollection<PreviewerPageSnapshot> Pictures { get; set; }
        public DocumentInspectionElement DocumentHierarchy { get; set; }
    }
    
    internal class PreviewerCanvas : SkiaCanvasBase
    {
        private SKPictureRecorder? PictureRecorder { get; set; }
        private Size? CurrentPageSize { get; set; }

        private ICollection<PreviewerPageSnapshot> PageSnapshots { get; } = new List<PreviewerPageSnapshot>();
        
        public override void BeginDocument()
        {
            PageSnapshots.Clear();
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
                
            };
        }
    }
}