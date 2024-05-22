namespace QuestPDF.Previewer;

public class SocketMessage<TPayload>
{
    public string Channel { get; set; }
    public TPayload Payload { get; set; }
}