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
    
    public event Action<DocumentSnapshot>? OnDocumentRefreshed;

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
        Application.MapPost("update/preview", HandleUpdatePreview);
            
        return Application.RunAsync($"http://localhost:{port}/");
    }

    public async Task Stop()
    {
        await Application.StopAsync();
        await Application.DisposeAsync();
    }

    private async Task<IResult> HandlePing()
    {
        return OnDocumentRefreshed == null 
            ? Results.StatusCode(StatusCodes.Status503ServiceUnavailable) 
            : Results.Ok();
    }
    
    private async Task<IResult> HandleVersion()
    {
        return Results.Json(GetType().Assembly.GetName().Version);
    }
    
    private async Task<IResult> HandleUpdatePreview(HttpRequest request)
    {
        var documentSnapshot = JsonSerializer.Deserialize<DocumentSnapshot>(request.Form["command"], JsonSerializerOptions);

        foreach (var pageSnapshot in documentSnapshot.Pages)
        {
            using var stream = request.Form.Files[pageSnapshot.Id].OpenReadStream();
            pageSnapshot.Picture = SKPicture.Deserialize(stream);
        }

        Task.Run(() => OnDocumentRefreshed(documentSnapshot));
        return Results.Ok();
    }
}