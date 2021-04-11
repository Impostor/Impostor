using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using Impostor.Api.Events.Managers;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages;
using Impostor.Api.Net.Messages.S2C;
using Impostor.Server.Events;
using Impostor.Server.Net.Manager;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.State
{
    internal partial class Game
    {
        private readonly ILogger<Game> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly GameManager _gameManager;
        private readonly ClientManager _clientManager;
        private readonly ConcurrentDictionary<int, ClientPlayer> _players;
        private readonly HashSet<IPAddress> _bannedIps;
        private readonly IEventManager _eventManager;

        public Game(
            ILogger<Game> logger,
            IServiceProvider serviceProvider,
            GameManager gameManager,
            IPEndPoint publicIp,
            GameCode code,
            GameOptionsData options,
            ClientManager clientManager,
            IEventManager eventManager)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _gameManager = gameManager;
            _players = new ConcurrentDictionary<int, ClientPlayer>();
            _bannedIps = new HashSet<IPAddress>();

            PublicIp = publicIp;
            Code = code;
            HostId = -1;
            GameState = GameStates.NotStarted;
            GameNet = new GameNet();
            Options = options;
            _clientManager = clientManager;
            _eventManager = eventManager;
            Items = new ConcurrentDictionary<object, object>();
        }

        public IPEndPoint PublicIp { get; }

        public GameCode Code { get; }

        public bool IsPublic { get; private set; }

        public string? DisplayName { get; set; }

        public int HostId { get; private set; }

        public GameStates GameState { get; private set; }

        public GameOptionsData Options { get; }

        public IDictionary<object, object> Items { get; }

        public int PlayerCount => _players.Count;

        public ClientPlayer? Host => _players.GetValueOrDefault(HostId);

        public IEnumerable<IClientPlayer> Players => _players.Select(p => p.Value);

        internal GameNet GameNet { get; }

        public bool TryGetPlayer(int id, [MaybeNullWhen(false)] out ClientPlayer player)
        {
            if (_players.TryGetValue(id, out var result))
            {
                player = result;
                return true;
            }

            player = default;
            return false;
        }

        public IClientPlayer? GetClientPlayer(int clientId)
        {
            return _players.TryGetValue(clientId, out var clientPlayer) ? clientPlayer : null;
        }

        public ValueTask EndAsync()
        {
            return _gameManager.RemoveAsync(Code);
        }

        internal async ValueTask StartedAsync()
        {
            if (GameState == GameStates.Starting)
            {
                foreach (var player in _players.Values)
                {
                    player.Character?.NetworkTransform.OnPlayerSpawn();
                    await player.Character!.NetworkTransform.SetPositionAsync(player, GameNet.ShipStatus!.GetSpawnLocation(player.Character, PlayerCount, true), Vector2.Zero);
                }

                GameState = GameStates.Started;

                await _eventManager.CallAsync(new GameStartedEvent(this));
            }
        }

        private ValueTask BroadcastJoinMessage(IMessageWriter message, bool clear, ClientPlayer player)
        {
            Message01JoinGameS2C.SerializeJoin(message, clear, Code, player.Client.Id, HostId);

            return SendToAllExceptAsync(message, player.Client.Id);
        }

        private IEnumerable<IHazelConnection> GetConnections(Func<IClientPlayer, bool> filter)
        {
            return Players
                .Where(filter)
                .Select(p => p.Client.Connection)!;
        }
    }
}
