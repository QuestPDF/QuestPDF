#if NET6_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using QuestPDF.Drawing.DocumentCanvases;

namespace QuestPDF.Companion
{
    internal sealed class CompanionService
    {
        private int Port { get; }
        private HttpClient HttpClient { get; }
        
        public event Action? OnCompanionStopped;

        private const int RequiredCompanionApiVersion = 3;
        
        private static CompanionDocumentSnapshot? CurrentDocumentSnapshot { get; set; }

        public static bool IsCompanionAttached { get; private set; }
        public static bool IsDocumentHotReloaded { get; set; } = false;
        
#if NET8_0_OR_GREATER
        private static JsonSerializerOptions JsonSerializerOptions => CompanionJsonContext.Default.Options;
#else
        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            MaxDepth = 256,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
#endif

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

        public async Task Connect()
        {
            await CheckIfCompanionIsRunning();
            await CheckCompanionVersionCompatibility();
            StartNotifyPresenceTask();
        }

        private async Task CheckIfCompanionIsRunning()
        {
            try
            {
                using var result = await HttpClient.GetAsync("/ping");
                result.EnsureSuccessStatusCode();
            }
            catch
            {
                throw new Exception("Cannot connect to the QuestPDF Companion tool. Please ensure that the tool is running and the port is correct. Learn more: https://www.questpdf.com/companion/usage.html");
            }
        }
        
        internal async Task StartNotifyPresenceTask()
        {
            while (true)
            {
                try
                {
#if NET8_0_OR_GREATER
                    using var result = await HttpClient.PostAsJsonAsync($"/v{RequiredCompanionApiVersion}/notify", new CompanionCommands.Notify(), CompanionJsonContext.Default.Notify);
#else
                    using var result = await HttpClient.PostAsJsonAsync($"/v{RequiredCompanionApiVersion}/notify", new CompanionCommands.Notify(), JsonSerializerOptions);
#endif
                }
                catch
                {
                    
                }
                
                await Task.Delay(TimeSpan.FromMilliseconds(250));
            }
        }
        
        private async Task CheckCompanionVersionCompatibility()
        {
            using var result = await HttpClient.GetAsync("/version");
#if NET8_0_OR_GREATER
            var response = await result.Content.ReadFromJsonAsync(CompanionJsonContext.Default.GetVersionCommandResponse);
#else
            var response = await result.Content.ReadFromJsonAsync<CompanionCommands.GetVersionCommandResponse>();
#endif
            
            if (response.SupportedVersions.Contains(RequiredCompanionApiVersion))
                return;
            
            throw new Exception($"The QuestPDF Companion application is not compatible. Please install the QuestPDF Companion tool in a proper version.");
        }

        public async Task RefreshPreview(CompanionDocumentSnapshot companionDocumentSnapshot)
        {
            // clean old state
            if (CurrentDocumentSnapshot != null)
            {
                foreach (var companionPageSnapshot in CurrentDocumentSnapshot.Pictures)
                    companionPageSnapshot.Picture.Dispose();
            }
            
            // set new state
            CurrentDocumentSnapshot = companionDocumentSnapshot;
            
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

#if NET8_0_OR_GREATER
            await HttpClient.PostAsJsonAsync($"/v{RequiredCompanionApiVersion}/documentPreview/update", documentStructure, CompanionJsonContext.Default.UpdateDocumentStructure);
#else
            await HttpClient.PostAsJsonAsync($"/v{RequiredCompanionApiVersion}/documentPreview/update", documentStructure, JsonSerializerOptions);
#endif
        }
        
        public void StartRenderRequestedPageSnapshotsTask(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await RenderRequestedPageSnapshots();
                    }
                    catch
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(250), cancellationToken);
                    }
                }
            });
        }

        private async Task RenderRequestedPageSnapshots()
        {
            // get requests
            var getRequestedSnapshots = await HttpClient.GetAsync($"/v{RequiredCompanionApiVersion}/documentPreview/getRenderingRequests");
            getRequestedSnapshots.EnsureSuccessStatusCode();
            
#if NET8_0_OR_GREATER
            var requestedSnapshots = await getRequestedSnapshots.Content.ReadFromJsonAsync(CompanionJsonContext.Default.PageSnapshotIndexCollection);
#else
            var requestedSnapshots = await getRequestedSnapshots.Content.ReadFromJsonAsync<ICollection<PageSnapshotIndex>>();
#endif
            
            if (!requestedSnapshots.Any())
                return;
            
            if (CurrentDocumentSnapshot == null)
                return;
      
            // render snapshots
            if (!requestedSnapshots.Any())
                return;

            var renderingTasks = requestedSnapshots
                .Select(index => Task.Run(() =>
                {
                    var image = CurrentDocumentSnapshot
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
            var command = new CompanionCommands.ProvideRenderedDocumentPage { Pages = renderedPages };
#if NET8_0_OR_GREATER
            await HttpClient.PostAsJsonAsync($"/v{RequiredCompanionApiVersion}/documentPreview/provideRenderedImages", command, CompanionJsonContext.Default.ProvideRenderedDocumentPage);
#else
            await HttpClient.PostAsJsonAsync($"/v{RequiredCompanionApiVersion}/documentPreview/provideRenderedImages", command);
#endif
        }
        
        internal async Task InformAboutGenericException(Exception exception)
        {
            var command = new CompanionCommands.ShowGenericException
            {
                Exception = Map(exception)
            };
            
#if NET8_0_OR_GREATER
            await HttpClient.PostAsJsonAsync($"/v{RequiredCompanionApiVersion}/genericException/show", command, CompanionJsonContext.Default.ShowGenericException);
#else
            await HttpClient.PostAsJsonAsync($"/v{RequiredCompanionApiVersion}/genericException/show", command, JsonSerializerOptions);
#endif
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

#endif
