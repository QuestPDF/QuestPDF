#if NET6_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using QuestPDF.Drawing;

namespace QuestPDF.Previewer
{
    internal class PreviewerService
    {
        private int Port { get; }
        private HttpClient HttpClient { get; }
        
        public  event Action? OnPreviewerStopped;

        private const int RequiredPreviewerVersionMajor = 2023;
        private const int RequiredPreviewerVersionMinor = 12;
        
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

            var newLine = Environment.NewLine;
            var newParagraph = newLine + newLine;
            
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
                    throw new Exception($"Cannot connect to the QuestPDF Previewer tool. Please make sure that your Operating System does not block HTTP connections on port {Port}.");

                var isConnected = await IsPreviewerAvailable();

                if (isConnected)
                    break;
            }
        }
        
        public async Task RefreshPreview(PreviewerDocumentSnapshot previewerDocumentSnapshot)
        {
            using var multipartContent = new MultipartFormDataContent();

            var pages = new List<PreviewerRefreshCommand.Page>();
            
            foreach (var picture in previewerDocumentSnapshot.Pictures)
            {
                var page = new PreviewerRefreshCommand.Page
                {
                    Width = picture.Size.Width,
                    Height = picture.Size.Height
                };
                
                pages.Add(page);

                var pictureStream = picture.Picture.Serialize().AsStream();
                multipartContent.Add(new StreamContent(pictureStream), page.Id, page.Id);
            }

            var command = new PreviewerRefreshCommand
            {
                DocumentContentHasLayoutOverflowIssues = previewerDocumentSnapshot.DocumentContentHasLayoutOverflowIssues,
                Pages = pages
            };
            
            multipartContent.Add(JsonContent.Create(command), "command");

            using var _ = await HttpClient.PostAsync("/update/preview", multipartContent);

            foreach (var picture in previewerDocumentSnapshot.Pictures)
                picture.Picture.Dispose();
        }
    }
}

#endif
