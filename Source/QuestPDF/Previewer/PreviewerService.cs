#if NET6_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using QuestPDF.Drawing;

namespace QuestPDF.Previewer
{
    internal class PreviewerService
    {
        private int Port { get; }
        private SocketClient SocketClient { get; }
        
        public event Action? OnPreviewerStopped;

        private const int RequiredPreviewerVersionMajor = 2024;
        private const int RequiredPreviewerVersionMinor = 3;
        
        private static PreviewerDocumentSnapshot? CurrentDocumentSnapshot { get; set; }
        
        public PreviewerService(int port)
        {
            SocketClient = new SocketClient("127.0.0.1", port);
            
            
            
            SocketClient.StartCommunication();
        }

        public async Task Connect()
        {
            var isAvailable = await IsPreviewerAvailable();

            if (!isAvailable)
            {
                StartPreviewer();
                await WaitForConnection();
            }
            
            var previewerVersion = await GetPreviewerVersion();
            CheckVersionCompatibility(previewerVersion);
        }

        private async Task<bool> IsPreviewerAvailable()
        {
            var isPreviewerAvailable = false;

            SocketClient.OnMessageReceived += async message =>
            {
                var content = JsonDocument.Parse(message).RootElement;
                var channel = content.GetProperty("Channel").GetString();

                if (channel == "ping/ok")
                    isPreviewerAvailable = true;
            };
            
            var request = new SocketMessage<PageSnapshotCommunicationData>
            {
                Channel = "ping/check"
            };
            
            SocketClient.SendMessage(JsonSerializer.Serialize(request));
            
            while (!isPreviewerAvailable)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }

            return true;
        }
        
        private async Task<Version> GetPreviewerVersion()
        {
            Version previewerVersion = default;
            
            SocketClient.OnMessageReceived += async message =>
            {
                var content = JsonDocument.Parse(message).RootElement;
                var channel = content.GetProperty("Channel").GetString();

                if (channel == "version/provide")
                    previewerVersion = content.GetProperty("Payload").Deserialize<Version>();
            };
            
            var request = new SocketMessage<PageSnapshotCommunicationData>
            {
                Channel = "version/check"
            };
            
            SocketClient.SendMessage(JsonSerializer.Serialize(request));

            while (previewerVersion == default)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }

            return previewerVersion;
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
            
            var documentStructure = new DocumentStructure
            {
                DocumentContentHasLayoutOverflowIssues = previewerDocumentSnapshot.DocumentContentHasLayoutOverflowIssues,
                
                Pages = previewerDocumentSnapshot
                    .Pictures
                    .Select(x => new DocumentStructure.PageSize()
                    {
                        Width = x.Size.Width,
                        Height = x.Size.Height
                    })
                    .ToArray()
            };
            
            var response = new SocketMessage<DocumentStructure>
            {
                Channel = "preview/update",
                Payload = documentStructure
            };
                
            SocketClient.SendMessage(JsonSerializer.Serialize(response));
        }
        
        public void StartRenderRequestedPageSnapshotsTask(CancellationToken cancellationToken)
        {
            RenderRequestedPageSnapshots();
        }
        
        private async Task RenderRequestedPageSnapshots()
        {
            SocketClient.OnMessageReceived += async message =>
            {
                var content = JsonDocument.Parse(message).RootElement;
                var channel = content.GetProperty("Channel").GetString();

                if (channel != "preview/requestPage")
                    return;

                var page = content.GetProperty("Payload").Deserialize<PageSnapshotIndex>();

                var image = CurrentDocumentSnapshot
                    .Pictures
                    .ElementAt(page.PageIndex)
                    .RenderImage(page.ZoomLevel);

                var response = new SocketMessage<PageSnapshotCommunicationData>
                {
                    Channel = "preview/updatePage",
                    Payload = new PageSnapshotCommunicationData
                    {
                        PageIndex = page.PageIndex, ZoomLevel = page.ZoomLevel, ImageData = image
                    }
                };
                
                SocketClient.SendMessage(JsonSerializer.Serialize(response));
            };
        }
    }
}

#endif
