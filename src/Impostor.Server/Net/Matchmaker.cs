using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Impostor.Api.Config;
using Impostor.Api.Events.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Hazel;
using Impostor.Hazel.Dtls;
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
    private readonly List<ConnectionData> _connectionDataS = [];
    private AuNetListeners? _auNetListeners;

    private X509Certificate2? _certificate2Collection;

    private uint LastId;

    private string ServerCertification = string.Empty;

    public ConnectionData GetOrCreateConnectionData(string matchmakerToken)
    {
        if (_connectionDataS.Exists(n => n.MatchmakerToken == matchmakerToken))
        {
            return _connectionDataS.First(n => n.MatchmakerToken == matchmakerToken);
        }

        var data = new ConnectionData
        {
            MatchmakerToken = matchmakerToken,
        };

        _connectionDataS.Add(data);
        return data;
    }

    public ConnectionData? GetConnectionData(uint lastId)
    {
        return _connectionDataS.Exists(n => n.LastId == lastId)
            ? _connectionDataS.First(n => n.LastId == lastId)
            : null;
    }

    public async ValueTask StartAsync(IPEndPoint ipEndPoint, ServerConfig serverConfig)
    {
        if (serverConfig.ServerCertification == string.Empty)
        {
            serverConfig.UseOnline = false;
            serverConfig.UseDtl = false;
        }
        else
        {
            ServerCertification = serverConfig.ServerCertification;
        }

        if ((serverConfig.UseOnline || serverConfig.UseDtl) && _certificate2Collection == null)
        {
            var x509Certificate = new X509Certificate2(DecodePem(ServerCertification));
        }

        var mode = ipEndPoint.AddressFamily switch
        {
            AddressFamily.InterNetwork => IPMode.IPv4,
            AddressFamily.InterNetworkV6 => IPMode.IPv6,
            _ => throw new InvalidOperationException(),
        };

        _auNetListeners = await CreateListenerAsync(ipEndPoint, mode, serverConfig);

        await _auNetListeners.StartAsync();
    }

    public async ValueTask StopAsync()
    {
        if (_auNetListeners != null)
        {
            await _auNetListeners.StopAsync();
        }
    }

    private async ValueTask OnUDPConnection(NewConnectionEventArgs e)
    {
        // Handshake.
        var reader = e.HandshakeData;
        var clientVersion = reader.ReadGameVersion();
        var name = reader.ReadString();
        var lastId = reader.ReadUInt32();
        var message = reader.ReadMessage();
        var data = GetConnectionData(lastId) ?? new ConnectionData();
        data.Version ??= clientVersion;
        data.Name = name;
        data.Language = (SupportedLanguages)reader.ReadUInt32();
        data.ChatMode = (QuickChatModes)reader.ReadByte();
        data.Platforms = (Platforms)message.Tag;
        data.PlatformName = message.ReadString();
        data.PlatformId = message.ReadUInt32();

        _ = reader.ReadString();
        _ = reader.ReadUInt32();

        var connection = new HazelConnection(e.Connection, connectionLogger);

        await eventManager.CallAsync(new ClientConnectionEvent(connection, e.HandshakeData));

        // Register client
        await clientManager.RegisterConnectionAsync(data);
    }

    private async ValueTask OnOnlineConnection(NewConnectionEventArgs e)
    {
        LastId++;
        var reader = e.HandshakeData;
        var connection = e.Connection;

        var version = reader.ReadGameVersion();
        var platform = (Platforms)reader.ReadByte();
        var matchmakerToken = reader.ReadString();
        var friendCode = reader.ReadString();

        var data = GetOrCreateConnectionData(matchmakerToken);
        data.LastId = LastId;
        data.Platforms = platform;
        data.FriendCode = friendCode;
        data.Version = version;

        var writer = MessageWriter.Get();
        writer.StartMessage(1);
        writer.Write(LastId);
        writer.EndMessage();
        await connection.SendAsync(writer);
    }

    private async ValueTask OnDtlConnection(NewConnectionEventArgs e)
    {
        // Handshake.
        var reader = e.HandshakeData;
        var clientVersion = reader.ReadGameVersion();
        var name = reader.ReadString();
        var matchmakerToken = reader.ReadString();
        var message = reader.ReadMessage();
        var data = GetOrCreateConnectionData(matchmakerToken);
        data.HandshakeData = reader;
        data.Version ??= clientVersion;
        data.Name = name;
        data.Language = (SupportedLanguages)reader.ReadUInt32();
        data.ChatMode = (QuickChatModes)reader.ReadByte();
        data.Platforms = (Platforms)message.Tag;
        data.PlatformName = message.ReadString();
        data.PlatformId = message.ReadUInt32();

        data.FriendCode = reader.ReadString();

        var connection = new HazelConnection(e.Connection, connectionLogger);
        data._connection = connection;

        await eventManager.CallAsync(new ClientConnectionEvent(connection, e.HandshakeData));

        // Register client
        await clientManager.RegisterConnectionAsync(data);
    }

    private async Task<AuNetListeners> CreateListenerAsync(IPEndPoint ipEndPoint, IPMode mode,
        ServerConfig serverConfig)
    {
        if (_auNetListeners != null)
        {
            await _auNetListeners.StopAsync();
        }

        var listener = new AuNetListeners();
        if (serverConfig.UseUDP)
        {
            listener._udpConnectionListener = new UdpConnectionListener(ipEndPoint, readerPool, mode)
            {
                NewConnection = OnUDPConnection,
            };

            if (serverConfig.UseOnline)
            {
                var onlineIpEndPoint = new IPEndPoint(ipEndPoint.Address, ipEndPoint.Port + 2);
                listener._onlineConnectionListener = new DtlsConnectionListener(onlineIpEndPoint, readerPool, mode)
                {
                    NewConnection = OnOnlineConnection,
                };
                listener._onlineConnectionListener.SetCertificate(_certificate2Collection);
            }
        }

        if (!serverConfig.UseDtl)
        {
            return listener;
        }

        var dtlIpEndPoint = new IPEndPoint(ipEndPoint.Address, ipEndPoint.Port + 3);
        listener._dtlsConnectionListener = new DtlsConnectionListener(dtlIpEndPoint, readerPool, mode)
        {
            NewConnection = OnDtlConnection,
        };
        listener._dtlsConnectionListener.SetCertificate(_certificate2Collection);

        return listener;
    }

    private static byte[] DecodePem(string pemData)
    {
        var list = new List<byte>();
        pemData = pemData.Replace("\r", string.Empty);
        foreach (var text in pemData.Split(['\n']))
        {
            if (text.StartsWith("-----"))
            {
                continue;
            }

            var array2 = Convert.FromBase64String(text);
            list.AddRange(array2);
        }

        return list.ToArray();
    }
}
