using QuestPDF.Drawing;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Previewer
{
    record PreviewPage(SKPicture Picture, Size Size);
    
    sealed class PreviewerCanvas : SkiaCanvasBase, IRenderingCanvas
    {
        private SKPictureRecorder? PictureRecorder { get; set; }
        private Size? CurrentPageSize { get; set; }
        
        public ICollection<PreviewPage> Pictures { get; } = new List<PreviewPage>();
        
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
                Pictures.Add(new PreviewPage(picture, CurrentPageSize.Value));

            PictureRecorder?.Dispose();
            PictureRecorder = null;
        }

        public override void EndDocument() { }
    }
}
