using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api.Events.Managers;
using Impostor.Api.Net.Messages;
using Impostor.Api.Net.Messages.Auth;
using Impostor.Hazel;
using Impostor.Hazel.Dtls;
using Impostor.Server.Config;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Net
{
    internal class AuthService : IHostedService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly AuthServerConfig _config;
        private readonly ObjectPool<MessageReader> _readerPool;
        private readonly IEventManager _eventManager;
        private DtlsConnectionListener? _connection;

        public AuthService(ILogger<AuthService> logger, IOptions<AuthServerConfig> config, ObjectPool<MessageReader> readerPool, IEventManager eventManager)
        {
            _logger = logger;
            _config = config.Value;
            _readerPool = readerPool;
            _eventManager = eventManager;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var endpoint = new IPEndPoint(IPAddress.Parse(_config.ResolveListenIp()), _config.ListenPort);

            var mode = endpoint.AddressFamily switch
            {
                AddressFamily.InterNetwork => IPMode.IPv4,
                AddressFamily.InterNetworkV6 => IPMode.IPv6,
                _ => throw new InvalidOperationException(),
            };

            var rsa = RSA.Create();
            rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(string.Join(string.Empty, (await File.ReadAllLinesAsync(_config.PrivateKey, cancellationToken)).Where(x => !x.StartsWith("-----")))), out _);
            var cert = new X509Certificate2(_config.Certificate).CopyWithPrivateKey(rsa);

            _connection = new DtlsConnectionListener(endpoint, _readerPool, mode);
            _connection.SetCertificate(cert);
            _connection.NewConnection = ConnectionOnNewConnection;

            await _connection.StartAsync();

            _logger.LogInformation("Auth server is listening on {Address}:{Port}", endpoint.Address, endpoint.Port);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Auth server is shutting down!");

            if (_connection != null)
            {
                await _connection.DisposeAsync();
            }
        }

        private ValueTask ConnectionOnNewConnection(NewConnectionEventArgs e)
        {
            MessageHandshake.Deserialize(e.HandshakeData, out var clientVersion, out var platform, out var clientId);

            _logger.LogTrace("New authentication request: {clientVersion}, {platform}, {clientId}", clientVersion, platform, clientId);

            using var writer = MessageWriter.Get(MessageType.Reliable);

            writer.StartMessage(1);
            Message01Complete.Serialize(writer, (uint)RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue));
            writer.EndMessage();

            return e.Connection.SendAsync(writer);
        }
    }
}
