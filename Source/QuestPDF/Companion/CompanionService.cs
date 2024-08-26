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

        private const int RequiredCompanionVersionMajor = 2024;
        private const int RequiredCompanionVersionMinor = 7;
        
        private static CompanionDocumentSnapshot? CurrentDocumentSnapshot { get; set; }

        public static bool IsCompanionAttached { get; private set; } = true;
        internal bool IsDocumentHotReloaded { get; set; } = false;
        
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
            var isAvailable = await IsCompanionAvailable();

            if (!isAvailable)
            {
                StartCompanion();
                await WaitForConnection();
            }

            StartNotifyPresenceTask();
            
            var companionVersion = await GetCompanionVersion();
            CheckVersionCompatibility(companionVersion);
        }

        private async Task<bool> IsCompanionAvailable()
        {
            try
            {
                using var result = await HttpClient.GetAsync("/ping");
                return result.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
        
        internal async Task StartNotifyPresenceTask()
        {
            while (true)
            {
                try
                {
                    using var result = await HttpClient.PostAsJsonAsync("/notify", new CompanionCommands.Notify(), JsonSerializerOptions);
                }
                catch
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(250));
                }
            }
        }
        
        private async Task<Version> GetCompanionVersion()
        {
            using var result = await HttpClient.GetAsync("/version");
            return await result.Content.ReadFromJsonAsync<Version>();
        }
        
        private void StartCompanion()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new()
                    {
                        FileName = "questpdf-companion",
                        Arguments = $"{Port}",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                
                process.Start();

                Task.Run(async () =>
                {
                    await process.WaitForExitAsync();
                    process.Dispose();
                    OnCompanionStopped?.Invoke();
                });
            }
            catch
            {
                throw new Exception("Cannot start the QuestPDF Companion tool. " +
                                    "Please install it by executing in terminal the following command: 'dotnet tool install --global QuestPDF.Companion'.");
            }
        }

        private static void CheckVersionCompatibility(Version version)
        {
            if (version.Major == RequiredCompanionVersionMajor && version.Minor == RequiredCompanionVersionMinor)
                return;

            const string newLine = "\n";
            const string newParagraph = newLine + newLine;
            
            throw new Exception($"The QuestPDF Companion application is not compatible. Possible solutions: {newParagraph}" +
                                $"1) Change the QuestPDF library to the {version.Major}.{version.Minor}.X version to match the Companion application version. {newParagraph}" +
                                $"2) Recommended: install the QuestPDF Companion tool in a proper version using the following commands: {newParagraph}"+
                                $"dotnet tool uninstall --global QuestPDF.Companion {newLine}"+
                                $"dotnet tool update --global QuestPDF.Companion --version {RequiredCompanionVersionMajor}.{RequiredCompanionVersionMinor} {newParagraph}");
        }
        
        private async Task WaitForConnection()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(10));
            
            var cancellationToken = cancellationTokenSource.Token; 
            
            while (true)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(250));

                if (cancellationToken.IsCancellationRequested)
                {
                    throw new Exception($"Cannot connect to the QuestPDF Companion tool. Please ensure that: " +
                                        $"1) the dotnet 8 runtime is installed on your environment, " +
                                        $"2) your operating system does not block HTTP connections on port {Port}.");
                }

                var isConnected = await IsCompanionAvailable();

                if (isConnected)
                    break;
            }
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

            await HttpClient.PostAsJsonAsync("/documentPreview/update", documentStructure, JsonSerializerOptions);
        }
        
        public void StartRenderRequestedPageSnapshotsTask(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        RenderRequestedPageSnapshots();
                    }
                    catch
                    {
                        
                    }
                    
                    await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
                }
            });
        }

        private async Task RenderRequestedPageSnapshots()
        {
            // get requests
            var getRequestedSnapshots = await HttpClient.GetAsync("/documentPreview/getRenderingRequests");
            getRequestedSnapshots.EnsureSuccessStatusCode();
            
            var requestedSnapshots = await getRequestedSnapshots.Content.ReadFromJsonAsync<ICollection<PageSnapshotIndex>>();
            
            if (!requestedSnapshots.Any())
                return;
            
            if (CurrentDocumentSnapshot == null)
                return;
      
            // render snapshots
            var renderingTasks = requestedSnapshots
                .Select(async index =>
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
                })
                .ToList();

            if (!renderingTasks.Any())
                return;

            var renderedPages = await Task.WhenAll(renderingTasks);
            var command = new CompanionCommands.ProvideRenderedDocumentPage { Pages = renderedPages };
            await HttpClient.PostAsJsonAsync("/documentPreview/provideRenderedImages", command);
        }
        
        internal async Task InformAboutGenericException(Exception exception)
        {
            var command = new CompanionCommands.ShowGenericException
            {
                Exception = Map(exception)
            };
            
            await HttpClient.PostAsJsonAsync("/genericException/show", command, JsonSerializerOptions);
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
