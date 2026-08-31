#if NET8_0_OR_GREATER

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using QuestPDF.Drawing.DocumentCanvases;

namespace QuestPDF.Companion
{
    internal sealed class CompanionService : IDisposable
    {
        private const int RequiredCompanionApiVersion = 3;

        private HttpClient HttpClient { get; }
        
        private CompanionDocumentSnapshot? CurrentSnapshot { get; set; }
        private SemaphoreSlim CurrentSnapshotLock { get; } = new(1, 1);

        public static bool IsCompanionAttached { get; private set; }
        public static bool IsDocumentHotReloaded { get; set; }

        public CompanionService(int port)
        {
            IsCompanionAttached = true;

            HttpClient = new()
            {
                BaseAddress = new Uri($"http://localhost:{port}/"),
                Timeout = TimeSpan.FromSeconds(10)
            };
        }

        public void Dispose()
        {
            IsCompanionAttached = false;
            HttpClient.Dispose();
            CurrentSnapshot?.Dispose();
            CurrentSnapshotLock.Dispose();
        }

        public async Task Connect(CancellationToken cancellationToken)
        {
            await CheckIfCompanionIsRunning(cancellationToken);
            await CheckCompanionVersionCompatibility(cancellationToken);
        }

        private async Task CheckIfCompanionIsRunning(CancellationToken cancellationToken)
        {
            try
            {
                using var result = await HttpClient.GetAsync("/ping", cancellationToken);
                result.EnsureSuccessStatusCode();
            }
            catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
            {
                throw new Exception("Cannot connect to the QuestPDF Companion tool. Please ensure that the tool is running and the port is correct. Learn more: https://www.questpdf.com/companion/usage.html", exception);
            }
        }

        private async Task CheckCompanionVersionCompatibility(CancellationToken cancellationToken)
        {
            using var result = await HttpClient.GetAsync("/version", cancellationToken);
            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadFromJsonAsync(CompanionJsonContext.Default.GetVersionCommandResponse, cancellationToken);

            if (response != null && response.SupportedVersions.Contains(RequiredCompanionApiVersion))
                return;

            throw new Exception("The QuestPDF Companion application is not compatible. Please install the QuestPDF Companion tool in a proper version.");
        }
        
        public async Task RunHeartbeatLoop(CancellationToken cancellationToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(250));

            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                var isCompanionRunning = await SendHeartbeat(cancellationToken);

