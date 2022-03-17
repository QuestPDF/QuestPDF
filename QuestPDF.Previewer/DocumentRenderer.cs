using QuestPDF.Drawing;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Previewer
{
    internal class DocumentRenderer
    {
        public float PageSpacing { get; set; }
        public Size Bounds { get; private set; }
        public IDocument? Document { get; private set; }
        public SKPicture? Picture { get; private set; }
        public Exception? RenderException { get; private set; }
        public bool IsRendering { get; private set; }

        public void UpdateDocument(IDocument? document)
        {
            Document = document;
            RenderException = null;
            if (document != null)
            {
                try
                {
                    IsRendering = true;
                    RenderDocument(document);
                }
                catch (Exception ex)
                {
                    RenderException = ex;
                    Picture?.Dispose();
                    Picture = null;
                }
                finally
                {
                    IsRendering = false;
                }
            }
        }

        private void RenderDocument(IDocument document)
        {
            Picture?.Dispose();

            var canvas = new PreviewerCanvas()
            {
                PageSpacing = PageSpacing,
            };

            using var recorder = new SKPictureRecorder();
            DocumentGenerator.RenderDocument(canvas, new SizeTrackingCanvas(), document, s =>
            {
                var width = s.PageSizes.Max(p => p.Width);
                var height = s.PageSizes.Sum(p => p.Height) + ((s.PageSizes.Count - 1) * PageSpacing);
                Bounds = new Size(width, height);
                canvas.Canvas = recorder.BeginRecording(new SKRect(0, 0, width, height));
                canvas.MaxPageWidth = width;
            });
            Picture = recorder.EndRecording();
        }
    }
}
