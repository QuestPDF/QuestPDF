#if NET8_0_OR_GREATER

using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using QuestPDF.Drawing.DocumentCanvases;

namespace QuestPDF.Companion
{
    internal sealed class CompanionService : IDisposable
    {
        internal const string ClientIdHeaderName = "X-QuestPDF-Client-Id";
        internal const string CodeLocationIdHeaderName = "X-QuestPDF-Code-Location-Id";
        
        private const int RequiredCompanionApiVersion = 4;
        private const int MaxDocumentStructureSizeInBytes = 64 * 1024 * 1024;

        private int Port { get; }

        // unique for every preview session: a restarted application, as well as another ShowInCompanion call, gets a new one
        private string ClientId { get; } = Guid.NewGuid().ToString();
        private HttpClient HttpClient { get; }
        
        private CompanionDocumentSnapshot? CurrentSnapshot { get; set; }
        private SemaphoreSlim CurrentSnapshotLock { get; } = new(1, 1);

        public static bool IsCompanionAttached { get; private set; }
        public static bool IsDocumentHotReloaded { get; set; }

        public CompanionService(int port)
        {
            IsCompanionAttached = true;
            Port = port;

            HttpClient = new()
            {
                BaseAddress = new Uri($"http://127.0.0.1:{port}/"),
                Timeout = TimeSpan.FromSeconds(10)
            };

            // every request tells the Companion app which session has sent it, and from which application:
            // the application folder stays the same when the application restarts (percent-encoded, as header values must be ASCII)
            HttpClient.DefaultRequestHeaders.Add(ClientIdHeaderName, ClientId);
            HttpClient.DefaultRequestHeaders.Add(CodeLocationIdHeaderName, Uri.EscapeDataString(AppContext.BaseDirectory));
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
            var isCompanionAppResponding = await IsCompanionResponding(cancellationToken);
            
            if (!isCompanionAppResponding)
            {
                StartCompanionApplication();
                await WaitForCompanionConnection(cancellationToken);
            }

            await CheckCompanionVersionCompatibility(cancellationToken);
        }

        private async Task<bool> IsCompanionResponding(CancellationToken cancellationToken)
        {
            try
            {
                using var result = await HttpClient.GetAsync("/ping", cancellationToken);
                return result.IsSuccessStatusCode;
            }
            catch when (!cancellationToken.IsCancellationRequested)
            {
                return false;
            }
        }

        private void StartCompanionApplication()
        {
            try
            {
                var startInfo = CreateCompanionStartInfo(Port);

                using var process = Process.Start(startInfo);
                
                if (process == null)
                    throw new InvalidOperationException("The process could not be started.");

                // On Windows, the launcher is a short-lived cmd.exe that only asks the shell to start Companion.
                // A failure to locate the Companion executable surfaces here as a non-zero exit code.
                if (startInfo.FileName != CompanionCommandName && process.WaitForExit(5000) && process.ExitCode != 0)
                    throw new InvalidOperationException($"The launcher exited with code {process.ExitCode}. Is the '{CompanionCommandName}' tool installed?");
            }
            catch (Exception exception)
            {
                throw new Exception($"The QuestPDF Companion application is not running, and the library was not able to launch it automatically. Please start the application manually. Learn more: https://www.questpdf.com/companion/usage.html", exception);
            }
        }

        private const string CompanionCommandName = "questpdf-companion";

        internal static ProcessStartInfo CreateCompanionStartInfo(int port)
        {
            var portText = port.ToString(CultureInfo.InvariantCulture);

            if (!OperatingSystem.IsWindows())
            {
                var unixStartInfo = new ProcessStartInfo(CompanionCommandName)
                {
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                unixStartInfo.ArgumentList.Add(portText);
                return unixStartInfo;
            }

            // When the IDE stops the debugged app (e.g. Stop in Rider), it kills the app's whole process tree on Windows,
            // so a Companion started as a child process would close too. Starting it through `cmd /c start` detaches it
            // from that tree, so it stays open between runs.
            // CreateNoWindow hides only the cmd.exe console. Companion's own window must not be hidden, or it stays hidden for good.
            var commandInterpreterPath = Environment.GetEnvironmentVariable("ComSpec") is { Length: > 0 } configuredPath 
                ? configuredPath 
                : "cmd.exe";

            var windowsStartInfo = new ProcessStartInfo(commandInterpreterPath)
            {
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // ArgumentList applies the standard Windows quoting; the empty string becomes the required "" window title.
            windowsStartInfo.ArgumentList.Add("/c");
            windowsStartInfo.ArgumentList.Add("start");
            windowsStartInfo.ArgumentList.Add("");
            windowsStartInfo.ArgumentList.Add(CompanionCommandName);
            windowsStartInfo.ArgumentList.Add(portText);
            return windowsStartInfo;
        }

        private async Task WaitForCompanionConnection(CancellationToken cancellationToken)
        {
            var launchTimeout = TimeSpan.FromSeconds(10);
            var deadline = DateTime.UtcNow + launchTimeout;

            while (DateTime.UtcNow < deadline)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);

                if (await IsCompanionResponding(cancellationToken))
                    return;
            }
            
            throw new Exception($"The library was not able to launch the QuestPDF Companion application automatically: it did not accept connections on port {Port}. The port may be used by another program, which prevents the Companion application from starting: please stop that program or use a different port. If the Companion application is already running, please close it first, as it keeps listening on the port it was originally started with. Learn more: https://www.questpdf.com/companion/usage.html");
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
        /// Sends a single heartbeat. Returns false when the Companion app has been closed or has stopped accepting this application.
        /// </summary>
        private async Task<bool> SendHeartbeat(CancellationToken cancellationToken)
        {
            try
            {
                using var result = await HttpClient.PostAsJsonAsync($"/v{RequiredCompanionApiVersion}/notify", new CompanionCommands.Notify(), CompanionJsonContext.Default.Notify, cancellationToken);
                
                // the Companion app has given up on this application (e.g. it was unresponsive for too long) and shows its welcome screen
                return result.StatusCode != HttpStatusCode.Conflict;
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

            using var content = JsonContent.Create(command, CompanionJsonContext.Default.UpdateDocumentStructure);
           
            try
            {
                await content.LoadIntoBufferAsync(MaxDocumentStructureSizeInBytes);
            }
            catch (HttpRequestException ex) when (ex.HttpRequestError == HttpRequestError.ConfigurationLimitExceeded)
            {
                throw new InvalidOperationException(
                    "The document is too complex to be previewed in the QuestPDF Companion application. " +
                    "To ensure a responsive preview and quick hot-reload, please consider reducing the amount of content in the document during development. " +
                    "This applies only to the preview and does not affect PDF generation.",
                    ex);
            }

            using var result = await HttpClient.PostAsync($"/v{RequiredCompanionApiVersion}/documentPreview/update", content, cancellationToken);
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
