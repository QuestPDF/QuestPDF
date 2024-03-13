using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using QuestPDF.Previewer;

var applicationPort = GetCommunicationPort();
CommunicationService.Instance.Start(applicationPort);

if (Application.Current?.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime desktop)
{
    desktop.MainWindow = new PreviewerWindow()
    {
        DataContext = new PreviewerWindowViewModel()
    };

    desktop.MainWindow.Show();
    desktop.Start(Array.Empty<string>());

    return;
}

AppBuilder
    .Configure(() => new PreviewerApp())
    .UsePlatformDetect()
    .UseReactiveUI()
    .StartWithClassicDesktopLifetime(Array.Empty<string>());

static int GetCommunicationPort()
{
    const int defaultApplicationPort = 12500;
    var arguments = Environment.GetCommandLineArgs();

    if (arguments.Length < 2)
        return defaultApplicationPort;

    return int.TryParse(arguments[1], out var port) ? port : defaultApplicationPort;
}