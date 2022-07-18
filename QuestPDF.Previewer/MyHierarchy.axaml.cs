using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Color = System.Drawing.Color;

namespace QuestPDF.Previewer
{
    public partial class MyHierarchy : UserControl
    {
        public static readonly StyledProperty<InspectionElement> HierarchyProperty =
            AvaloniaProperty.Register<MyHierarchy, InspectionElement>(nameof(Hierarchy));

        public InspectionElement Hierarchy
        {
            get => GetValue(HierarchyProperty);
            set => SetValue(HierarchyProperty, value);
        }
        
        public static readonly StyledProperty<InspectionElement?> SelectionProperty =
            AvaloniaProperty.Register<MyHierarchy, InspectionElement?>(nameof(Selection));

        public InspectionElement? Selection
        {
            get => GetValue(SelectionProperty);
            set => SetValue(SelectionProperty, value);
        }

        public event Action<InspectionElement>? OnSelected;
        
        public bool Extended { get; set; }
        
        public MyHierarchy()
        {
            InitializeComponent();

            HierarchyProperty.Changed.Subscribe(x =>
            {
                Configure();
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            Configure();
        }

        private void Configure()
        {
            var panel = this.FindControl<Panel>("Panel");
            panel.IsVisible = Hierarchy?.Children?.Any() ?? false;
            
            this.FindControl<Panel>("Indentation").Width = (Hierarchy?.Level ?? 0) * 24;

            UpdateIcon();

            this.FindControl<TextBlock>("ElementName").Text = Hierarchy?.Text;
            this.FindControl<TextBlock>("ElementName").Foreground = new SolidColorBrush(Avalonia.Media.Color.Parse(Hierarchy?.FontColor ?? "#FFF"));
        }

        private const string ExtendedIcon = "M7,15L12,10L17,15H7Z";
        private const string CollapsedIcon = "M7,10L12,15L17,10H7Z";
        
        private void Toggle(object? sender, PointerPressedEventArgs e)
        {
            Extended = !Extended;
            
            UpdateIcon();
            this.FindControl<ItemsRepeater>("Repeater").Items = Extended ? Hierarchy?.Children : Array.Empty<InspectionElement>();
        }

        private void UpdateIcon()
        {
            var iconControl = this.FindControl<Avalonia.Controls.Shapes.Path>("Icon");

            iconControl.Data = new PathGeometry
            {
                Figures = PathFigures.Parse(Extended ? ExtendedIcon : CollapsedIcon)
            };
        }

        private void InputElement_OnDoubleTapped(object? sender, RoutedEventArgs e)
        {
            Toggle(null, null);
        }

        private void Highlight_OnPointerEnter(object? sender, PointerEventArgs e)
        {
            Cursor = Cursor.Parse("hand");
            this.FindControl<Panel>("Highlight").Background = new SolidColorBrush(Avalonia.Media.Color.Parse("#1FFF"));
        }

        private void Indentation_OnPointerLeave(object? sender, PointerEventArgs e)
        {
            Cursor = Cursor.Default;
            this.FindControl<Panel>("Highlight").Background = new SolidColorBrush(Avalonia.Media.Color.Parse("#0000"));
        }

        private void Clicked(object? sender, PointerPressedEventArgs e)
        {
            OnSelected?.Invoke(Hierarchy);
        }

        private void MyHierarchy_OnOnSelected(InspectionElement selected)
        {
            OnSelected?.Invoke(selected);
            Selection = selected;
        }
    }
}
