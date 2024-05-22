using System.Text.Json;
using SkiaSharp;

namespace QuestPDF.Previewer;

class CommunicationService
{
    public static CommunicationService Instance { get; } = new ();
    
    public event Action<DocumentStructure>? OnDocumentUpdated;
    public Action<RenderedPageSnapshot> OnPageSnapshotsProvided  { get; set; }

    private SocketServer? Server { get; set; }

    private readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private CommunicationService()
    {
        
    }
    
    public void Start(int port)
    {
        Server = new SocketServer("127.0.0.1", port);
        
        Server.OnMessageReceived += async message =>
        {
            var content = JsonDocument.Parse(message).RootElement;
            var channel = content.GetProperty("Channel").GetString();
            
            if (channel == "ping/check")
            {
                if (OnDocumentUpdated == null)
                    return;
                
                var response = new SocketMessage<string>
                {
                    Channel = "ping/ok",
                    Payload = GetType().Assembly.GetName().Version.ToString()
                };
                
                Server.SendMessage(JsonSerializer.Serialize(response));
            }
            else if (channel == "version/check")
            {
                var response = new SocketMessage<string>
                {
                    Channel = "version/provide",
                    Payload = GetType().Assembly.GetName().Version.ToString()
                };
                
                Server.SendMessage(JsonSerializer.Serialize(response));
            }
            else if (channel == "preview/update")
            {
                var documentStructure = content.GetProperty("Payload").Deserialize<DocumentStructure>();
                Task.Run(() => OnDocumentUpdated(documentStructure));
            }
            else if (channel == "preview/updatePage")
            {
                var previewData = content.GetProperty("Payload").Deserialize<PageSnapshotCommunicationData>();
                var image = SKImage.FromEncodedData(previewData.ImageData).ToRasterImage(true);

                var renderedPage = new RenderedPageSnapshot
                {
                    ZoomLevel = previewData.ZoomLevel, 
                    PageIndex = previewData.PageIndex, 
                    Image = image
                };
            
                Task.Run(() => OnPageSnapshotsProvided(renderedPage));
            }
        };
        
        Server.Start();
    }

    public void RequestNewPage(PageSnapshotIndex index)
    {
        Task.Run(() =>
        {
            var message = new SocketMessage<PageSnapshotIndex>
            {
                Channel = "preview/requestPage",
                Payload = index
            };
            
            Server.SendMessage(JsonSerializer.Serialize(message));
        });
    }
}