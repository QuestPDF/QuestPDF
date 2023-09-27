using Avalonia;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;

namespace QuestPDF.Previewer;

class InteractiveCanvas : ICustomDrawOperation
{
    public Rect Bounds { get; set; }
    public ICollection<DocumentSnapshot.PageSnapshot> Pages { get; set; }

    private float Width => (float)Bounds.Width;
    private float Height => (float)Bounds.Height;

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

        canvas.Translate(Width / 2, 0);
        
        canvas.Scale(Scale);
        canvas.Translate(TranslateX, -TranslateY + SafeZone / Scale);
        
        foreach (var page in Pages)
        {
            canvas.Translate(-page.Width / 2f, 0);
            DrawBlankPage(canvas, page.Width, page.Height);
            DrawPageSnapshot(canvas, page);
            canvas.Translate(page.Width / 2f, page.Height + PageSpacing);
        }

        canvas.SetMatrix(originalMatrix);
        DrawInnerGradient(canvas);
    }

    private static void DrawPageSnapshot(SKCanvas canvas, DocumentSnapshot.PageSnapshot pageSnapshot)
    {
        canvas.Save();
        canvas.ClipRect(new SKRect(0, 0, pageSnapshot.Width, pageSnapshot.Height));
        canvas.DrawPicture(pageSnapshot.Picture);
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
        
        canvas.DrawRect(0, 0, Width, InnerGradientSize, fogPaint);
    }

    #endregion
}
