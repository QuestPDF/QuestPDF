using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace QuestPDF.Previewer;

internal class PreviewerApp : Application
{
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
            };
        }
            
        base.OnFrameworkInitializationCompleted();
    }
}