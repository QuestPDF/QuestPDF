using System;
using System.Collections.Generic;
using QuestPDF.Companion;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Drawing
{
    internal class CompanionPageSnapshot
    {
        public SkPicture Picture { get; set; }
        public Size Size { get; set; }

        public CompanionPageSnapshot(SkPicture picture, Size size)
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
    
    internal class CompanionDocumentSnapshot
    {
        public ICollection<CompanionPageSnapshot> Pictures { get; set; }
        public bool DocumentContentHasLayoutOverflowIssues { get; set; }
        public CompanionCommands.UpdateDocumentStructure.DocumentHierarchyElement Hierarchy { get; set; }
    }
    
    internal class CompanionCanvas : SkiaCanvasBase, IDisposable
    {
        private SkPictureRecorder? PictureRecorder { get; set; }
        private Size? CurrentPageSize { get; set; }

        private ICollection<CompanionPageSnapshot> PageSnapshots { get; } = new List<CompanionPageSnapshot>();
        
        internal CompanionCommands.UpdateDocumentStructure.DocumentHierarchyElement Hierarchy { get; set; }
        
        ~CompanionCanvas()
        {
            this.WarnThatFinalizerIsReached();
            Dispose();
        }

        public void Dispose()
        {
            Canvas?.Dispose();
            PictureRecorder?.Dispose();
            // do not dispose PageSnapshots, they are used by the CompanionDocumentSnapshot
            GC.SuppressFinalize(this);
        }
        
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
                PageSnapshots.Add(new CompanionPageSnapshot(picture, CurrentPageSize.Value));

            PictureRecorder?.Dispose();
            PictureRecorder = null;
        }

        public override void EndDocument() { }

        public CompanionDocumentSnapshot GetContent()
        {
            return new CompanionDocumentSnapshot
            {
                Pictures = PageSnapshots,
                DocumentContentHasLayoutOverflowIssues = DocumentContentHasLayoutOverflowIssues,
                Hierarchy = Hierarchy
            };
        }
    }
}