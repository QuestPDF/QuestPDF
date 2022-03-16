using Avalonia;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Previewer
{
    internal class DocumentRenderOperation : ICustomDrawOperation
    {
        public IDocument Document { get; }
        public float PageSpacing { get; }
        public Rect Bounds { get; }

        public DocumentRenderOperation(IDocument document, Rect bounds, float pageSpacing)
        {
            Document = document;
            Bounds = bounds;
            PageSpacing = pageSpacing;
        }

        public void Dispose() { }

        public bool Equals(ICustomDrawOperation? other)
        {
            return other is DocumentRenderOperation renderer && renderer.Document == Document;
        }

        public bool HitTest(Point p)
        {
            return false;
        }

        public void Render(IDrawingContextImpl context)
        {
            var canvas = (context as ISkiaDrawingContextImpl)?.SkCanvas;
            if (canvas == null)
                throw new InvalidOperationException($"Context needs to be ISkiaDrawingContextImpl but got {nameof(context)}");

            using (new SKAutoCanvasRestore(canvas))
            {
                var previewerCanvas = new PreviewerCanvas()
                {
                    Canvas = canvas,
                    PageSpacing = PageSpacing,
                    MaxPageWidth = (float)Bounds.Width,
                };

                DocumentGenerator.RenderDocument(previewerCanvas, Document);
            }
        }
    }
}
