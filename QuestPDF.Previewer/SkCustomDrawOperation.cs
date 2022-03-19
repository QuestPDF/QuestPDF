using Avalonia;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;

namespace QuestPDF.Previewer
{
    internal sealed class SkCustomDrawOperation : ICustomDrawOperation
    {
        private readonly Action<SKCanvas> _renderFunc;
        public Rect Bounds { get; }

        public SkCustomDrawOperation(Rect bounds, Action<SKCanvas> renderFunc)
        {
            Bounds = bounds;
            _renderFunc = renderFunc;
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
            var canvas = (context as ISkiaDrawingContextImpl)?.SkCanvas;
            if (canvas == null)
                throw new InvalidOperationException($"Context needs to be ISkiaDrawingContextImpl but got {nameof(context)}");

            _renderFunc(canvas);
        }
    }
}
