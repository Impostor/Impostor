using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Impostor.Api.Config;
using Impostor.Api.Events.Managers;
using Impostor.Api.Net.Manager;
using Impostor.Api.Net.Messages.C2S;
using Impostor.Api.Utils;
using Impostor.Server.Events.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Next.Hazel.Dtls;
using Next.Hazel.Udp;

namespace Impostor.Server.Net.Manager;

internal sealed class NetListenerManager(
    ILogger<NetListenerManager> logger,
    ILogger<HazelConnection> connectionLogger,
    ObjectPool<MessageReader> readerPool,
    IEventManager eventManager,
    ClientManager clientManager,
    ClientAuthManager clientAuthManager
) : INetListenerManager
{
    public List<ListenerInfo> Listeners { get; } = [];

    public Dictionary<(string, string), X509Certificate2> CachedCertificates { get; } = new();

    public void Create(ListenerConfig config, int index = 0)
    {
        if (!CheckConfig(config))
        {
            logger.LogWarning("config is invalid, config: {config}", index);
            return;
        }

        NetworkConnectionListener listener = config.IsDtl
            ? CreateDtls(config.ListenIp, config.ListenPort + 3, ev => OnConnectionAsync(ev, true, config))
            : CreateUdp(config.ListenIp, config.ListenPort, ev => OnConnectionAsync(ev, false, config));

        var authListener = config.HasAuth
            ? CreateDtls(config.ListenIp, config.ListenPort + 2, OnAuthConnectionAsync)
            : null;

        Listeners.Add(SetCertificate(config, listener, authListener));
    }

    private ListenerInfo SetCertificate(ListenerConfig config, NetworkConnectionListener? listener,
        DtlsConnectionListener? authListener)
    {
        if (!CachedCertificates.TryGetValue((config.CertificatePath, config.PrivateKeyPath), out var certificate))
        {
            if (File.Exists(config.CertificatePath) && File.Exists(config.PrivateKeyPath))
            {
                logger.LogInformation("New certificate loaded: {certificatePath} {privateKeyPath}", config.CertificatePath, config.PrivateKeyPath);
                var newCertificate = DtlsHelper.GetCertificate(File.ReadAllText(config.CertificatePath),
                    File.ReadAllText(config.PrivateKeyPath));
                certificate = CachedCertificates[(config.CertificatePath, config.PrivateKeyPath)] = newCertificate;
            }
        }

        if (certificate == null)
        {
            return new ListenerInfo(config, listener, authListener);
        }

        if (listener is DtlsConnectionListener dtlsListener)
        {
            dtlsListener.SetCertificate(certificate);
        }

        authListener?.SetCertificate(certificate);

        return new ListenerInfo(config, listener, authListener);
    }

    public UdpConnectionListener CreateUdp(string ip, int port, Func<NewConnectionEventArgs, ValueTask> newConnection)
    {
        var address = IPAddress.Parse(ip.ResolveIp());
        var endpoint = new IPEndPoint(address, port);
        var mode = endpoint.AddressFamily switch
        {
            AddressFamily.InterNetwork => IPMode.IPv4,
            AddressFamily.InterNetworkV6 => IPMode.IPv6,
            _ => throw new InvalidOperationException(),
        };
        return new UdpConnectionListener(endpoint, readerPool, mode)
        {
            NewConnection = newConnection,
        };
    }

    public DtlsConnectionListener CreateDtls(string ip, int port, Func<NewConnectionEventArgs, ValueTask> newConnection)
    {
        var address = IPAddress.Parse(ip.ResolveIp());
        var endpoint = new IPEndPoint(address, port);
        var mode = endpoint.AddressFamily switch
        {
            AddressFamily.InterNetwork => IPMode.IPv4,
            AddressFamily.InterNetworkV6 => IPMode.IPv6,
            _ => throw new InvalidOperationException(),
        };
        return new DtlsConnectionListener(endpoint, readerPool, mode)
        {
            NewConnection = newConnection,
        };
    }

    private bool CheckConfig(ListenerConfig config)
    {
        if (Listeners.Any(n => n.Config.ListenIp == config.ListenIp && n.Config.ListenPort == config.ListenPort))
        {
            logger.LogWarning("Ip: {ip} Port:{port} already exists", config.ListenIp, config.ListenPort);
            return false;
        }

        if (config.HasAuth || config.IsDtl)
        {
            logger.LogWarning("Dtls and auth is not supported yet");
            
            if (config is { PrivateKeyPath: "" } or { CertificatePath: "" })
            {
                logger.LogWarning("private key or certificate path is empty not use dtl and auth");
                return false;
            }
        }

        return true;
    }

    public async Task StartAllAsync()
    {
        foreach (var info in Listeners)
        {
            try
            {
                if (info.Listener is null)
                {
                    continue;
                }

                await info.Listener.StartAsync();

                if (info.AuthListener is null)
                {
                    continue;
                }

                await info.AuthListener.StartAsync();
            }
            catch (Exception e)
            {
                logger.LogError(
                    "Failed to start listener ip:{ip} port:{port} dtl:{dtl} auth:{auth} :\n{e}",
                    info.Config.ListenIp,
                    info.Config.ListenPort,
                    info.Config.IsDtl,
                    info.Config.HasAuth,
                    e);
            }
        }
    }

    public async Task StopAllAsync()
    {
        foreach (var info in Listeners)
        {
            try
            {
                if (info.Listener is null)
                {
                    continue;
                }

                await info.Listener.DisposeAsync();

                if (info.AuthListener is null)
                {
                    continue;
                }

                await info.AuthListener.DisposeAsync();
            }
            catch (Exception e)
            {
                logger.LogError(
                    "Failed to stop listener ip:{ip} port:{port} dtl:{dtl} auth:{auth} :\n{e}",
                    info.Config.ListenIp,
                    info.Config.ListenPort,
                    info.Config.IsDtl,
                    info.Config.HasAuth,
                    e);
            }
        }
    }

    private async ValueTask OnAuthConnectionAsync(NewConnectionEventArgs eventArgs)
    {
        AuthHandshakeC2S.Deserialize(eventArgs.HandshakeData, out var version, out var platform,
            out var matchmakerToken, out var friendCode);
        var id = clientAuthManager.CreateAuthInfo(version, platform, matchmakerToken, friendCode);
        using var writer = MessageWriter.Get(MessageType.Reliable);
        writer.StartMessage(1);
        writer.Write(id);
        writer.EndMessage();
        logger.LogInformation("Has New Auth Ip:{ip} LastId:{Id}", eventArgs.Connection.EndPoint.ToString(), id);
        await eventArgs.Connection.SendAsync(writer);
    }

    private async ValueTask OnConnectionAsync(NewConnectionEventArgs eventArgs, bool isDtl, ListenerConfig config)
    {
        // Handshake.
        HandshakeC2S.Deserialize(
            eventArgs.HandshakeData, isDtl,
            out var clientVersion, out var name,
            out var language, out var chatMode,
            out var platformSpecificData, out var matchmakerToken,
            out var lastId, out var friendCode
        );
        
        logger.LogInformation(
            "Has New Connection Ip:{ip} isDtl:{dtl} Name:{name} Token:{token} FriendCode:{code} LastId:{Id}",
            eventArgs.Connection.EndPoint.ToString(), isDtl, name, matchmakerToken, friendCode, lastId);

        var connection = new HazelConnection(eventArgs.Connection, connectionLogger);

        await eventManager.CallAsync(new ClientConnectionEvent(connection, eventArgs.HandshakeData));

        // Register client
        await clientManager.RegisterConnectionAsync(connection, name, clientVersion, language, chatMode,
            platformSpecificData);
    }

    public ListenerConfig? GetAvailableListener()
    {
        return Listeners.FirstOrDefault()?.Config;
    }

    public record ListenerInfo(
        ListenerConfig Config,
        NetworkConnectionListener? Listener,
        DtlsConnectionListener? AuthListener);
}
