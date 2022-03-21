using Avalonia;
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
