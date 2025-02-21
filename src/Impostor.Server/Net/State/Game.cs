using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Impostor.Api.Config;
using Impostor.Api.Events.Managers;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.Customization;
using Impostor.Api.Innersloth.GameOptions;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Manager;
using Impostor.Api.Net.Messages.S2C;
using Impostor.Server.Events;
using Impostor.Server.Net.Manager;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Net.State;

internal partial class Game(
    ILogger<Game> logger,
    IServiceProvider serviceProvider,
    GameManager gameManager,
    GameCode code,
    IGameOptions options,
    GameFilterOptions filterOptions,
    ClientManager clientManager,
    IEventManager eventManager,
    ICompatibilityManager compatibilityManager,
    IOptions<CompatibilityConfig> compatibilityConfig,
    IOptions<TimeoutConfig> timeoutConfig
    )
{
    private readonly HashSet<IPAddress> _bannedIps = new();
    private readonly CompatibilityConfig _compatibilityConfig = compatibilityConfig.Value;
    private readonly ConcurrentDictionary<int, ClientPlayer> _players = new();
    private readonly TimeoutConfig _timeoutConfig = timeoutConfig.Value;

    public ClientPlayer? Host
    {
        get => _players.GetValueOrDefault(HostId);
    }

    internal GameNet GameNet { get; } = new();

    public GameCode Code { get; } = code;

    public bool IsPublic { get; private set; }

    public string? DisplayName { get; set; }

    public int HostId { get; private set; } = -1;

    public GameStates GameState { get; private set; } = GameStates.NotStarted;

    public IGameOptions Options { get; } = options;

    public GameFilterOptions FilterOptions { get; } = filterOptions;

    public IDictionary<object, object> Items { get; } = new ConcurrentDictionary<object, object>();

    public int PlayerCount
    {
        get => _players.Count;
    }

    public IEnumerable<IClientPlayer> Players
    {
        get => _players.Select(p => p.Value);
    }

    public bool IsHostAuthoritive
    {
        get => Host != null && Host.Client.GameVersion.HasDisableServerAuthorityFlag;
    }

    public IClientPlayer? GetClientPlayer(int clientId)
    {
        return _players.TryGetValue(clientId, out var clientPlayer) ? clientPlayer : null;
    }

    public bool TryGetPlayer(int id, [MaybeNullWhen(false)] out ClientPlayer player)
    {
        if (_players.TryGetValue(id, out var result))
        {
            player = result;
            return true;
        }

        player = null;
        return false;
    }

    internal async ValueTask StartedAsync()
    {
        if (GameState == GameStates.Starting)
        {
            foreach (var player in _players.Values)
            {
                if (GameNet.ShipStatus != null)
                {
                    await player.Character!.NetworkTransform.SetPositionAsync(player,
                        GameNet.ShipStatus.GetSpawnLocation(player.Character, PlayerCount, true));
                }
            }

            GameState = GameStates.Started;

            await eventManager.CallAsync(new GameStartedEvent(this));
        }
    }

    /// <summary>Check if there are players using a color.</summary>
    /// <param name="color">The color to check for.</param>
    /// <param name="exceptBy">Exempt a player from being checked.</param>
    /// <returns>True if there is player other than exceptBy that uses that color.</returns>
    internal bool IsColorUsed(ColorType color, IInnerPlayerControl? exceptBy = null)
    {
        return Players.Any(p =>
            p.Character != null && p.Character != exceptBy && p.Character.PlayerInfo.CurrentOutfit.Color == color);
    }

    private ValueTask BroadcastJoinMessage(IMessageWriter message, bool clear, ClientPlayer player)
    {
        Message01JoinGameS2C.SerializeJoin(message, clear, Code, player, HostId);

        return SendToAllExceptAsync(message, player.Client.Id);
    }

    private IEnumerable<IHazelConnection> GetConnections(Func<IClientPlayer, bool> filter)
    {
        return Players
            .Where(filter)
            .Select(p => p.Client.Connection)
            .Where(c => c != null && c.IsConnected)!;
    }
}
