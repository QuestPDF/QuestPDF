using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace QuestPDF.Previewer;

class CommunicationService
{
    public static CommunicationService Instance { get; } = new ();
    
    public event Action<ICollection<PreviewPage>> OnDocumentRefreshed;

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
        Application = builder.Build();

        Application.MapGet("ping", () => Results.Ok());
        Application.MapGet("version", () => Results.Json(GetType().Assembly.GetName().Version));
        Application.MapPost("update/preview", HandleUpdatePreview);
            
        return Application.RunAsync($"http://localhost:{port}/");
    }

    public async Task Stop()
    {
        await Application.StopAsync();
        await Application.DisposeAsync();
    }

    private async Task<IResult> HandleUpdatePreview(HttpRequest request)
    {
        var command = JsonSerializer.Deserialize<DocumentSnapshot>(request.Form["command"], JsonSerializerOptions);

        var pages = command
            .Pages
            .Select(page =>
            {
                using var stream = request.Form.Files[page.Id].OpenReadStream();
                var picture = SKPicture.Deserialize(stream);
                        
                return new PreviewPage(picture, page.Width, page.Height);
            })
            .ToList();

        Task.Run(() => OnDocumentRefreshed(pages));
        return Results.Ok();
    }
}