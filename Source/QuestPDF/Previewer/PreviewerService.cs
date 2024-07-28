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

namespace QuestPDF.Previewer
{
    internal class PreviewerService
    {
        private int Port { get; }
        private HttpClient HttpClient { get; }
        
        public event Action? OnPreviewerStopped;

        private const int RequiredPreviewerVersionMajor = 2024;
        private const int RequiredPreviewerVersionMinor = 7;
        
        private static PreviewerDocumentSnapshot? CurrentDocumentSnapshot { get; set; }
        
        JsonSerializerOptions JsonSerializerOptions = new()
        {
            MaxDepth = 256,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
        
        public PreviewerService(int port)
        {
            Port = port;
            HttpClient = new()
            {
                BaseAddress = new Uri($"http://localhost:{port}/"), 
                Timeout = TimeSpan.FromSeconds(1)
            };
        }

        public async Task Connect()
        {
            var isAvailable = await IsPreviewerAvailable();

            if (!isAvailable)
            {
                StartPreviewer();
                await WaitForConnection();
            }

            StartNotifyPresenceTask();
            
            var previewerVersion = await GetPreviewerVersion();
            CheckVersionCompatibility(previewerVersion);
        }

        private async Task<bool> IsPreviewerAvailable()
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
                    using var result = await HttpClient.PostAsJsonAsync("/notify", new PreviewerCommands.Notify(), JsonSerializerOptions);
                }
                catch
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(250));
                }
            }
        }
        
        private async Task<Version> GetPreviewerVersion()
        {
            using var result = await HttpClient.GetAsync("/version");
            return await result.Content.ReadFromJsonAsync<Version>();
        }
        
        private void StartPreviewer()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new()
                    {
                        FileName = "questpdf-previewer",
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
                    OnPreviewerStopped?.Invoke();
                });
            }
            catch
            {
                throw new Exception("Cannot start the QuestPDF Previewer tool. " +
                                    "Please install it by executing in terminal the following command: 'dotnet tool install --global QuestPDF.Previewer'.");
            }
        }

        private static void CheckVersionCompatibility(Version version)
        {
            if (version.Major == RequiredPreviewerVersionMajor && version.Minor == RequiredPreviewerVersionMinor)
                return;

            const string newLine = "\n";
            const string newParagraph = newLine + newLine;
            
            throw new Exception($"The QuestPDF Previewer application is not compatible. Possible solutions: {newParagraph}" +
                                $"1) Change the QuestPDF library to the {version.Major}.{version.Minor}.X version to match the Previewer application version. {newParagraph}" +
                                $"2) Recommended: install the QuestPDF Previewer tool in a proper version using the following commands: {newParagraph}"+
                                $"dotnet tool uninstall --global QuestPDF.Previewer {newLine}"+
                                $"dotnet tool update --global QuestPDF.Previewer --version {RequiredPreviewerVersionMajor}.{RequiredPreviewerVersionMinor} {newParagraph}");
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
                    throw new Exception($"Cannot connect to the QuestPDF Previewer tool. Please ensure that: " +
                                        $"1) the dotnet 8 runtime is installed on your environment, " +
                                        $"2) your operating system does not block HTTP connections on port {Port}.");
                }

                var isConnected = await IsPreviewerAvailable();

                if (isConnected)
                    break;
            }
        }
        
        public async Task RefreshPreview(PreviewerDocumentSnapshot previewerDocumentSnapshot)
        {
            // clean old state
            if (CurrentDocumentSnapshot != null)
            {
                foreach (var previewerPageSnapshot in CurrentDocumentSnapshot.Pictures)
                    previewerPageSnapshot.Picture.Dispose();
            }
            
            // set new state
            CurrentDocumentSnapshot = previewerDocumentSnapshot;
            
            var documentStructure = new PreviewerCommands.UpdateDocumentStructure
            {
                Hierarchy = previewerDocumentSnapshot.Hierarchy,
                DocumentContentHasLayoutOverflowIssues = previewerDocumentSnapshot.DocumentContentHasLayoutOverflowIssues,
                
                Pages = previewerDocumentSnapshot
                    .Pictures
                    .Select(x => new PreviewerCommands.UpdateDocumentStructure.PageSize
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

                    return new PreviewerCommands.ProvideRenderedDocumentPage.RenderedPage
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
            var command = new PreviewerCommands.ProvideRenderedDocumentPage { Pages = renderedPages };
            await HttpClient.PostAsJsonAsync("/documentPreview/provideRenderedImages", command);
        }
        
        internal async Task InformAboutGenericException(Exception exception)
        {
            var command = new PreviewerCommands.ShowGenericException
            {
                Exception = Map(exception)
            };
            
            await HttpClient.PostAsJsonAsync("/genericException/show", command, JsonSerializerOptions);
            return;

            static PreviewerCommands.ShowGenericException.GenericExceptionDetails Map(Exception exception)
            {
                return new PreviewerCommands.ShowGenericException.GenericExceptionDetails
                {
                    Type = exception.GetType().FullName ?? "Unknown", 
                    Message = exception.Message, 
                    StackTrace = exception.StackTrace.ParseStackTrace(),
                    InnerException = exception.InnerException == null ? null : Map(exception.InnerException)
                };
            }
        }
        
        internal async Task InformAboutLayoutError(PreviewerCommands.ShowLayoutError command)
        {
            await HttpClient.PostAsJsonAsync("/layoutError/show", command, JsonSerializerOptions);
        }
    }
}

#endif
