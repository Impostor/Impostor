using System;
using System.Net;
using System.Threading.Tasks;
using Impostor.Api.Net;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Hazel;

internal class HazelConnection : IHazelConnection
{
    private readonly ILogger<HazelConnection> _logger;

    public HazelConnection(Connection innerConnection, ILogger<HazelConnection> logger)
    {
        _logger = logger;
        InnerConnection = innerConnection;
        innerConnection.DataReceived = ConnectionOnDataReceived;
        innerConnection.Disconnected = ConnectionOnDisconnected;
    }

    public Connection InnerConnection { get; }

    public IPEndPoint EndPoint
    {
        get => InnerConnection.EndPoint;
    }

    public bool IsConnected
    {
        get => InnerConnection.State == ConnectionState.Connected;
    }

    public IClient? Client { get; set; }

    public float AveragePing
    {
        get => InnerConnection is NetworkConnection networkConnection ? networkConnection.AveragePingMs : 0;
    }

    public ValueTask SendAsync(IMessageWriter writer)
    {
        return InnerConnection.SendAsync(writer);
    }

    public ValueTask DisconnectAsync(string? reason, IMessageWriter? writer = null)
    {
        return InnerConnection.Disconnect(reason, writer as MessageWriter);
    }

    public void DisposeInnerConnection()
    {
        InnerConnection.Dispose();
    }

    private async ValueTask ConnectionOnDisconnected(DisconnectedEventArgs e)
    {
        try
        {
            if (Client != null)
            {
                await Client.HandleDisconnectAsync(e.Reason);
            }
        }
        catch
        {
            // ignored
        }
    }

    private async ValueTask ConnectionOnDataReceived(DataReceivedEventArgs e)
    {
        if (Client == null)
        {
            return;
        }
        
        while (true)
        {
            if (e.Message.Position >= e.Message.Length)
            {
                break;
            }
            
            if (!IsConnected)
            {
                break;
            }
            
            try
            {
                using var message = e.Message.ReadMessage();
                await Client.HandleMessageAsync(message, e.Type);
            }
            catch
            {
                _logger.LogWarning("Error readMessage Form {connection}", InnerConnection.EndPoint);
            }
        }
    }
}
