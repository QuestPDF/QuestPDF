using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace QuestPDF.Previewer;

class CommunicationService
{
    public static CommunicationService Instance { get; } = new ();
    
    public event Action<DocumentStructure>? OnDocumentUpdated;
    public Func<ICollection<PageSnapshotIndex>>? OnPageSnapshotsRequested { get; set; }
    public Action<ICollection<RenderedPageSnapshot>> OnPageSnapshotsProvided  { get; set; }

    private WebApplication? Application { get; set; }

    private readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private CommunicationService()
    {
        
    }
    
    public Task Start(int port)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddLogging(x => x.ClearProviders());
        builder.WebHost.UseKestrel(options => options.Limits.MaxRequestBodySize = null);
        Application = builder.Build();

        Application.MapGet("ping", HandlePing);
        Application.MapGet("version", HandleVersion);
        Application.MapPost("preview/update", HandlePreviewRefresh);
        Application.MapGet("preview/getRenderingRequests", HandleGetRequests);
        Application.MapPost("preview/provideRenderedImages", HandleProvidedSnapshotImages);
            
        return Application.RunAsync($"http://localhost:{port}/");
    }

    public async Task Stop()
    {
        await Application.StopAsync();
        await Application.DisposeAsync();
    }

    private async Task<IResult> HandlePing()
    {
        return OnDocumentUpdated == null 
            ? Results.StatusCode(StatusCodes.Status503ServiceUnavailable) 
            : Results.Ok();
    }
    
    private async Task<IResult> HandleVersion()
    {
        return Results.Json(GetType().Assembly.GetName().Version);
    }
    
    private async Task HandlePreviewRefresh(DocumentStructure documentStructure)
    {
        Task.Run(() => OnDocumentUpdated(documentStructure));
    }

    private async Task<ICollection<PageSnapshotIndex>> HandleGetRequests()
    {
        return OnPageSnapshotsRequested();
    }
    
    private async Task HandleProvidedSnapshotImages(HttpRequest request)
    {
        var renderedPageIndexes = JsonSerializer.Deserialize<ICollection<PageSnapshotIndex>>(request.Form["metadata"], JsonSerializerOptions);
        var renderedPages = new List<RenderedPageSnapshot>();

        foreach (var index in renderedPageIndexes)
        {
            using var memoryStream = new MemoryStream();
            await request.Form.Files.GetFile(index.ToString()).CopyToAsync(memoryStream);
            var image = SKImage.FromEncodedData(memoryStream.ToArray()).ToRasterImage(true);

            var renderedPage = new RenderedPageSnapshot
            {
                ZoomLevel = index.ZoomLevel, PageIndex = index.PageIndex, Image = image
            };
            
            renderedPages.Add(renderedPage);
        }

        Task.Run(() => OnPageSnapshotsProvided(renderedPages));
    }
}