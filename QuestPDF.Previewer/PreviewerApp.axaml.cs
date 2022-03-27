using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using QuestPDF.Infrastructure;

namespace QuestPDF.Previewer
{
    internal class PreviewerApp : Application
    {
        public IDocument? Document { get; init; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new PreviewerWindow()
                {
                    DataContext = new PreviewerWindowViewModel()
                    {
                        Document = Document,
                    }
                };
            }
            
            base.OnFrameworkInitializationCompleted();
        }
    }
}
