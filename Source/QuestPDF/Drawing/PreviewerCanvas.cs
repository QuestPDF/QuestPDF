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