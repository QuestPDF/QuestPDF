using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace QuestPDF.Previewer
{
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
        
        public static readonly StyledProperty<InspectionElement> HierarchyProperty = AvaloniaProperty.Register<PreviewerControl, InspectionElement>(nameof(Hierarchy));
        
        public InspectionElement Hierarchy
        {
            get => GetValue(HierarchyProperty);
            set => SetValue(HierarchyProperty, value);
        }
        
        public static readonly StyledProperty<InspectionElement> CurrentSelectionProperty = AvaloniaProperty.Register<PreviewerControl, InspectionElement>(nameof(CurrentSelection));
        
        public InspectionElement CurrentSelection
        {
            get => GetValue(CurrentSelectionProperty);
            set => SetValue(CurrentSelectionProperty, value);
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

            CurrentSelectionProperty.Changed.Subscribe(x =>
            {
                InteractiveCanvas.InspectionElement = CurrentSelection;
                //InteractiveCanvas.ScrollToInspectionElement(CurrentSelection);
                InvalidateVisual();
            });
            
            ClipToBounds = true;

            PointerPressed += (sender, args) =>
            {
                var position = args.GetPosition(this);
                InteractiveCanvas.SetActivePage((float)position.X, (float)position.Y);

                var clickedPosition = InteractiveCanvas.FindClickedPointOnThePage((float)position.X, (float)position.Y);
                
                if (clickedPosition != null) 
                    FindHighlightedElement(clickedPosition.Value.pageNumber, clickedPosition.Value.x, clickedPosition.Value.y);
                
                InvalidateVisual();
            };
        }

        void FindHighlightedElement(int pageNumber, float x, float y)
        {
            var possible = FlattenHierarchy(Hierarchy, 0)
                .Select(x =>
                {
                    var location = x.element.Location.First(y => y.PageNumber == pageNumber);

                    return new
                    {
                        Element = x.element,
                        Level = x.level,
                        Size = location.Width * location.Height
                    };
                })
                .ToList();

            var minSize = possible.Min(x => x.Size);

            CurrentSelection = possible
                .Where(x => Math.Abs(x.Size - minSize) < 1)
                .OrderByDescending(x => x.Level)
                .First()
                .Element;

            IEnumerable<(InspectionElement element, int level)> FlattenHierarchy(InspectionElement element, int level)
            {
                var location = element.Location.FirstOrDefault(x => x.PageNumber == pageNumber);
                
                if (location == null)
                    yield break;

                if (x < location.Left || location.Left + location.Width < x)
                    yield break;

                if (y < location.Top || location.Top + location.Height < y)
                    yield break;
                
                yield return (element, level);
                
                foreach (var childIndex in Enumerable.Range(0, element.Children.Count))
                foreach (var nestedChild in FlattenHierarchy(element.Children[childIndex], level + childIndex + 1))
                    yield return nestedChild;
            }
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
}
