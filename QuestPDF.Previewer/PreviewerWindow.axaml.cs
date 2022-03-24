using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using QuestPDF.Fluent;
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

        public static readonly StyledProperty<float> CurrentScrollProperty = AvaloniaProperty.Register<PreviewerWindow, float>(nameof(CurrentScroll));

        public float CurrentScroll
        {
            get => GetValue(CurrentScrollProperty);
            set => SetValue(CurrentScrollProperty, value);
        }

        public static readonly StyledProperty<float> ScrollViewportSizeProperty = AvaloniaProperty.Register<PreviewerWindow, float>(nameof(ScrollViewportSize));

        public float ScrollViewportSize
        {
            get => GetValue(ScrollViewportSizeProperty);
            set => SetValue(ScrollViewportSizeProperty, value);
        }

        public static readonly StyledProperty<bool> VerticalScrollbarVisibleProperty = AvaloniaProperty.Register<PreviewerWindow, bool>(nameof(VerticalScrollbarVisible));

        public bool VerticalScrollbarVisible
        {
            get => GetValue(VerticalScrollbarVisibleProperty);
            set => SetValue(VerticalScrollbarVisibleProperty, value);
        }

        public PreviewerWindow()
        {
            InitializeComponent();

            ScrollViewportSizeProperty.Changed
                .Subscribe(e => Dispatcher.UIThread.Post(() =>
                {
                    VerticalScrollbarVisible = e.NewValue.Value < 1;
                }));
  
            DocumentProperty.Changed.Subscribe(v => Task.Run(() => DocumentRenderer.UpdateDocument(v.NewValue.Value)));
            HotReloadManager.UpdateApplicationRequested += InvalidatePreview;
        }

        protected override void OnClosed(EventArgs e)
        {
            HotReloadManager.UpdateApplicationRequested -= InvalidatePreview;
            base.OnClosed(e);
        }

        private void InvalidatePreview(object? sender, EventArgs e) => InvalidatePreview();
        private void InvalidatePreview()
        {
            Dispatcher.UIThread.Post(() =>
            {
                var document = Document;
                _ = Task.Run(() => DocumentRenderer.UpdateDocument(document));
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void ShowPDF(object? sender, RoutedEventArgs e)
        {
            var path = Path.GetTempPath() + ".pdf";
            
            try
            {
                DocumentRenderer.Document?.GeneratePdf(path);
            }
            catch (Exception exception)
            {
                new ExceptionDocument(exception).GeneratePdf(path);
            }
            
            var openBrowserProcess = new Process
            {
                StartInfo = new()
                {
                    UseShellExecute = true,
                    FileName = path
                }
            };

            openBrowserProcess.Start();
        }
    }
}
