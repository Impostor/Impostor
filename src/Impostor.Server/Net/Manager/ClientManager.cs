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

internal partial class ClientManager(
    ILogger<ClientManager> logger,
    IEventManager eventManager,
    IClientFactory clientFactory,
    ICompatibilityManager compatibilityManager,
    IOptions<CompatibilityConfig> compatibilityConfig)
{
    private readonly ConcurrentDictionary<int, ClientBase> _clients = new();
    private readonly CompatibilityConfig _compatibilityConfig = compatibilityConfig.Value;
    private int _idLast;

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

    public async ValueTask RegisterConnectionAsync(ConnectionData connectionData)
    {
        var clientVersion = connectionData.Version!.Value;
        var platformData = new PlatformSpecificData(
            connectionData.Platforms,
            connectionData.PlatformName,
            connectionData.Platforms == Platforms.Xbox ? connectionData.PlatformId : null,
            connectionData.Platforms == Platforms.Playstation ? connectionData.PlatformId : null
        );
        var versionCompare = compatibilityManager.CanConnectToServer(clientVersion);
        if (
            versionCompare == ICompatibilityManager.VersionCompareResult.ServerTooOld
            &&
            _compatibilityConfig.AllowFutureGameVersions
        )
        {
            logger.LogWarning(
                "Client connected using future version: {clientVersion} ({version}). Unsupported, continue at your own risk.",
                clientVersion.Value, clientVersion.ToString());
        }
        else if (versionCompare != ICompatibilityManager.VersionCompareResult.Compatible)
        {
            logger.LogInformation(
                "Client connected using unsupported version: {clientVersion} ({version})",
                clientVersion.Value, clientVersion.ToString());

            using var packet = MessageWriter.Get(MessageType.Reliable);

            var message = versionCompare switch
            {
                ICompatibilityManager.VersionCompareResult.ClientTooOld => DisconnectMessages.VersionClientTooOld,
                ICompatibilityManager.VersionCompareResult.ServerTooOld => DisconnectMessages.VersionServerTooOld,
                ICompatibilityManager.VersionCompareResult.Unknown => DisconnectMessages.VersionUnsupported,
                _ => throw new ArgumentOutOfRangeException(),
            };

            await connectionData._connection.CustomDisconnectAsync(DisconnectReason.Custom, message);
            return;
        }

        if (connectionData.Name.Length > 10)
        {
            await connectionData._connection.CustomDisconnectAsync(DisconnectReason.Custom,
                DisconnectMessages.UsernameLength);
            return;
        }

        if (string.IsNullOrWhiteSpace(connectionData.Name))
        {
            await connectionData._connection.CustomDisconnectAsync(
                DisconnectReason.Custom,
                DisconnectMessages.UsernameIllegalCharacters);
            return;
        }

        var client = clientFactory.Create(
            connectionData._connection,
            connectionData.Name,
            connectionData.Version.Value,
            connectionData.Language,
            connectionData.ChatMode,
            platformData,
            connectionData);
        var id = NextId();

        client.Id = id;
        logger.LogTrace("Client connected.");
        _clients.TryAdd(id, client);

        await eventManager.CallAsync(new ClientConnectedEvent(connectionData._connection, client));
    }

    public void Remove(IClient client)
    {
        logger.LogTrace("Client disconnected.");
        _clients.TryRemove(client.Id, out _);
    }

    public bool Validate(IClient client)
    {
        return client.Id != 0
               && _clients.TryGetValue(client.Id, out var registeredClient)
               && ReferenceEquals(client, registeredClient);
    }
}
