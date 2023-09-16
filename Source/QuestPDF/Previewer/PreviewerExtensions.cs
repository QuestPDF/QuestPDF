

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Previewer
{
    public static class Extensions
    {
        #if NET6_0_OR_GREATER
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="previewer.supported"]/*' />
        public static void ShowInPreviewer(this IDocument document, int port = 12500)
        {
            document.ShowInPreviewerAsync(port).ConfigureAwait(true).GetAwaiter().GetResult();
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="previewer.supported"]/*' />
        public static async Task ShowInPreviewerAsync(this IDocument document, int port = 12500)
        {
            var previewerService = new PreviewerService(port);
            
            using var cancellationTokenSource = new CancellationTokenSource();
            previewerService.OnPreviewerStopped += () => cancellationTokenSource.Cancel();
            
            await previewerService.Connect();
            await RefreshPreview();
            
            HotReloadManager.UpdateApplicationRequested += (_, _) => RefreshPreview();
            
            await WaitForPreviewerExit(cancellationTokenSource.Token);
            
            Task RefreshPreview()
            {
                var pictures = GetPictures();
                return previewerService.RefreshPreview(pictures);
                
                ICollection<PreviewerPicture> GetPictures()
                {
                    try
                    {
                        return DocumentGenerator.GeneratePreviewerPictures(document);
                    }
                    catch (Exception exception)
                    {
                        var exceptionDocument = new ExceptionDocument(exception);
                        return DocumentGenerator.GeneratePreviewerPictures(exceptionDocument);
                    }
                }
            }

            async Task WaitForPreviewerExit(CancellationToken cancellationToken)
            {
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;
                
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }
            }
        }
        
        #else

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="previewer.notSupported"]/*' />
        public static void ShowInPreviewer(this IDocument document, int port = 12500)
        {
            throw new Exception("The hot-reload feature requires .NET 6 or later.");
        }

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="previewer.notSupported"]/*' />
        public static async Task ShowInPreviewerAsync(this IDocument document, int port = 12500)
        {
            throw new Exception("The hot-reload feature requires .NET 6 or later.");
        }

        #endif
    }
}
