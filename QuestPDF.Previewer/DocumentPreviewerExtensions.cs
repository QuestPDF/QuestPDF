using Avalonia;
using QuestPDF.Infrastructure;

namespace QuestPDF.Previewer
{
    /// <summary>
    /// Extensions for <see cref="IDocument"/> for previewer
    /// </summary>
    public static class DocumentPreviewerExtensions
    {
        /// <summary>
        /// Displays the document in a previewer which supports hot reloading.
        /// </summary>
        /// <remarks>
        /// Intended for development only. Not intended for shipping.
        /// </remarks>
        /// <param name="document"></param>
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
