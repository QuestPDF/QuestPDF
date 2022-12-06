#if NETCOREAPP3_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Previewer.Inspection;

namespace QuestPDF.Previewer
{
    internal class PreviewerService
    {
        private string ClientId { get; } = Guid.NewGuid().ToString("D");
        private string LibraryVersion { get; } = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        
        private int Port { get; }
        private HttpClient HttpClient { get; }
        
        public event Action? OnLostConnection;
        
        private const int RequiredPreviewerVersionMajor = 2023;
        private const int RequiredPreviewerVersionMinor = 1;
        
        public PreviewerService(int port)
        {
            Port = port;
            HttpClient = new()
            {
                BaseAddress = new Uri($"http://localhost:{port}/"), 
                Timeout = TimeSpan.FromSeconds(1)
            };
        }
        
        internal static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            MaxDepth = 256,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        #region Connection
        
        public async Task Connect()
        {
            var isAvailable = await IsPreviewerAvailable();
        
            if (!isAvailable)
            {
                StartPreviewer();
                await WaitForConnection();
            }
            
            var previewerVersion = await GetPreviewerVersion();
            await CheckVersionCompatibility(previewerVersion);

            StartNotifyPresenceTask();
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
            }
            catch
            {
                throw new Exception("Cannot start the QuestPDF Previewer tool. " +
                                    "Please install it by executing in terminal the following command: 'dotnet tool install --global QuestPDF.Previewer'.");
            }
        }
        
        private async Task WaitForConnection()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(10));
            
            var cancellationToken = cancellationTokenSource.Token; 
            
            while (true)
            {
                // ReSharper disable once MethodSupportsCancellation
                await Task.Delay(TimeSpan.FromMilliseconds(250));
        
                if (cancellationToken.IsCancellationRequested)
                    throw new Exception($"Cannot connect to the QuestPDF Previewer tool. Please make sure that your Operating System does not block HTTP connections on port {Port}.");
        
                var isConnected = await IsPreviewerAvailable();
        
                if (isConnected)
                    break;
            }
        }
        
        #endregion
        
        #region Checking compatibility
        
        private async Task<Version> GetPreviewerVersion()
        {
            using var result = await HttpClient.GetAsync("/version");
            return await result.Content.ReadFromJsonAsync<Version>();
        }

        private async Task CheckVersionCompatibility(Version version)
        {
            if (version.Major == RequiredPreviewerVersionMajor && version.Minor == RequiredPreviewerVersionMinor)
                return;

            await ShowIncompatibleVersion();
            
            throw new Exception($"Previewer version is not compatible. Possible solutions: " +
                                $"1) Update the QuestPDF library to newer version. " +
                                $"2) Update the QuestPDF previewer tool using the following command: 'dotnet tool update --global QuestPDF.Previewer --version {RequiredPreviewerVersionMajor}.{RequiredPreviewerVersionMinor}'");
        }

        private async Task ShowIncompatibleVersion()
        {
            var payload = new IncompatibleVersionApiRequest();
            await HttpClient.PostAsJsonAsync("/update/incompatibleVersion", payload, JsonSerializerOptions);
        }
        
        #endregion
        
        #region Reporting presence
        
        private void StartNotifyPresenceTask()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await NotifyPresence();
                    await Task.Delay(1000);
                }
            });
        }
        
        private async Task NotifyPresence()
        {
            var payload = new NotifyPresenceApiRequest
            {
                Id = ClientId,
                LibraryVersion = LibraryVersion,
                IsDotnet6OrBeyond = RuntimeDetector.IsNet6OrGreater,
                IsDotnet3OrBeyond = RuntimeDetector.IsNet3OrGreater,
                IsExecutedInUnitTest = UnitTestDetector.RunningInUnitTest
            };
 
            await HttpClient.PostAsJsonAsync("/update/notifyPresence", payload);
        }
        
        #endregion
        
        #region Updating state

        public async Task ShowDocumentPreview(DocumentPreviewResult documentPreviewResult)
        {
            using var multipartContent = new MultipartFormDataContent();
            
            var pages = new List<DocumentPreviewApiRequest.PageSnapshot>();
            
            foreach (var snapshot in documentPreviewResult.PageSnapshots)
            {
                var page = new DocumentPreviewApiRequest.PageSnapshot
                {
                    PageNumber = documentPreviewResult.PageSnapshots.IndexOf(snapshot) + 1,
                    Width = snapshot.Size.Width,
                    Height = snapshot.Size.Height
                };
                
                pages.Add(page);
            
                var pictureStream = snapshot.Picture.Serialize().AsStream();
                multipartContent.Add(new StreamContent(pictureStream), "snapshots", page.ResourceId);
            }
            
            var request = new DocumentPreviewApiRequest
            {
                PageSnapshots = pages, 
                DocumentHierarchy = documentPreviewResult.DocumentHierarchy
            };
            
            var json = JsonSerializer.Serialize(request, JsonSerializerOptions);
            var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
            multipartContent.Add(jsonContent, "request");
            
            using var response = await HttpClient.PostAsync("/update/documentPreview", multipartContent);
            response.EnsureSuccessStatusCode();
            
            foreach (var picture in documentPreviewResult.PageSnapshots)
                picture.Picture.Dispose();
        }
        
        public async Task ShowGenericError(Exception exception)
        {
            var payload = new GenericErrorApiRequest
            {
                Error = MapException(exception)
            };

            await HttpClient.PostAsJsonAsync("/update/genericError", payload, JsonSerializerOptions);
            
            static GenericError? MapException(Exception? exception)
            {
                if (exception == null)
                    return null;
                        
                return new GenericError
                {
                    ExceptionType = exception.GetType().FullName, 
                    Message = exception.Message, 
                    StackTrace = exception.StackTrace, 
                    InnerException = MapException(exception.InnerException)
                };
            }
        }
        
        public async Task ShowLayoutError(DocumentLayoutException exception)
        {
            var payload = new LayoutErrorApiRequest
            {
                Trace = exception.ElementTrace
            };
            
            await HttpClient.PostAsJsonAsync("/update/layoutError", payload, JsonSerializerOptions);
        }
        
        #endregion
    }
}

#endif
