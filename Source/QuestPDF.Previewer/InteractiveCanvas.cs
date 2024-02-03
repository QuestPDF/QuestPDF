using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;

namespace QuestPDF.Previewer;

class InteractiveCanvas : ICustomDrawOperation
{
    public Rect Bounds { get; set; }
    public float RenderingScale { get; set; }

    private List<DocumentStructure.PageSize> PageSizes { get; set; } = new();
    private List<RenderedPageSnapshot> PageSnapshotCache { get; set; } = new();

    private float Width => (float)Bounds.Width;
    private float Height => (float)Bounds.Height;

    public float Scale { get; private set; } = 1;
    public float TranslateX { get; set; }
    public float TranslateY { get; set; }

    private const float MinScale = 1 / 8f;
    private const float MaxScale = 8f;

    private const float PageSpacing = 25f;
    private const float SafeZone = 25f;

    public float TotalPagesHeight => PageSizes.Sum(x => x.Height) + (PageSizes.Count - 1) * PageSpacing;
    public float TotalHeight => TotalPagesHeight + SafeZone * 2 / Scale;
    public float MaxWidth => PageSizes.Any() ? PageSizes.Max(x => x.Width) : 0;
    
    public float MaxTranslateY => TotalHeight - Height / Scale;

    public float ScrollPercentY
    {
        get
        {
            return TranslateY / MaxTranslateY;
        }
        set
        {
            TranslateY = value * MaxTranslateY;
        }
    }

    public float ScrollViewportSizeY
    {
        get
        {
            var viewPortSize = Height / Scale / TotalHeight;
            return Math.Clamp(viewPortSize, 0, 1);
        }
    }

    #region interaction

    public void SetNewDocumentStructure(DocumentStructure document)
    {
        foreach (var renderedSnapshot in PageSnapshotCache)
            renderedSnapshot.Image.Dispose();
        
        PageSnapshotCache.Clear();
        PageSizes = document.Pages.ToList();
    }
    
    public ICollection<PageSnapshotIndex> GetMissingSnapshots()
    {
        var requiredKeys = GetVisiblePages(padding: 500).Select(x => (x.pageIndex, PreferredZoomLevel)).ToList();
        var availableKeys = PageSnapshotCache.Select(x => (x.PageIndex, x.ZoomLevel)).ToList();

        var missingKeys = requiredKeys.Except(availableKeys).ToArray();

        return missingKeys
            .Select(x => new PageSnapshotIndex
            {
                PageIndex = x.Item1,
                ZoomLevel = x.Item2
            })
            .ToList();
    }

    public void AddSnapshots(ICollection<RenderedPageSnapshot> snapshots)
    {
        PageSnapshotCache.AddRange(snapshots);
    }
    
    #endregion
    
    #region transformations
    
    private void LimitScale()
    {
        Scale = Math.Max(Scale, MinScale);
        Scale = Math.Min(Scale, MaxScale);
    }
    
    private void LimitTranslate()
    {
        if (TotalPagesHeight > Height / Scale)
        {
            TranslateY = Math.Min(TranslateY, MaxTranslateY);
            TranslateY = Math.Max(TranslateY, 0);
        }
        else
        {
            TranslateY = (TotalPagesHeight - Height / Scale) / 2;
        }

        if (Width / Scale < MaxWidth)
        {
            var maxTranslateX = (Width / 2 - SafeZone) / Scale - MaxWidth / 2;

            TranslateX = Math.Min(TranslateX, -maxTranslateX);
            TranslateX = Math.Max(TranslateX, maxTranslateX);
        }
        else
        {
            TranslateX = 0;
        }
    }

    public void TranslateWithCurrentScale(float x, float y)
    {
        TranslateX += x / Scale;
        TranslateY += y / Scale;

        LimitTranslate();
    }
    
    public void ZoomToPoint(float x, float y, float factor)
    {
        var oldScale = Scale;
        Scale *= factor;
                
        LimitScale();
   
        TranslateX -= x / Scale - x / oldScale;
        TranslateY -= y / Scale - y / oldScale;

        LimitTranslate();
    }
    
    #endregion
    
    #region rendering

    private int PreferredZoomLevel => (int)Math.Clamp(Math.Ceiling(Math.Log2(Scale * RenderingScale)), -2, 2);
    
