using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api.Config;
using Impostor.Api.Events.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Manager;
using Impostor.Hazel;
using Impostor.Server.Events.Client;
using Impostor.Server.Net.Factories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Net.Manager;

internal partial class ClientManager
{
    private readonly IClientFactory _clientFactory;
    private readonly ConcurrentDictionary<int, ClientBase> _clients;
    private readonly CompatibilityConfig _compatibilityConfig;
    private readonly ICompatibilityManager _compatibilityManager;
    private readonly IEventManager _eventManager;
    private readonly ILogger<ClientManager> _logger;
    private int _idLast;

    public ClientManager(ILogger<ClientManager> logger, IEventManager eventManager, IClientFactory clientFactory,
        ICompatibilityManager compatibilityManager, IOptions<CompatibilityConfig> compatibilityConfig)
    {
        _logger = logger;
        _eventManager = eventManager;
        _clientFactory = clientFactory;
        _clients = new ConcurrentDictionary<int, ClientBase>();
        _compatibilityManager = compatibilityManager;
        _compatibilityConfig = compatibilityConfig.Value;
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

    public async ValueTask RegisterConnectionAsync(IHazelConnection connection, string name, GameVersion clientVersion,
        Language language, QuickChatModes chatMode, PlatformSpecificData? platformSpecificData)
    {
        var versionCompare = _compatibilityManager.CanConnectToServer(clientVersion);
        if (versionCompare == ICompatibilityManager.VersionCompareResult.ServerTooOld &&
            _compatibilityConfig.AllowFutureGameVersions && platformSpecificData != null)
        {
            _logger.LogWarning(
                "Client connected using future version: {clientVersion} ({version}). Unsupported, continue at your own risk.",
                clientVersion.Value, clientVersion.ToString());
        }
        else if (versionCompare != ICompatibilityManager.VersionCompareResult.Compatible ||
                 platformSpecificData == null)
        {
            _logger.LogInformation("Client connected using unsupported version: {clientVersion} ({version})",
                clientVersion.Value, clientVersion.ToString());

            using var packet = MessageWriter.Get(MessageType.Reliable);

            var message = versionCompare switch
            {
                ICompatibilityManager.VersionCompareResult.ClientTooOld => DisconnectMessages.VersionClientTooOld,
                ICompatibilityManager.VersionCompareResult.ServerTooOld => DisconnectMessages.VersionServerTooOld,
                ICompatibilityManager.VersionCompareResult.Unknown => DisconnectMessages.VersionUnsupported,
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
            await connection.CustomDisconnectAsync(DisconnectReason.Custom,
                DisconnectMessages.UsernameIllegalCharacters);
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
}
