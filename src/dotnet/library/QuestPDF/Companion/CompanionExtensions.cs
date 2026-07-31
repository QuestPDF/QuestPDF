using System;
using System.Threading;
using System.Threading.Tasks;
using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Companion
{
    public static class CompanionExtensions
    {
        static CompanionExtensions()
        {
            LicenseChecker.ValidateLicense();
        }

        #if NET6_0_OR_GREATER
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="companion.support"]/*' />
        public static void ShowInCompanion(this IDocument document, int port = 12500)
        {
            document.ShowInCompanionAsync(port).GetAwaiter().GetResult();
        }

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="companion.support"]/*' />
        public static Task ShowInCompanionAsync(this IDocument document, int port = 12500, CancellationToken cancellationToken = default)
        {
            // run the entire session on the thread pool so that no await captures the caller's SynchronizationContext;
            // otherwise the blocking ShowInCompanion entry point could deadlock in UI applications
            return Task.Run(() => ShowInCompanionImplementation(document, port, cancellationToken));
        }

        private static async Task ShowInCompanionImplementation(IDocument document, int port, CancellationToken cancellationToken)
        {
            Settings.EnableCaching = false;
            Settings.EnableDebugging = true;
            
            if (document is MergedDocument)
                throw new NotSupportedException("The QuestPDF Companion App does not currently support merged documents. Please use the tool with a single document at a time.");
            
            using var companionService = new CompanionService(port);
            
            using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            
            companionService.OnCompanionStopped += () =>
            {
                try
                {
                    cancellationTokenSource.Cancel();
                }
                catch (ObjectDisposedException)
                {
                    // the preview session has already ended
                }
            };

            var refreshSemaphore = new SemaphoreSlim(1, 1);
            var isRefreshPending = 0;

            await companionService.Connect(cancellationTokenSource.Token);
            companionService.StartRenderRequestedPageSnapshotsTask(cancellationTokenSource.Token);
            await RefreshPreview();

            HotReloadManager.UpdateApplicationRequested += InvalidatePreview;

            try
            {
                await KeepApplicationAlive(cancellationTokenSource.Token);
            }
            finally
            {
                HotReloadManager.UpdateApplicationRequested -= InvalidatePreview;
            }

            void InvalidatePreview(object? sender, EventArgs args)
            {
                CompanionService.IsDocumentHotReloaded = true;
                _ = RefreshPreviewSafely();
            }

            async Task RefreshPreviewSafely()
            {
                // coalesce hot-reload bursts: at most one refresh runs while one more waits;
                // a refresh that starts later picks up the newest code anyway
                if (Interlocked.Exchange(ref isRefreshPending, 1) == 1)
                    return;

                try
                {
                    await RefreshPreview();
                }
                catch
                {
                    // the Companion app is not reachable; the disconnect detection will end the session
                }
            }

            async Task RefreshPreview()
            {
                await refreshSemaphore.WaitAsync(cancellationTokenSource.Token);

                try
                {
                    Interlocked.Exchange(ref isRefreshPending, 0);

                    var pictures = await Task.Run(() => DocumentGenerator.GenerateCompanionContent(document));
                    await companionService.RefreshPreview(pictures);
                }
                catch (Exception exception)
                {
                    await companionService.InformAboutGenericException(exception);
                }
                finally
                {
                    refreshSemaphore.Release();
                }
            }

            async Task KeepApplicationAlive(CancellationToken sessionCancellationToken)
            {
                try
                {
                    await Task.Delay(Timeout.InfiniteTimeSpan, sessionCancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // the preview session has ended: either the Companion app was closed or the caller cancelled
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
