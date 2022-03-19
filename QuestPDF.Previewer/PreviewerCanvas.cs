using QuestPDF.Drawing;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Previewer
{
    internal sealed class PreviewerCanvas : SkiaCanvasBase, IRenderingCanvas
    {
        private SKPictureRecorder? _currentRecorder;

        private readonly List<RenderedPageInfo> _pictures = new();
        public IReadOnlyList<RenderedPageInfo> Pictures => _pictures;

        private Size? _currentSize;

        public override void BeginDocument()
        {
            _pictures.Clear();
        }

        public override void BeginPage(Size size)
        {
            _currentSize = size;
            _currentRecorder = new SKPictureRecorder();
            Canvas = _currentRecorder.BeginRecording(new SKRect(0, 0, size.Width, size.Height));

            using var paint = new SKPaint() { Color = SKColors.White };
            Canvas.DrawRect(0, 0, size.Width, size.Height, paint);
        }

        public override void EndPage()
        {
            var picture = _currentRecorder?.EndRecording();
            if (picture != null && _currentSize.HasValue)
                _pictures.Add(new RenderedPageInfo(picture, _currentSize.Value));

            _currentRecorder?.Dispose();
            _currentRecorder = null;
        }

        public override void EndDocument() { }
    }
}
