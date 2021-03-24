using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api.Events.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Messages;
using Impostor.Api.Net.Messages.Announcements;
using Impostor.Hazel;
using Impostor.Hazel.Udp;
using Impostor.Server.Config;
using Impostor.Server.Events.Announcements;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Net
{
    internal class AnnouncementsService : IHostedService
    {
        private readonly ILogger<AnnouncementsService> _logger;
        private readonly AnnouncementsServerConfig _config;
        private readonly ObjectPool<MessageReader> _readerPool;
        private readonly IEventManager _eventManager;
        private UdpConnectionListener? _connection;

        public AnnouncementsService(ILogger<AnnouncementsService> logger, IOptions<AnnouncementsServerConfig> config, ObjectPool<MessageReader> readerPool, IEventManager eventManager)
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

            _connection = new UdpConnectionListener(endpoint, _readerPool, mode)
            {
                NewConnection = OnNewConnection,
            };

            await _connection.StartAsync();

            _logger.LogInformation("Announcements server is listening on {Address}:{Port}", endpoint.Address, endpoint.Port);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Announcements server is shutting down!");

            if (_connection != null)
            {
                await _connection.DisposeAsync();
            }
        }

        private async ValueTask OnNewConnection(NewConnectionEventArgs e)
        {
            MessageHello.Deserialize(e.HandshakeData, out var announcementVersion, out var id, out var language);

            _logger.LogDebug("Client requested announcement (version: {Version}, id: {Id}, language: {Language})", announcementVersion, id, language);

            if (announcementVersion != 2)
            {
                await e.Connection.Disconnect("Unsupported announcement version");
                return;
            }

            var @event = new AnnouncementRequestEvent(id, language);
            await _eventManager.CallAsync(@event);

            var response = @event.Response;

            if (response.UseCached)
            {
                using var writer = MessageWriter.Get(MessageType.Reliable);
                Message00UseCache.Serialize(writer);
                await e.Connection.SendAsync(writer);

                _logger.LogDebug("Sent UseCache response");
            }

            if (response.Announcement != null)
            {
                using var writer = MessageWriter.Get(MessageType.Reliable);
                var announcement = response.Announcement.Value;
                Message01Update.Serialize(writer, announcement.Id, announcement.Message);
                await e.Connection.SendAsync(writer);

                _logger.LogDebug("Sent ({Id}) {Message}", announcement.Id, announcement.Message);
            }

            if (response.FreeWeekendState != FreeWeekendState.NotFree)
            {
                using var writer = MessageWriter.Get(MessageType.Reliable);
                Message02SetFreeWeekend.Serialize(writer, response.FreeWeekendState);
                await e.Connection.SendAsync(writer);

                _logger.LogDebug("Sent {FreeWeekendState} weekend state", response.FreeWeekendState);
            }

            await e.Connection.Disconnect(null);
        }
    }
}
