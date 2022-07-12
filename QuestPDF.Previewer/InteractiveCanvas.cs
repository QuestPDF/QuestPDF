using Avalonia;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using DynamicData;
using SkiaSharp;

namespace QuestPDF.Previewer;

class InteractiveCanvas : ICustomDrawOperation
{
    public Rect Bounds { get; set; }
    public ICollection<PreviewPage> Pages { get; set; }
    public InspectionElement? InspectionElement { get; set; }

    private float ViewportWidth => (float)Bounds.Width;
    private float ViewportHeight => (float)Bounds.Height;

    public float Scale { get; private set; } = 1;
    public float TranslateX { get; set; }
    public float TranslateY { get; set; }

    private const float MinScale = 0.1f;
    private const float MaxScale = 10f;

    private const float PageSpacing = 25f;
    private const float SafeZone = 25f;

    public float TotalPagesHeight => Pages.Sum(x => x.Height) + (Pages.Count - 1) * PageSpacing;
    public float TotalHeight => TotalPagesHeight + SafeZone * 2 / Scale;
    public float MaxWidth => Pages.Any() ? Pages.Max(x => x.Width) : 0;
    
    public float MaxTranslateY => TotalHeight - ViewportHeight / Scale;

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
            var viewPortSize = ViewportHeight / Scale / TotalHeight;
            return Math.Clamp(viewPortSize, 0, 1);
        }
    }

    #region transformations
    
    private void LimitScale()
    {
        Scale = Math.Max(Scale, MinScale);
        Scale = Math.Min(Scale, MaxScale);
    }
    
    private void LimitTranslate()
    {
        if (TotalPagesHeight > ViewportHeight / Scale)
        {
            TranslateY = Math.Min(TranslateY, MaxTranslateY);
            TranslateY = Math.Max(TranslateY, 0);
        }
        else
        {
            TranslateY = (TotalPagesHeight - ViewportHeight / Scale) / 2;
        }

        if (ViewportWidth / Scale < MaxWidth)
        {
            var maxTranslateX = (ViewportWidth / 2 - SafeZone) / Scale - MaxWidth / 2;

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

    public int ActivePage { get; set; } = 1;

    public IEnumerable<(int pageNumber, float beginY, float endY)> GetPagePosition()
    {
        var pageNumber = 1;
        var currentPagePosition = SafeZone / Scale;
        
        foreach (var page in Pages)
        {
            yield return (pageNumber, currentPagePosition, currentPagePosition + page.Height);
            currentPagePosition += page.Height + PageSpacing;
            pageNumber++;
        }
    }

    public void SetActivePage(float x, float y)
    {
        y /= Scale;
        y += TranslateY;

        ActivePage = GetPagePosition().FirstOrDefault(p => p.beginY <= y && y <= p.endY).pageNumber;
    }

    public void ScrollToInspectionElement(InspectionElement element)
    {
        var location = element.Location.MinBy(x => x.PageNumber);
        var pagePosition = GetPagePosition().ElementAt(location.PageNumber - 1);
        var page = Pages.ElementAt(location.PageNumber - 1);

        var widthScale = ViewportWidth / location.Width;
        var heightScale = ViewportHeight / location.Height;
        var targetScale = Math.Min(widthScale, heightScale);
        targetScale *= 0.7f; // slightly zoom out to show entire element with padding

        Scale = targetScale; 
        
        TranslateY = pagePosition.beginY + location.Top + location.Height / 2 - ViewportHeight / Scale / 2;
        TranslateX = page.Width / 2 - location.Left - location.Width / 2;
    }
    
    public (int pageNumber, float x, float y)? FindClickedPointOnThePage(float x, float y)
    {
        x -= ViewportWidth / 2;
        x /= Scale;
        x += TranslateX;
        
        y /= Scale;
        y += TranslateY;
        
        var location = GetPagePosition().FirstOrDefault(p => p.beginY <= y && y <= p.endY);

        if (location == default)
            return null;

        var page = Pages.ElementAt(location.pageNumber - 1);
        
        x += page.Width / 2;

        if (x < 0 || page.Width < x)
            return null;
        
        y -= location.beginY;

        return (location.pageNumber, x, y);
    }
    
    #endregion
    
    #region rendering
    
    public void Render(IDrawingContextImpl context)
    {
        if (Pages.Count <= 0)
            return;

        LimitScale();
        LimitTranslate();
        
        var canvas = (context as ISkiaDrawingContextImpl)?.SkCanvas;
        
        if (canvas == null)
            throw new InvalidOperationException($"Context needs to be ISkiaDrawingContextImpl but got {nameof(context)}");

        var originalMatrix = canvas.TotalMatrix;

        canvas.Translate(ViewportWidth / 2, 0);
        canvas.Scale(Scale);
        canvas.Translate(TranslateX, -TranslateY);

        var topMatrix = canvas.TotalMatrix;;

        var positions = GetPagePosition().ToList();
        
        foreach (var pageIndex in Enumerable.Range(0, Pages.Count))
        {
            canvas.SetMatrix(topMatrix);
            
            var page = Pages.ElementAt(pageIndex);
            var position = positions.ElementAt(pageIndex);
            
            canvas.Translate(-page.Width / 2f, position.beginY);
            DrawBlankPage(canvas, page.Width, page.Height);
            canvas.DrawPicture(page.Picture);
            DrawInspectionElement(canvas, pageIndex + 1);
        }

        canvas.SetMatrix(topMatrix);
        DrawActivePage(canvas);
        
        canvas.SetMatrix(originalMatrix);
        
        DrawInnerGradient(canvas);
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
    
    private void DrawBlankPage(SKCanvas canvas, float width, float height)
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
        
        canvas.DrawRect(0, 0, ViewportWidth, InnerGradientSize, fogPaint);
    }

    #endregion

    #region Interactivity

    private void DrawActivePage(SKCanvas canvas)
    {
        if (ActivePage == default)
            return;
        
        var page = Pages.ElementAt(ActivePage - 1);
        var pagePosition = GetPagePosition().ElementAt(ActivePage - 1);

        var thickness = 6f / Scale;

        using var strokePaint = new SKPaint
        {
            StrokeWidth = thickness,
            IsStroke = true,
            Color = SKColor.Parse("#000")
        };
        
        canvas.DrawRect(- page.Width / 2 -thickness / 2, pagePosition.beginY -thickness / 2, page.Width + thickness, page.Height + thickness, strokePaint);
    }
    
    private void DrawInspectionElement(SKCanvas canvas, int pageNumber)
    {
        if (InspectionElement == null || InspectionElement.Location == null)
            return;

        var location = InspectionElement.Location.FirstOrDefault(x => x.PageNumber == pageNumber);

        if (location == null)
            return;

        var size = 4 / Scale;
        size = Math.Min(size, 2);

        using var strokePaint = new SKPaint
        {
            StrokeWidth = size,
            IsStroke = true,
            PathEffect = SKPathEffect.CreateDash(new[] { size * 4, size * 2 }, 0),
            Color = SKColor.Parse("#444"),
            StrokeJoin = SKStrokeJoin.Round
        };
        
        using var backgroundPaint = new SKPaint
        {
            Color = SKColor.Parse("#4444"),
        };
        
        canvas.DrawRect(location.Left, location.Top, location.Width, location.Height, backgroundPaint);
        canvas.DrawRect(location.Left + size / 2, location.Top + size / 2, location.Width - size, location.Height - size, strokePaint);
    }

    #endregion
}
