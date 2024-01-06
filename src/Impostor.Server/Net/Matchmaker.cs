using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Impostor.Api.Events.Managers;
using Impostor.Api.Net.Messages.C2S;
using Impostor.Hazel;
using Impostor.Hazel.Udp;
using Impostor.Server.Events.Client;
using Impostor.Server.Net.Hazel;
using Impostor.Server.Net.Manager;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;

namespace Impostor.Server.Net;

internal class Matchmaker(
    IEventManager eventManager,
    ClientManager clientManager,
    ObjectPool<MessageReader> readerPool,
    ILogger<HazelConnection> connectionLogger)
{
    private UdpConnectionListener? _connection;

    public async ValueTask StartAsync(IPEndPoint ipEndPoint)
    {
        var mode = ipEndPoint.AddressFamily switch
        {
            AddressFamily.InterNetwork => IPMode.IPv4,
            AddressFamily.InterNetworkV6 => IPMode.IPv6,
            _ => throw new InvalidOperationException(),
        };

        _connection = new UdpConnectionListener(ipEndPoint, readerPool, mode)
        {
            NewConnection = OnNewConnection,
        };

        await _connection.StartAsync();
    }

    public async ValueTask StopAsync()
    {
        if (_connection != null)
        {
            await _connection.DisposeAsync();
        }
    }

    private async ValueTask OnNewConnection(NewConnectionEventArgs e)
    {
        // Handshake.
        HandshakeC2S.Deserialize(e.HandshakeData, out var clientVersion, out var name, out var language, out var chatMode, out var platformSpecificData);

        var connection = new HazelConnection(e.Connection, connectionLogger);

        await eventManager.CallAsync(new ClientConnectionEvent(connection, e.HandshakeData));

        // Register client
        await clientManager.RegisterConnectionAsync(connection, name, clientVersion, language, chatMode, platformSpecificData);
    }
}
