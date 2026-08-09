#if NET8_0_OR_GREATER

using System;
using System.Threading;
using System.Threading.Tasks;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Companion
{
    internal sealed class CompanionSession : IDisposable
    {
        private static CompanionSession? CurrentSession;

        private IDocument Document { get; }
        private int Port { get; }

        private CancellationTokenSource CancellationTokenSource { get; }
        private TaskCompletionSource SessionCompletionSource { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        private CompanionService? CompanionService { get; set; }
        private SemaphoreSlim RefreshSemaphore { get; } = new(1, 1);
        private int IsRefreshPending;

        private Task Completion => SessionCompletionSource.Task;

        private CompanionSession(IDocument document, int port, CancellationToken cancellationToken)
        {
            Document = document;
            Port = port;
            CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        }

        public static async Task RunNewCompanionSession(IDocument document, int port, CancellationToken cancellationToken)
        {
            Settings.EnableCaching = false;
            Settings.EnableDebugging = true;

            if (document is MergedDocument)
                throw new NotSupportedException("The QuestPDF Companion App does not currently support merged documents. Please use the tool with a single document at a time.");

            // only one preview session can communicate with the Companion app at a time;
            // showing another document stops the previous session and waits until it fully releases its resources
            using var currentSession = new CompanionSession(document, port, cancellationToken);
            var previousSession = Interlocked.Exchange(ref CurrentSession, currentSession);

            if (previousSession != null)
            {
                previousSession.Stop();
                await previousSession.Completion;
            }

            try
            {
                await currentSession.Run();
            }
            finally
            {
                Interlocked.CompareExchange(ref CurrentSession, null, currentSession);
            }
        }

        private async Task Run()
        {
            try
            {
                CompanionService = new CompanionService(Port);
                CompanionService.OnCompanionStopped += Stop;

                await CompanionService.Connect(CancellationTokenSource.Token);

                HotReloadManager.UpdateApplicationRequested += InvalidatePreview;

                try
                {
                    await RefreshPreview();
                    await WaitUntilStopped();
                }
                finally
                {
                    HotReloadManager.UpdateApplicationRequested -= InvalidatePreview;
                }
            }
            finally
            {
                Stop();

                if (CompanionService != null)
                    await CompanionService.DisposeAsync();

                SessionCompletionSource.TrySetResult();
            }
        }

        private void Stop()
        {
            try
            {
                CancellationTokenSource.Cancel();
            }
            catch (ObjectDisposedException)
            {
                // the preview session has already ended
            }
        }

        private void InvalidatePreview(object? sender, EventArgs args)
        {
            CompanionService.IsDocumentHotReloaded = true;
            _ = RefreshPreviewSafely();
        }

        private async Task RefreshPreviewSafely()
        {
            // coalesce hot-reload bursts: at most one refresh runs while one more waits;
            // a refresh that starts later picks up the newest code anyway
            if (Interlocked.Exchange(ref IsRefreshPending, 1) == 1)
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

        private async Task RefreshPreview()
        {
            await RefreshSemaphore.WaitAsync(CancellationTokenSource.Token);

            try
            {
                Interlocked.Exchange(ref IsRefreshPending, 0);

                var documentSnapshot = await Task.Run(() => DocumentGenerator.GenerateCompanionContent(Document));
                await CompanionService!.RefreshPreview(documentSnapshot, CancellationTokenSource.Token);
            }
            catch (OperationCanceledException) when (CancellationTokenSource.IsCancellationRequested)
            {
                // the preview session is ending; there is nothing to refresh anymore
            }
            catch (Exception exception)
            {
                await CompanionService!.InformAboutGenericException(exception);
            }
            finally
            {
                RefreshSemaphore.Release();
            }
        }

        private async Task WaitUntilStopped()
        {
            try
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, CancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // the preview session has ended: the Companion app was closed, the caller cancelled,
                // or another preview session was started
            }
        }

        public void Dispose()
        {
            CancellationTokenSource.Dispose();
            RefreshSemaphore.Dispose();
        }
    }
}

#endif
