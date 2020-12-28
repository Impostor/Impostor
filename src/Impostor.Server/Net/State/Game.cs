using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public int HostId { get; private set; }

        public GameStates GameState { get; private set; }

        internal GameNet GameNet { get; }

        public GameOptionsData Options { get; }

        public IDictionary<object, object> Items { get; }

        public int PlayerCount => _players.Count;

        public ClientPlayer Host => _players[HostId];

        public IEnumerable<IClientPlayer> Players => _players.Select(p => p.Value);

        public bool TryGetPlayer(int id, out ClientPlayer player)
        {
            if (_players.TryGetValue(id, out var result))
            {
                player = result;
                return true;
            }

            player = default;
            return false;
        }

        public IClientPlayer GetClientPlayer(int clientId)
        {
            return _players.TryGetValue(clientId, out var clientPlayer) ? clientPlayer : null;
        }

        internal async ValueTask StartedAsync()
        {
            if (GameState == GameStates.Starting)
            {
                for (var i = 0; i < _players.Values.Count; i++)
                {
                    var player = _players.Values.ElementAt(i);
                    await player.Character!.NetworkTransform.SetPositionAsync(player, MapSpawn.Maps[Options.Map].GetSpawnLocation(i, PlayerCount, true));
                }

                GameState = GameStates.Started;

                await _eventManager.CallAsync(new GameStartedEvent(this));
            }
        }

        public ValueTask EndAsync()
        {
            return _gameManager.RemoveAsync(Code);
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
                .Select(p => p.Client.Connection);
        }
    }
}
