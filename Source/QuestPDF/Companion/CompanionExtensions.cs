using System;
using System.Threading;
using System.Threading.Tasks;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Companion
{
    public static class CompanionExtensions
    {
        static CompanionExtensions()
        {
            DocumentGenerator.ValidateLicense();
        }

        #if NET6_0_OR_GREATER
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="companion.supported"]/*' />
        public static void ShowInCompanion(this IDocument document, int port = 12500)
        {
            document.ShowInCompanionAsync(port).ConfigureAwait(true).GetAwaiter().GetResult();
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="companion.supported"]/*' />
        public static async Task ShowInCompanionAsync(this IDocument document, int port = 12500, CancellationToken cancellationToken = default)
        {
            Settings.EnableCaching = false;
            Settings.EnableDebugging = true;
            
            var companionService = new CompanionService(port);
            
            using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            companionService.OnCompanionStopped += () => cancellationTokenSource.Cancel();

            await companionService.Connect();
            companionService.StartRenderRequestedPageSnapshotsTask(cancellationToken);
            await RefreshPreview();

            HotReloadManager.UpdateApplicationRequested += (_, _) =>
            {
                CompanionService.IsDocumentHotReloaded = true;
                RefreshPreview();
            };
            
            await KeepApplicationAlive(cancellationTokenSource.Token);
            
            Task RefreshPreview()
            {
                try
                {
                    var pictures = DocumentGenerator.GenerateCompanionContent(document);
                    return companionService.RefreshPreview(pictures);
                }
                catch (Exception exception)
                {
                    return companionService.InformAboutGenericException(exception);
                }
            }

            async Task KeepApplicationAlive(CancellationToken cancellationToken)
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

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="companion.notSupported"]/*' />
        public static void ShowInCompanion(this IDocument document, int port = 12500)
        {
            throw new Exception("The hot-reload feature requires .NET 6 or later.");
        }

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="companion.notSupported"]/*' />
        public static async Task ShowInCompanionAsync(this IDocument document, int port = 12500, CancellationToken cancellationToken = default)
        {
            throw new Exception("The hot-reload feature requires .NET 6 or later.");
        }

        #endif
    }
}
