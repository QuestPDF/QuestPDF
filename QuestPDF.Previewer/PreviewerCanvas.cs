using QuestPDF.Drawing;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Previewer
{
    internal sealed class PreviewerCanvas : SkiaCanvasBase, IRenderingCanvas
    {
        private bool _isFirstPage;
        private Size? _lastPageSize;
        private float? _distanceToCenter;

        public float PageSpacing { get; set; }
        public float MaxPageWidth { get; set; }

        public override void BeginDocument()
        {
            _isFirstPage = true;
        }

        public override void BeginPage(Size size)
        {
            _lastPageSize = size;

            if (!_isFirstPage)
                Canvas.Translate(0, PageSpacing);

            if (MaxPageWidth > 0)
            {
                _distanceToCenter = (MaxPageWidth - size.Width) / 2;
                Canvas.Translate(_distanceToCenter.Value, 0);
            }

            using var paint = new SKPaint() { Color = SKColors.White };
            Canvas.DrawRect(0, 0, size.Width, size.Height, paint);

            _isFirstPage = false;
        }

        public override void EndDocument() { }
        public override void EndPage()
        {
            if (_lastPageSize.HasValue)
                Canvas.Translate(0, _lastPageSize.Value.Height);

            if (_distanceToCenter.HasValue)
                Canvas.Translate(-_distanceToCenter.Value, 0);
        }
    }
}
