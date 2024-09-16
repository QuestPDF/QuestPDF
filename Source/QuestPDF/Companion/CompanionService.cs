#if NET6_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using QuestPDF.Drawing;

namespace QuestPDF.Companion
{
    internal class CompanionService
    {
        private int Port { get; }
        private HttpClient HttpClient { get; }
        
        public event Action? OnCompanionStopped;

        private const int RequiredCompanionApiVersion = 2;
        
        private static CompanionDocumentSnapshot? CurrentDocumentSnapshot { get; set; }

        public static bool IsCompanionAttached { get; private set; }
        public static bool IsDocumentHotReloaded { get; set; } = false;
        
        JsonSerializerOptions JsonSerializerOptions = new()
        {
            MaxDepth = 256,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
        
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
                    using var result = await HttpClient.PostAsJsonAsync($"/v{RequiredCompanionApiVersion}/notify", new CompanionCommands.Notify(), JsonSerializerOptions);
                }
                catch
                {
                    
                }
                
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }
        }
        
        private async Task CheckCompanionVersionCompatibility()
        {
            using var result = await HttpClient.GetAsync("/version");
            var response = await result.Content.ReadFromJsonAsync<CompanionCommands.GetVersionCommandResponse>();
            
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

            await HttpClient.PostAsJsonAsync($"/v{RequiredCompanionApiVersion}/documentPreview/update", documentStructure, JsonSerializerOptions);
        }
        
        public void StartRenderRequestedPageSnapshotsTask(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        await RenderRequestedPageSnapshots();
                    }
                    catch
                    {
                        
                    }
                    
                    await Task.Delay(TimeSpan.FromMilliseconds(25), cancellationToken);
                }
            });
        }

        private async Task RenderRequestedPageSnapshots()
        {
            // get requests
            var getRequestedSnapshots = await HttpClient.GetAsync($"/v{RequiredCompanionApiVersion}/documentPreview/getRenderingRequests");
            getRequestedSnapshots.EnsureSuccessStatusCode();
            
            var requestedSnapshots = await getRequestedSnapshots.Content.ReadFromJsonAsync<ICollection<PageSnapshotIndex>>();
            
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
            await HttpClient.PostAsJsonAsync($"/v{RequiredCompanionApiVersion}/documentPreview/provideRenderedImages", command);
        }
        
        internal async Task InformAboutGenericException(Exception exception)
        {
            var command = new CompanionCommands.ShowGenericException
            {
                Exception = Map(exception)
            };
            
            await HttpClient.PostAsJsonAsync($"/v{RequiredCompanionApiVersion}/genericException/show", command, JsonSerializerOptions);
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
