using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace QuestPDF.Previewer;

class PreviewerControl : Control
{
    private InteractiveCanvas InteractiveCanvas { get; set; } = new ();
        
    public static readonly StyledProperty<ObservableCollection<PreviewPage>> PagesProperty =
        AvaloniaProperty.Register<PreviewerControl, ObservableCollection<PreviewPage>>(nameof(Pages));
        
    public ObservableCollection<PreviewPage>? Pages
    {
        get => GetValue(PagesProperty);
        set => SetValue(PagesProperty, value);
    }

    public static readonly StyledProperty<float> CurrentScrollProperty = AvaloniaProperty.Register<PreviewerControl, float>(nameof(CurrentScroll));
        
    public float CurrentScroll
    {
        get => GetValue(CurrentScrollProperty);
        set => SetValue(CurrentScrollProperty, value);
    }
        
    public static readonly StyledProperty<float> ScrollViewportSizeProperty = AvaloniaProperty.Register<PreviewerControl, float>(nameof(ScrollViewportSize));
        
    public float ScrollViewportSize
    {
        get => GetValue(ScrollViewportSizeProperty);
        set => SetValue(ScrollViewportSizeProperty, value);
    }
        
    public PreviewerControl()
    {
        PagesProperty.Changed.Subscribe(x =>
        {
            InteractiveCanvas.Pages = x.NewValue.Value;
            InvalidateVisual();
        });

        CurrentScrollProperty.Changed.Subscribe(x =>
        {
            InteractiveCanvas.ScrollPercentY = x.NewValue.Value;
            InvalidateVisual();
        });

        ClipToBounds = true;
    }
        
    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);

        if ((e.KeyModifiers & KeyModifiers.Control) != 0)
        {
            var scaleFactor = 1 + e.Delta.Y / 10f;
            var point = new Point(Bounds.Center.X, Bounds.Top) - e.GetPosition(this);
                
            InteractiveCanvas.ZoomToPoint((float)point.X, -(float)point.Y, (float)scaleFactor);
        }
                
        if (e.KeyModifiers == KeyModifiers.None)
        {
            var translation = (float)e.Delta.Y * 25;
            InteractiveCanvas.TranslateWithCurrentScale(0, -translation);
        }
                
        InvalidateVisual();
    }

    private bool IsMousePressed { get; set; }
    private Vector MousePosition { get; set; }
        
    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (IsMousePressed)
        {
            var currentPosition = e.GetPosition(this);
            var translation = currentPosition - MousePosition;
            InteractiveCanvas.TranslateWithCurrentScale((float)translation.X, -(float)translation.Y);
                
            InvalidateVisual();
        }

        MousePosition = e.GetPosition(this);
    }
        
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        IsMousePressed = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        IsMousePressed = false;
    }

    public override void Render(DrawingContext context)
    {
        CurrentScroll = InteractiveCanvas.ScrollPercentY;
        ScrollViewportSize = InteractiveCanvas.ScrollViewportSizeY;
    
        InteractiveCanvas.Bounds = new Rect(0, 0, Bounds.Width, Bounds.Height);

        context.Custom(InteractiveCanvas);
        base.Render(context);
    }
}