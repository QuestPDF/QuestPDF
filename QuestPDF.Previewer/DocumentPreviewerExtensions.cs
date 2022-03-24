using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using QuestPDF.Infrastructure;

namespace QuestPDF.Previewer
{
    public static class DocumentPreviewerExtensions
    {
        /// <summary>
        /// Opens document in the QuestPDF previewer tool.
        /// Improves development speed by supporting hot reloading.
        /// Shows document preview and refreshes it after each code change.
        /// </summary>
        /// <remarks>
        /// Intended for development only. Do not use in production environment.
        /// </remarks>
        public static void ShowInPreviewer(this IDocument document)
        {
            ArgumentNullException.ThrowIfNull(document);

            // currently there is no way to utilize a previously run Avalonia app.
            // so we need to check if the previewer was already run and show the window again.
            if(Application.Current?.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new PreviewerWindow()
                {
                    Document = document,
                };
                
                desktop.MainWindow.Show();
                desktop.Start(Array.Empty<string>());
                
                return;
            }

            AppBuilder
                .Configure(() => new PreviewerApp()
                {
                    Document = document,
                })
                .UsePlatformDetect()
                .StartWithClassicDesktopLifetime(Array.Empty<string>());
        }
    }
}
