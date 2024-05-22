#if NET6_0_OR_GREATER

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuestPDF.Previewer;

internal class SocketClient
{
    private TcpClient Client { get; set; }
    private NetworkStream Stream;

    private ConcurrentQueue<string> OutgoingMessages { get; } = new();
    private ConcurrentQueue<string> IncomingMessages { get; } = new();
    
    public event Func<string, Task>? OnMessageReceived;
    
    public SocketClient(string ipAddress, int port)
    {
        Client = new TcpClient();
        Client.Connect(IPAddress.Parse(ipAddress), port);
        Stream = Client.GetStream();
    }
    
    public SocketClient(TcpClient client)
    {
        Client = client;
        Stream = client.GetStream();
    }

    public Task StartCommunication(CancellationToken cancellationToken = default)
    {
        var taskWorkers = Enumerable
            .Range(0, Environment.ProcessorCount)
            .Select(_ => Task.Run(() => HandleTaskReceivers(cancellationToken)))
            .ToList();
        
        var runningTasks = new List<Task>
        {
            Task.Run(() => HandleIncomingMessages(cancellationToken)),
            Task.Run(() => HandleOutgoingMessages(cancellationToken))
        };
        
        return Task.WhenAll(taskWorkers.Concat(runningTasks));
    }
    
    public void SendMessage(string message)
    {
        OutgoingMessages.Enqueue(message);
    }

    private async Task HandleIncomingMessages(CancellationToken cancellationToken = default)
    {
        using var binaryStream = new BinaryReader(Stream);
        
        while (Client.Connected)
        {
            var messageLength = binaryStream.ReadInt32();
            var messageBytes = await ReadExactlyAsync(Stream, messageLength, cancellationToken);
                
            var message = Encoding.UTF8.GetString(messageBytes);
            IncomingMessages.Enqueue(message);
        }
        
        static async Task<byte[]> ReadExactlyAsync(Stream stream, int bytesToRead, CancellationToken cancellationToken = default)
        {
            var buffer = new byte[bytesToRead];
            
            var bytesRead = 0;

            while (bytesRead < bytesToRead)
            {
                var readBytes = await stream.ReadAsync(buffer, bytesRead, bytesToRead - bytesRead, cancellationToken);
                
                if (readBytes == 0)
                    throw new EndOfStreamException("Reached end of stream before reading the required number of bytes.");

                bytesRead += readBytes;
            }

            return buffer;
        }
    }
    
    private async Task HandleOutgoingMessages(CancellationToken cancellationToken = default)
    {
        while (Client.Connected)
        {
            if (!OutgoingMessages.TryDequeue(out var message))
            {
                await Task.Delay(10, cancellationToken);
                continue;
            }
            
            Console.WriteLine($"Sending message: {message}");
            
            var data = Encoding.UTF8.GetBytes(message);
            var length = BitConverter.GetBytes(data.Length);
            
            await Stream.WriteAsync(length, 0, length.Length, cancellationToken);
            await Stream.WriteAsync(data, 0, data.Length, cancellationToken);
        }
    }
    
    private async Task HandleTaskReceivers(CancellationToken cancellationToken = default)
    {
        while (Client.Connected)
        {
            if (!IncomingMessages.TryDequeue(out var message))
            {
                await Task.Delay(10, cancellationToken);
                continue;
            }
            
            if (OnMessageReceived == null)
                continue;
            
            await OnMessageReceived.Invoke(message);
        }
    }

    public void Close()
    {
        OnMessageReceived = null;
        
        Stream.Close();
        Client.Close();
    }
}

#endif