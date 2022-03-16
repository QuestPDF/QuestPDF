using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using QuestPDF.Infrastructure;

namespace QuestPDF.Previewer
{
    internal class PreviewerApp : Application
    {
        private PreviewerView? _preview;

        public IDocument? Document { get; init; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                HotReloadManager.Register(HandleDocumentHotReload);

                _preview = new PreviewerView()
                {
                    Document = Document,
                };
                desktop.MainWindow = new Window()
                {
                    Title = "QuestPDF Document Preview",
                    Content = _preview,
                };
            }
            base.OnFrameworkInitializationCompleted();
        }

        private void HandleDocumentHotReload()
        {
            _preview?.InvalidatePreview();
        }
    }
}
