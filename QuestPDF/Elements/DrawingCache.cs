using System.Collections.Generic;
using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Elements
{
    internal class DrawingCache : ContainerElement
    {
        private SkiaCaptureCanvas CaptureCanvas { get; } = new();
        private Dictionary<int, SpacePlan> MeasureCache { get; } = new();
        private Dictionary<int, SKPicture> DrawCache { get; } = new();
        
        ~DrawingCache()
        {
            foreach (var picture in DrawCache.Values)
                picture.Dispose();

            DrawCache.Clear();
        }
        
        internal override void Initialize(IPageContext pageContext, ICanvas canvas)
        {
            Child.VisitChildren(x => x.Canvas = CaptureCanvas);
            base.Initialize(pageContext, canvas);
        }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            var cacheKey = PageContext.CurrentPage;

            if (MeasureCache.TryGetValue(cacheKey, out var result))
                return result;

            var childSize = Child?.Measure(availableSpace) ?? SpacePlan.FullRender(Size.Zero);
            MeasureCache.Add(cacheKey, childSize);
            return childSize;
        }
        
        internal override void Draw(Size availableSpace)
        {
            var cacheKey = PageContext.CurrentPage;

            if (DrawCache.TryGetValue(cacheKey, out var result))
            {
                Canvas.DrawPicture(result);
                return;
            }
            
            CaptureCanvas.BeginPage(availableSpace);
            Child?.Draw(availableSpace);
            CaptureCanvas.EndPage();

            var picture = CaptureCanvas.CurrentPicture;
            DrawCache.Add(cacheKey, picture);
        }
    }
}