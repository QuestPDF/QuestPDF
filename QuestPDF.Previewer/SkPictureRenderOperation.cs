using Avalonia;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;

namespace QuestPDF.Previewer
{
    internal class SkPictureRenderOperation : ICustomDrawOperation
    {
        public SKPicture? Picture { get; }
        public Rect Bounds { get; }

        public SkPictureRenderOperation(SKPicture picture, Rect bounds)
        {
            Picture = picture;
            Bounds = bounds;
        }

        public void Dispose() { }

        public bool Equals(ICustomDrawOperation? other)
        {
            return false;
        }

        public bool HitTest(Point p)
        {
            return false;
        }

        public void Render(IDrawingContextImpl context)
        {
            if (Picture == null)
                return;

            var canvas = (context as ISkiaDrawingContextImpl)?.SkCanvas;
            if (canvas == null)
                throw new InvalidOperationException($"Context needs to be ISkiaDrawingContextImpl but got {nameof(context)}");

            canvas.DrawPicture(Picture);
        }
    }
}
