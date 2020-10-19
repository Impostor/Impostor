using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Impostor.Server.Events.Managers;
using Impostor.Server.Games;
using Impostor.Server.Games.Managers;
using Impostor.Server.Hazel;
using Impostor.Server.Hazel.Messages;
using Impostor.Server.Net.Manager;
using Impostor.Server.Net.Messages;
using Impostor.Server.Net.Redirector;
using Impostor.Shared.Innersloth;
using Impostor.Shared.Innersloth.Data;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Impostor.Server.Net.State
{
    internal partial class Game
    {
        private static readonly ILogger Logger = Log.ForContext<Game>();

        private readonly IServiceProvider _serviceProvider;
        private readonly GameManager _gameManager;
        private readonly ClientManager _clientManager;
        private readonly Matchmaker _matchmaker;
        private readonly ConcurrentDictionary<int, ClientPlayer> _players;
        private readonly HashSet<IPAddress> _bannedIps;
        private readonly IEventManager _eventManager;

        public Game(
            IServiceProvider serviceProvider,
            GameManager gameManager,
            INodeLocator nodeLocator,
            IPEndPoint publicIp,
            GameCode code,
            GameOptionsData options,
            Matchmaker matchmaker,
            ClientManager clientManager,
            IEventManager eventManager)
        {
            _serviceProvider = serviceProvider;
            _gameManager = gameManager;
            _players = new ConcurrentDictionary<int, ClientPlayer>();
            _bannedIps = new HashSet<IPAddress>();

            PublicIp = publicIp;
            Code = code;
            HostId = -1;
            GameState = GameStates.NotStarted;
            Options = options;
            _matchmaker = matchmaker;
            _clientManager = clientManager;
            _eventManager = eventManager;
            Items = new ConcurrentDictionary<object, object>();
        }

        public IPEndPoint PublicIp { get; }

        public GameCode Code { get; }

        public bool IsPublic { get; private set; }

        public int HostId { get; private set; }

        public GameStates GameState { get; private set; }

        public GameOptionsData Options { get; }

        public IDictionary<object, object> Items { get; }

        public int PlayerCount => _players.Count;

        public ClientPlayer Host => _players[HostId];

        public IEnumerable<IClientPlayer> Players => _players.Select(p => p.Value);

        public IGameMessageWriter CreateMessage(MessageType type)
        {
            return new HazelGameMessageWriter(type, this);
        }

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

        public ValueTask EndAsync()
        {
            return _gameManager.RemoveAsync(Code);
        }

        private ValueTask BroadcastJoinMessage(IGameMessageWriter message, bool clear, ClientPlayer player)
        {
            Message01JoinGame.SerializeJoin(message, clear, Code, player.Client.Id, HostId);

            return message.SendToAllExceptAsync(player.Client.Id);
        }
    }
}