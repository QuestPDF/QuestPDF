using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ReactiveUI;

namespace QuestPDF.Previewer
{
    class PreviewerWindow : Window
    {
        public DocumentRenderer DocumentRenderer { get; } = new();

        public static readonly StyledProperty<IDocument?> DocumentProperty =
            AvaloniaProperty.Register<PreviewerWindow, IDocument?>(nameof(Document));

        public IDocument? Document
        {
            get => GetValue(DocumentProperty);
            set => SetValue(DocumentProperty, value);
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
        
        public static readonly StyledProperty<bool> VerticalScrollbarVisibleProperty = AvaloniaProperty.Register<PreviewerControl, bool>(nameof(VerticalScrollbarVisible));
        
        public bool VerticalScrollbarVisible
        {
            get => GetValue(VerticalScrollbarVisibleProperty);
            set => SetValue(VerticalScrollbarVisibleProperty, value);
        }
        
        
        public PreviewerWindow()
        {
            InitializeComponent();

            ScrollViewportSizeProperty.Changed.Subscribe(_ =>
            {
                VerticalScrollbarVisible = ScrollViewportSize < 1;
                Debug.WriteLine($"Scrollbar visible: {VerticalScrollbarVisible}");
            });
            
            // this.FindControl<Button>("GeneratePdf")
            //     .Click += (_, _) => _ = PreviewerUtils.SavePdfWithDialog(Document, this);

            DocumentProperty.Changed.Subscribe(v => Task.Run(() => DocumentRenderer.UpdateDocument(v.NewValue.Value)));
            HotReloadManager.UpdateApplicationRequested += (_, _) => InvalidatePreview();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void InvalidatePreview()
        {
            Dispatcher.UIThread.Post(() =>
            {
                var document = Document;
                _ = Task.Run(() => DocumentRenderer.UpdateDocument(document));
            });
        }
    }
}
