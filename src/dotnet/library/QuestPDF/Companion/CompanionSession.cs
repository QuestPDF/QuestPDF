#if NET8_0_OR_GREATER

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using QuestPDF.Drawing;
using QuestPDF.Drawing.DocumentCanvases;
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

        /// <summary>                                                                                                                                                                      
        /// Delivers "regenerate the preview" signals from hot-reload events to the refresh worker.                                                                                        
        /// Since every refresh picks up the newest code, one pending signal is enough:                                                                                                    
        /// the single-item capacity with the DropWrite policy safely merges signal bursts into one refresh.                                                                               
        /// </summary>
        private Channel<bool> RefreshSignals { get; } = Channel.CreateBounded<bool>(
            new BoundedChannelOptions(1) { FullMode = BoundedChannelFullMode.DropWrite });

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
                using var companionService = new CompanionService(Port);
                await companionService.Connect(CancellationTokenSource.Token);

                HotReloadManager.UpdateApplicationRequested += InvalidatePreview;

                try
                {
                    RefreshSignals.Writer.TryWrite(true);

                    var heartbeat = companionService.RunHeartbeatLoop(CancellationTokenSource.Token);
                    var refreshWorker = RunRefreshWorker(companionService, CancellationTokenSource.Token);
                    
                    await Task.WhenAny(heartbeat, refreshWorker);
                    await CancellationTokenSource.CancelAsync();
                    
                    await Task.WhenAll(heartbeat, refreshWorker);
                }
                catch (Exception exception) when (exception is OperationCanceledException or HttpRequestException)
                {
                    // ignored
                }
                finally
                {
                    HotReloadManager.UpdateApplicationRequested -= InvalidatePreview;
                }
            }
            finally
            {
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
                // the preview session has already ended and released its resources
            }
        }

        private void InvalidatePreview(object? sender, EventArgs args)
        {
            CompanionService.IsDocumentHotReloaded = true;
            RefreshSignals.Writer.TryWrite(true);
        }

        private async Task RunRefreshWorker(CompanionService companionService, CancellationToken cancellationToken)
        {
            while (await RefreshSignals.Reader.WaitToReadAsync(cancellationToken))
            {
                RefreshSignals.Reader.TryRead(out _);
                await RefreshPreview(companionService, cancellationToken);
            }
        }

        private async Task RefreshPreview(CompanionService companionService, CancellationToken cancellationToken)
        {
            using var documentSnapshot = await GenerateDocumentSnapshot(companionService, cancellationToken);

            if (documentSnapshot == null)
                return;

            if (!await TryUpdateDocumentStructure(companionService, documentSnapshot, cancellationToken))
                return;

            await ServeRenderRequestsUntilNextRefreshSignal(companionService, documentSnapshot, cancellationToken);
        }

        private async Task<CompanionDocumentSnapshot?> GenerateDocumentSnapshot(CompanionService companionService, CancellationToken cancellationToken)
        {
            try
            {
                return await Task.Run(() => DocumentGenerator.GenerateCompanionContent(Document), cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                // the document cannot be generated, e.g. the user code has thrown;
                // show the problem in the Companion app and wait for the next refresh signal
                await companionService.InformAboutGenericException(exception, cancellationToken);
                return null;
            }
        }

        private async Task<bool> TryUpdateDocumentStructure(CompanionService companionService, CompanionDocumentSnapshot documentSnapshot, CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    await companionService.UpdateDocumentStructure(documentSnapshot, cancellationToken);
                    return true;
                }
                catch (HttpRequestException exception) when (exception.HttpRequestError == HttpRequestError.ConnectionError)
                {
                    // on localhost, a connection that cannot be established means that the Companion app
                    // has been closed; the heartbeat loop ends the session within one tick
                    return false;
                }
                catch (Exception exception) when (!cancellationToken.IsCancellationRequested && exception is HttpRequestException or TaskCanceledException)
                {
                    // the app is alive but temporarily unresponsive, e.g. busy processing the previous update
                    await Task.Delay(TimeSpan.FromMilliseconds(250), cancellationToken);
                }
            }
        }
        
        private async Task ServeRenderRequestsUntilNextRefreshSignal(CompanionService companionService, CompanionDocumentSnapshot documentSnapshot, CancellationToken cancellationToken)
        {
            using var servingCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // observe the signal without consuming it: the refresh worker loop reads it right after this method returns
            var nextRefreshSignal = RefreshSignals.Reader.WaitToReadAsync(servingCancellation.Token).AsTask();
            var renderRequestServing = companionService.ServeRenderRequests(documentSnapshot, servingCancellation.Token);

            await Task.WhenAny(nextRefreshSignal, renderRequestServing);
            await servingCancellation.CancelAsync();
            
            await Task.WhenAll(nextRefreshSignal, renderRequestServing).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
        }

        public void Dispose()
        {
            CancellationTokenSource.Dispose();
        }
    }
}

#endif
