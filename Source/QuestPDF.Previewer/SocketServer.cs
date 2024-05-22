using System.Net;
using System.Net.Sockets;

namespace QuestPDF.Previewer;

internal class SocketServer
{
    private TcpListener Listener { get; }
    private bool IsServerRunning { get; set; }
    
    private SocketClient Client { get; set; }
    public event Func<string, Task>? OnMessageReceived;

    public SocketServer(string ipAddress, int port)
    {
        Listener = new TcpListener(IPAddress.Parse(ipAddress), port);
    }

    public void Start()
    {
        Listener.Start();
        IsServerRunning = true;
        
        Task.Run(() => ListenForClients());
    }

    public void Stop()
    {
        IsServerRunning = false;
        Listener.Stop();
    }

    private async Task ListenForClients()
    {
        while (IsServerRunning)
        {
            var client = await Listener.AcceptTcpClientAsync();
            Client?.Close();
            Client = new SocketClient(client);
            Client.OnMessageReceived += async message => await OnMessageReceived.Invoke(message);
            Client.StartCommunication();
        }
    }
    
    public void SendMessage(string message)
    {
        Client.SendMessage(message);
    }
}