                if (!isCompanionRunning)
                    return;
            }
        }

        /// <summary>
        /// Sends a single heartbeat. Returns false only when the Companion app has been closed.
        /// </summary>
        private async Task<bool> SendHeartbeat(CancellationToken cancellationToken)
        {
            try
            {
                using var result = await HttpClient.PostAsJsonAsync($"/v{RequiredCompanionApiVersion}/notify", new CompanionCommands.Notify(), CompanionJsonContext.Default.Notify, cancellationToken);
                return true;
            }
            catch (HttpRequestException exception) when (exception.HttpRequestError == HttpRequestError.ConnectionError)
            {
                // on localhost, a connection that cannot be established means that the Companion app has been closed
                return false;
            }
            catch when (!cancellationToken.IsCancellationRequested)
            {
                // the app is alive but temporarily unresponsive
                return true;
            }
        }
        
        public async Task UpdateDocumentPreview(CompanionDocumentSnapshot documentSnapshot, CancellationToken cancellationToken)
        {
            try
            {
                await CurrentSnapshotLock.WaitAsync(cancellationToken);
            }
            catch
            {
                documentSnapshot.Dispose();
                throw;
            }

            try
            {
                var previousDocument = CurrentSnapshot;
                CurrentSnapshot = documentSnapshot;

                // safe under the lock: no render can be drawing the pictures of the previous snapshot
                previousDocument?.Dispose();

                while (true)
                {
                    try
                    {
                        await UpdateDocumentStructure(documentSnapshot, cancellationToken);
                        return;
                    }
                    catch (HttpRequestException exception) when (exception.HttpRequestError == HttpRequestError.ConnectionError)
                    {
                        // app has been closed
                        return;
                    }
                    catch (Exception exception) when (!cancellationToken.IsCancellationRequested && exception is HttpRequestException or TaskCanceledException)
                    {
                        // the app is alive but temporarily unresponsive, e.g. busy processing the previous update
                        await Task.Delay(TimeSpan.FromMilliseconds(250), cancellationToken);
                    }
                }
            }
            finally
            {
                CurrentSnapshotLock.Release();
            }
        }

        private async Task UpdateDocumentStructure(CompanionDocumentSnapshot documentSnapshot, CancellationToken cancellationToken)
        {
            var command = new CompanionCommands.UpdateDocumentStructure
            {
                Hierarchy = documentSnapshot.Hierarchy,
                IsDocumentHotReloaded = IsDocumentHotReloaded,

                Pages = documentSnapshot
                    .Pictures
                    .Select(x => new CompanionCommands.UpdateDocumentStructure.PageSize
                    {
                        Width = x.Size.Width,
                        Height = x.Size.Height
                    })
                    .ToArray()
            };

            using var result = await HttpClient.PostAsJsonAsync($"/v{RequiredCompanionApiVersion}/documentPreview/update", command, CompanionJsonContext.Default.UpdateDocumentStructure, cancellationToken);
            result.EnsureSuccessStatusCode();
        }
        
        public async Task ServeRenderRequests(CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    await ServeRenderRequestsRound(cancellationToken);
                }
                catch (HttpRequestException exception) when (exception.HttpRequestError == HttpRequestError.ConnectionError)
                {
                    // on localhost, a connection that cannot be established means that the Companion app has been closed
                    return;
                }
                catch when (!cancellationToken.IsCancellationRequested)
                {
                    // the app is alive but the round failed
                    await Task.Delay(TimeSpan.FromMilliseconds(250), cancellationToken);
                }
            }
        }

        private async Task ServeRenderRequestsRound(CancellationToken cancellationToken)
        {
            // the Companion app keeps this connection open for a couple of seconds, waiting for new rendering requests
            using var renderingRequestsResponse = await HttpClient.GetAsync($"/v{RequiredCompanionApiVersion}/documentPreview/getRenderingRequests", cancellationToken);
            renderingRequestsResponse.EnsureSuccessStatusCode();

            var renderingRequests = await renderingRequestsResponse.Content.ReadFromJsonAsync(CompanionJsonContext.Default.PageSnapshotIndexCollection, cancellationToken);

            if (renderingRequests == null || renderingRequests.Count == 0)
                return;

            await CurrentSnapshotLock.WaitAsync(cancellationToken);

            try
            {
                var documentSnapshot = CurrentSnapshot;
                
                if (documentSnapshot == null)
                    return;

                var renderedPages = renderingRequests
                    .Where(x => x.PageIndex >= 0 && x.PageIndex < documentSnapshot.Pictures.Count)
                    .AsParallel()
                    .AsOrdered()
                    .WithCancellation(cancellationToken)
                    .Select(RenderPage)
                    .ToArray();

                if (renderedPages.Length == 0)
                    return;

                var command = new CompanionCommands.ProvideRenderedDocumentPage { Pages = renderedPages };

                using var provideRenderedImagesResponse = await HttpClient.PostAsJsonAsync($"/v{RequiredCompanionApiVersion}/documentPreview/provideRenderedImages", command, CompanionJsonContext.Default.ProvideRenderedDocumentPage, cancellationToken);
                provideRenderedImagesResponse.EnsureSuccessStatusCode();

                CompanionCommands.ProvideRenderedDocumentPage.RenderedPage RenderPage(PageSnapshotIndex request)
                {
                    var image = documentSnapshot.Pictures[request.PageIndex].RenderImage(request.ZoomLevel);

                    return new CompanionCommands.ProvideRenderedDocumentPage.RenderedPage
                    {
                        PageIndex = request.PageIndex,
                        ZoomLevel = request.ZoomLevel,
                        ImageData = image
                    };
                }
            }
            finally
            {
                CurrentSnapshotLock.Release();
            }
        }

        public async Task InformAboutGenericException(Exception exception, CancellationToken cancellationToken)
        {
            try
            {
                var command = new CompanionCommands.ShowGenericException
                {
                    Exception = Map(exception)
                };

                using var result = await HttpClient.PostAsJsonAsync($"/v{RequiredCompanionApiVersion}/genericException/show", command, CompanionJsonContext.Default.ShowGenericException, cancellationToken);
                result.EnsureSuccessStatusCode();
            }
            catch
            {
                // ignored
            }

            return;

            static CompanionCommands.ShowGenericException.GenericExceptionDetails Map(Exception exception)
            {
                return new CompanionCommands.ShowGenericException.GenericExceptionDetails
                {
                    Type = exception.GetType().FullName ?? "Unknown", 
                    Message = exception.Message, 
                    StackTrace = exception.StackTrace.ParseStackTrace(),
                    InnerException = exception.InnerException == null ? null : Map(exception.InnerException)
                };
            }
        }
    }
}

#else

namespace QuestPDF.Companion
{
    internal static class CompanionService
    {
        public static bool IsCompanionAttached => false;
        public static bool IsDocumentHotReloaded => false;
    }
}
#endif
