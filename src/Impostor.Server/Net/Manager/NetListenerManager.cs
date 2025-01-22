using System.Net;
using System.Net.Sockets;
using Impostor.Api.Utils;
using Microsoft.Extensions.ObjectPool;
using Next.Hazel.Udp;

namespace Impostor.Server.Net.Manager;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Api.Config;
using Microsoft.Extensions.Logging;
using Next.Hazel.Dtls;

public class NetListenerManager(ILogger<NetListenerManager> logger, ObjectPool<MessageReader> readerPool)
{
    public List<ListenerInfo> Listeners { get; private set; } = [];

    public Dictionary<string, X509Certificate> CachedCertificates { get; private set; } = new();

    public void Create(ListenerConfig config)
    {
        if (!CheckConfig(config))
        {
            return;
        }

        NetworkConnectionListener listener = config.IsDtl
            ? CreateDtls(config.ListenIp, config.ListenPort + 3, OnConnectionAsync)
            : CreateUdp(config.ListenIp, config.ListenPort, OnConnectionAsync);

        var authListener = config.HasAuth
            ? CreateDtls(config.ListenIp, config.ListenPort + 2, OnAuthConnectionAsync)
            : null;

        Listeners.Add(SetCertificate(config, listener, authListener));
    }

    private ListenerInfo SetCertificate(ListenerConfig config, NetworkConnectionListener? listener,
        DtlsConnectionListener? authListener)
    {
        // TODO: Set certificates
        if (listener is DtlsConnectionListener)
        {
        }

        if (authListener is not null)
        {
        }

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

        // Hazel current does not support dtl and auth
        if (config.HasAuth || config.IsDtl)
        {
            logger.LogWarning("dtl and auth not supported yet");
            return false;
        }

        if (config is { HasAuth: true, IsDtl: true })
        {
            logger.LogWarning("dtl not supported auth");
            return false;
        }

        if ((config.PrivateKeyPath == string.Empty || config.CertificatePath == string.Empty) && (config.HasAuth || config.IsDtl))
        {
            logger.LogWarning("private key or certificate path is empty not supported dtl and auth");
            return false;
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

    public record ListenerInfo(ListenerConfig Config, NetworkConnectionListener? Listener, DtlsConnectionListener? AuthListener);

    private async ValueTask OnAuthConnectionAsync(NewConnectionEventArgs connection)
    {

    }

    private async ValueTask OnConnectionAsync(NewConnectionEventArgs connection)
    {

    }
}
