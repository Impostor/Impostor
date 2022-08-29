using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api.Config;
using Impostor.Api.Events.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages;
using Impostor.Hazel;
using Impostor.Server.Events.Client;
using Impostor.Server.Net.Factories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Net.Manager
{
    internal partial class ClientManager
    {
        // NOTE: when updating this array, keep the versions ordered from old to new, otherwise the version compare logic doesn't work properly
        private static readonly int[] SupportedVersions =
        {
            GameVersion.GetVersion(2021, 11, 9), // 2021.11.9
            GameVersion.GetVersion(2021, 12, 14), // 2021.12.14
            GameVersion.GetVersion(2022, 1, 10), // 2021.2.23
            GameVersion.GetVersion(2022, 2, 2), // 2022.3.29 and 2022.4.19
            GameVersion.GetVersion(2022, 4, 20), // 2022.6.21 and 2022.7.12
            GameVersion.GetVersion(2022, 5, 12), // 2022.8.23
        };

        private readonly ILogger<ClientManager> _logger;
        private readonly IEventManager _eventManager;
        private readonly ConcurrentDictionary<int, ClientBase> _clients;
        private readonly CompatibilityConfig _compatibilityConfig;
        private readonly IClientFactory _clientFactory;
        private int _idLast;

        public ClientManager(ILogger<ClientManager> logger, IEventManager eventManager, IClientFactory clientFactory, IOptions<CompatibilityConfig> compatibilityConfig)
        {
            _logger = logger;
            _eventManager = eventManager;
            _clientFactory = clientFactory;
            _clients = new ConcurrentDictionary<int, ClientBase>();
            _compatibilityConfig = compatibilityConfig.Value;
        }

        private enum VersionCompareResult
        {
            Compatible,
            ClientTooOld,
            ServerTooOld,
            Unknown,
        }

        public IEnumerable<ClientBase> Clients => _clients.Values;

        public int NextId()
        {
            var clientId = Interlocked.Increment(ref _idLast);

            if (clientId < 1)
            {
                // Super rare but reset the _idLast because of overflow.
                _idLast = 0;

                // And get a new id.
                clientId = Interlocked.Increment(ref _idLast);
            }

            return clientId;
        }

        public async ValueTask RegisterConnectionAsync(IHazelConnection connection, string name, int clientVersion, Language language, QuickChatModes chatMode, PlatformSpecificData? platformSpecificData)
        {
            var versionCompare = CompareVersion(clientVersion);
            if (versionCompare == VersionCompareResult.ServerTooOld && _compatibilityConfig.AllowFutureGameVersions && platformSpecificData != null)
            {
                GameVersion.ParseVersion(clientVersion, out var year, out var month, out var day, out var revision);
                _logger.LogWarning("Client connected using future version: {clientVersion} ({version}). Unsupported, continue at your own risk.", clientVersion, $"{year}.{month}.{day}{(revision == 0 ? string.Empty : "." + revision)}");
            }
            else if (versionCompare != VersionCompareResult.Compatible || platformSpecificData == null)
            {
                GameVersion.ParseVersion(clientVersion, out var year, out var month, out var day, out var revision);
                _logger.LogTrace("Client connected using unsupported version: {clientVersion} ({version})", clientVersion, $"{year}.{month}.{day}{(revision == 0 ? string.Empty : "." + revision)}");

                using var packet = MessageWriter.Get(MessageType.Reliable);

                var message = versionCompare switch
                {
                    VersionCompareResult.ClientTooOld => DisconnectMessages.VersionClientTooOld,
                    VersionCompareResult.ServerTooOld => DisconnectMessages.VersionServerTooOld,
                    VersionCompareResult.Unknown => DisconnectMessages.VersionUnsupported,
                    _ => throw new ArgumentOutOfRangeException(),
                };

                await connection.CustomDisconnectAsync(DisconnectReason.Custom, message);
                return;
            }

            if (name.Length > 10)
            {
                await connection.CustomDisconnectAsync(DisconnectReason.Custom, DisconnectMessages.UsernameLength);
                return;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                await connection.CustomDisconnectAsync(DisconnectReason.Custom, DisconnectMessages.UsernameIllegalCharacters);
                return;
            }

            var client = _clientFactory.Create(connection, name, clientVersion, language, chatMode, platformSpecificData);
            var id = NextId();

            client.Id = id;
            _logger.LogTrace("Client connected.");
            _clients.TryAdd(id, client);

            await _eventManager.CallAsync(new ClientConnectedEvent(connection, client));
        }

        public void Remove(IClient client)
        {
            _logger.LogTrace("Client disconnected.");
            _clients.TryRemove(client.Id, out _);
        }

        public bool Validate(IClient client)
        {
            return client.Id != 0
                   && _clients.TryGetValue(client.Id, out var registeredClient)
                   && ReferenceEquals(client, registeredClient);
        }

        private VersionCompareResult CompareVersion(int clientVersion)
        {
            foreach (var serverVersion in SupportedVersions)
            {
                if (clientVersion == serverVersion)
                {
                    return VersionCompareResult.Compatible;
                }
            }

            if (clientVersion < SupportedVersions[0])
            {
                return VersionCompareResult.ClientTooOld;
            }

            if (clientVersion > SupportedVersions.Last())
            {
                return VersionCompareResult.ServerTooOld;
            }

            // This may happen in the very rare case that version X is supported, X+2 is as well, but X+1 is not.
            return VersionCompareResult.Unknown;
        }
    }
}