    private IEnumerable<(int pageIndex, float verticalOffset)> GetVisiblePages(float padding = 100)
    {
        padding /= Scale;
        
        var visibleOffsetFrom = TranslateY - padding;
        var visibleOffsetTo = TranslateY + Height / Scale + padding;
        
        var topOffset = 0f;

        foreach (var pageIndex in Enumerable.Range(0, PageSizes.Count))
        {
            var page = PageSizes.ElementAt(pageIndex);
            
            if (topOffset + page.Height > visibleOffsetFrom)
                yield return (pageIndex, topOffset);
            
            topOffset += page.Height + PageSpacing;
            
            if (topOffset > visibleOffsetTo)
                yield break;
        }
    }
    
    public void Render(ImmediateDrawingContext context)
    {
        // get SkiaSharp canvas
        var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
        using var lease = leaseFeature.Lease();

        var canvas = lease.SkCanvas;

        if (canvas == null)
            return;
        
        // draw document
        if (PageSizes.Count <= 0)
            return;
  
        LimitScale();
        LimitTranslate();

        var originalMatrix = canvas.TotalMatrix;

        canvas.Translate(Width / 2, 0);
        
        canvas.Scale(Scale);
        canvas.Translate(TranslateX, -TranslateY + SafeZone / Scale);

        foreach (var (pageIndex, offset) in GetVisiblePages())
        {
            var pageSize = PageSizes.ElementAt(pageIndex);
            
            canvas.Save();
            canvas.Translate(-pageSize.Width / 2f, offset);
            DrawBlankPage(canvas, pageSize.Width, pageSize.Height);
            DrawPageSnapshot(canvas, pageIndex);
            canvas.Restore();
        }

        canvas.SetMatrix(originalMatrix);
        DrawInnerGradient(canvas);
    }
    
    private void DrawPageSnapshot(SKCanvas canvas, int pageIndex)
    {
        var page = PageSizes.ElementAt(pageIndex);

        var renderedSnapshot = PageSnapshotCache
            .Where(x => x.PageIndex == pageIndex)
            .OrderBy(x => Math.Abs(PreferredZoomLevel - x.ZoomLevel))
            .ThenByDescending(x => x.ZoomLevel)
            .FirstOrDefault();
        
        if (renderedSnapshot == null)
            return;
        
        using var drawImagePaint = new SKPaint
        {
            FilterQuality = SKFilterQuality.High
        };

        var renderingScale = (float)Math.Pow(2, renderedSnapshot.ZoomLevel);
        
        canvas.Save();
        canvas.ClipRect(new SKRect(0, 0, page.Width, page.Height));
        canvas.Scale(1 / renderingScale);
        canvas.DrawImage(renderedSnapshot.Image, SKPoint.Empty, drawImagePaint);
        canvas.Restore();
    }
    
    public void Dispose() { }
    public bool Equals(ICustomDrawOperation? other) => false;
    public bool HitTest(Point p) => true;

    #endregion
    
    #region blank page

    private static SKPaint BlankPagePaint = new SKPaint
    {
        Color = SKColors.White
    };
    
    private static SKPaint BlankPageShadowPaint = new SKPaint
    {
        ImageFilter = SKImageFilter.CreateBlendMode(
            SKBlendMode.Overlay, 
            SKImageFilter.CreateDropShadowOnly(0, 6, 6, 6, SKColors.Black.WithAlpha(64)),
            SKImageFilter.CreateDropShadowOnly(0, 10, 14, 14, SKColors.Black.WithAlpha(32)))
    };
    
    private static void DrawBlankPage(SKCanvas canvas, float width, float height)
    {
        canvas.DrawRect(0, 0, width, height, BlankPageShadowPaint);
        canvas.DrawRect(0, 0, width, height, BlankPagePaint);
    }
    
    #endregion

    #region inner viewport gradient

    private const int InnerGradientSize = (int)SafeZone;
    private static readonly SKColor InnerGradientColor = SKColor.Parse("#666");
    
    private void DrawInnerGradient(SKCanvas canvas)
    {
        // gamma correction
        var colors = Enumerable
            .Range(0, InnerGradientSize)
            .Select(x => 1f - x / (float) InnerGradientSize)
            .Select(x => Math.Pow(x, 2f))
            .Select(x => (byte)(x * 255))
            .Select(x => InnerGradientColor.WithAlpha(x))
            .ToArray();
        
        using var fogPaint = new SKPaint
        {
            Shader = SKShader.CreateLinearGradient(
                new SKPoint(0, 0),
                new SKPoint(0, InnerGradientSize), 
                colors,
                SKShaderTileMode.Clamp)
        };
        
        canvas.DrawRect(0, 0, Width, InnerGradientSize, fogPaint);
    }

    #endregion
}
