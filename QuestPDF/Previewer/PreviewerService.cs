#if NET6_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using QuestPDF.Drawing;

namespace QuestPDF.Previewer
{
    internal class PreviewerService
    {
        private HttpClient HttpClient { get; init; }
        
        public PreviewerService(int port)
        {
            HttpClient = new()
            {
                BaseAddress = new Uri($"http://localhost:{port}/"), 
                Timeout = TimeSpan.FromMilliseconds(100)
            };
        }

        public async Task Connect()
        {
            var isConnected = await TestConnection();
            
            if (isConnected)
                return;

            StartPreviewer();
            await WaitForConnection();
        }

        private async Task<bool> TestConnection()
        {
            try
            {
                var result = await HttpClient.GetAsync("/ping");
                return result.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
        
        private async Task<Version> GetPreviewerVersion()
        {
            var result = await HttpClient.GetAsync("/version");
            var value = await result.Content.ReadAsStringAsync();
            return Version.Parse(value);
        }
        
        private static void StartPreviewer()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new()
                    {
                        UseShellExecute = false,
                        FileName = "questpdf-test",
                        CreateNoWindow = true
                    }
                };
                
                process.Start();
            }
            catch
            {
                throw new Exception("Cannot start the QuestPDF Previewer tool. Please install it by executing in terminal the following command: 'dotnet tool install --global QuestPDF.Previewer'.");
            }
        }

        private void CheckVersionCompatibility(Version version)
        {
            
        }
        
        private async Task WaitForConnection()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));

                var isConnected = await TestConnection();

                if (isConnected)
                    break;
            }
        }
        
        public async Task RefreshPreview(ICollection<PreviewerPicture> pictures)
        {
            var multipartContent = new MultipartFormDataContent();

            var pages = new List<PreviewerRefreshCommand.Page>();
            
            foreach (var picture in pictures)
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
                Pages = pages
            };
            
            multipartContent.Add(JsonContent.Create(command), "command");

            await HttpClient.PostAsync("/update/preview", multipartContent);

            foreach (var picture in pictures)
                picture.Picture.Dispose();
        }
    }
}

#endif
