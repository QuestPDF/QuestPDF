#if NETCOREAPP3_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using QuestPDF.Drawing;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Infrastructure;

namespace QuestPDF.Previewer
{
    public static class Extensions
    {
        public static void ShowInPreviewer(this IDocument document, int port = 12500)
        {
            document.ShowInPreviewerAsync(port).ConfigureAwait(true).GetAwaiter().GetResult();
        }
        
        public static async Task ShowInPreviewerAsync(this IDocument document, int port = 12500, CancellationToken cancellationToken = default)
        {
            QuestPDF.Settings.EnableDebugging = true;
            
            var previewerService = new PreviewerService(port);
            
            using var cancellationTokenSource = new CancellationTokenSource();
            previewerService.OnLostConnection += () => cancellationTokenSource.Cancel();
            
            await previewerService.Connect();
            await RefreshPreview();
            
            #if NET6_0_OR_GREATER
            HotReloadManager.UpdateApplicationRequested += (_, _) => RefreshPreview();
            #endif

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                // ReSharper disable once MethodSupportsCancellation
                await Task.Delay(TimeSpan.FromMilliseconds(1000));
            }

            Task RefreshPreview()
            {
                try
                {
                    var documentPreviewResult = DocumentGenerator.GeneratePreviewerPictures(document);
                    return previewerService.ShowDocumentPreview(documentPreviewResult);
                }
                catch (DocumentLayoutException exception)
                {
                    return previewerService.ShowLayoutError(exception);
                }
                catch (Exception exception)
                {
                    return previewerService.ShowGenericError(exception);
                }
            }
        }
    }
}

#endif
