#if NET8_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using QuestPDF.Drawing.DocumentCanvases;

namespace QuestPDF.Companion
{
    internal sealed class CompanionService : IAsyncDisposable
    {
        private int Port { get; }
        private HttpClient HttpClient { get; }
        
        public event Action? OnCompanionStopped;

        private const int RequiredCompanionApiVersion = 3;
        
        private CancellationTokenSource? RenderingTaskCancellation { get; set; }

        public static bool IsCompanionAttached { get; private set; }
        public static bool IsDocumentHotReloaded { get; set; } = false;

        public CompanionService(int port)
        {
            IsCompanionAttached = true;
            
            Port = port;
            HttpClient = new()
            {
                BaseAddress = new Uri($"http://localhost:{port}/"), 
                Timeout = TimeSpan.FromSeconds(5)
            };
        }

        public async Task Connect(CancellationToken cancellationToken)
        {
            await CheckIfCompanionIsRunning();
            await CheckCompanionVersionCompatibility();
            _ = StartNotifyPresenceTask(cancellationToken);
        }

        private async Task CheckIfCompanionIsRunning()
        {
            try
            {
                using var result = await HttpClient.GetAsync("/ping");
                result.EnsureSuccessStatusCode();
            }
            catch (Exception exception)
            {
                throw new Exception("Cannot connect to the QuestPDF Companion tool. Please ensure that the tool is running and the port is correct. Learn more: https://www.questpdf.com/companion/usage.html", exception);
            }
        }

        internal async Task StartNotifyPresenceTask(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    using var result = await HttpClient.PostAsJsonAsync($"/v{RequiredCompanionApiVersion}/notify", new CompanionCommands.Notify(), CompanionJsonContext.Default.Notify, cancellationToken);
                    result.EnsureSuccessStatusCode();

                    await Task.Delay(TimeSpan.FromMilliseconds(250), cancellationToken);
                }
            }
            catch when (cancellationToken.IsCancellationRequested)
            {
                // the preview session has ended
            }
            catch
            {
                // the Companion app is not reachable
                OnCompanionStopped?.Invoke();
            }
        }

        private async Task CheckCompanionVersionCompatibility()
        {
            using var result = await HttpClient.GetAsync("/version");
            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadFromJsonAsync(CompanionJsonContext.Default.GetVersionCommandResponse);

            if (response != null && response.SupportedVersions.Contains(RequiredCompanionApiVersion))
                return;
            
            throw new Exception($"The QuestPDF Companion application is not compatible. Please install the QuestPDF Companion tool in a proper version.");
        }

        public async Task RefreshPreview(CompanionDocumentSnapshot companionDocumentSnapshot, CancellationToken cancellationToken)
        {
            var documentStructure = new CompanionCommands.UpdateDocumentStructure
            {
                Hierarchy = companionDocumentSnapshot.Hierarchy.ImproveHierarchyStructure(),
                IsDocumentHotReloaded = IsDocumentHotReloaded,
                
                Pages = companionDocumentSnapshot
                    .Pictures
                    .Select(x => new CompanionCommands.UpdateDocumentStructure.PageSize
                    {
                        Width = x.Size.Width,
                        Height = x.Size.Height
                    })
                    .ToArray()
            };

            await StopRenderRequestedPageSnapshotsTask();

                using var result = await HttpClient.PostAsJsonAsync($"/v{RequiredCompanionApiVersion}/documentPreview/update", documentStructure, CompanionJsonContext.Default.UpdateDocumentStructure, cancellationToken);

            result.EnsureSuccessStatusCode();
            
            RenderingTaskCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _ = Task.Run(() => StartRenderRequestedPageSnapshotsTask(companionDocumentSnapshot, RenderingTaskCancellation.Token), CancellationToken.None);
        }
        
        private async Task StartRenderRequestedPageSnapshotsTask(CompanionDocumentSnapshot documentSnapshot, CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await RenderRequestedPageSnapshots(documentSnapshot, cancellationToken);
                }
            }
            catch when (cancellationToken.IsCancellationRequested)
            {
                
            }
            finally
            {
                documentSnapshot.Dispose();
            }
        }

        private async Task StopRenderRequestedPageSnapshotsTask()
        {
            if (RenderingTaskCancellation == null)
                return;
            
#if NET8_0_OR_GREATER
            await RenderingTaskCancellation.CancelAsync();
#else
                RenderingTaskCancellation.Cancel();
#endif
            RenderingTaskCancellation.Dispose();
            
            RenderingTaskCancellation = null;
        }

        private async Task RenderRequestedPageSnapshots(CompanionDocumentSnapshot documentSnapshot, CancellationToken cancellationToken)
        {
            // get requests (companion keeps the http connection for 2 seconds, waiting for new rendering requests)
            using var getRequestedSnapshots = await HttpClient.GetAsync($"/v{RequiredCompanionApiVersion}/documentPreview/getRenderingRequests", cancellationToken);
            getRequestedSnapshots.EnsureSuccessStatusCode();
            
            var requestedSnapshots = await getRequestedSnapshots.Content.ReadFromJsonAsync(CompanionJsonContext.Default.PageSnapshotIndexCollection, cancellationToken);
            
            if (requestedSnapshots == null || !requestedSnapshots.Any())
                return;

            var renderingTasks = requestedSnapshots
                .Select(index => Task.Run(() =>
                {
                    var image = documentSnapshot
                        .Pictures
                        .ElementAt(index.PageIndex)
                        .RenderImage(index.ZoomLevel);

                    return new CompanionCommands.ProvideRenderedDocumentPage.RenderedPage
                    {
                        PageIndex = index.PageIndex,
                        ZoomLevel = index.ZoomLevel,
                        ImageData = Convert.ToBase64String(image)
                    };
                }))
                .ToList();

            var renderedPages = await Task.WhenAll(renderingTasks);

            if (cancellationToken.IsCancellationRequested)
                return;

            var command = new CompanionCommands.ProvideRenderedDocumentPage { Pages = renderedPages };
            
            using var provideRenderedImagesResult = await HttpClient.PostAsJsonAsync($"/v{RequiredCompanionApiVersion}/documentPreview/provideRenderedImages", command, CompanionJsonContext.Default.ProvideRenderedDocumentPage, cancellationToken);

            provideRenderedImagesResult.EnsureSuccessStatusCode();
        }
        
        internal async Task InformAboutGenericException(Exception exception)
        {
            var command = new CompanionCommands.ShowGenericException
            {
                Exception = Map(exception)
            };
            
            using var result = await HttpClient.PostAsJsonAsync($"/v{RequiredCompanionApiVersion}/genericException/show", command, CompanionJsonContext.Default.ShowGenericException);
            result.EnsureSuccessStatusCode();
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

        public async ValueTask DisposeAsync()
        {
            IsCompanionAttached = false;
            HttpClient.Dispose();

            try
            {
                await StopRenderRequestedPageSnapshotsTask();
            }
            catch
            {
                // ignored
            }
        }
    }
}

#endif
