using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Hazel;
using Impostor.Api.Events.Managers;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.Data;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages;
using Impostor.Server.Hazel;
using Impostor.Server.Net.Hazel;
using Impostor.Server.Net.Manager;
using Impostor.Server.Net.Messages;
using Impostor.Server.Net.Redirector;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Impostor.Server.Net.State
{
    internal partial class Game : IGame
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

        private ValueTask BroadcastJoinMessage(IMessageWriter message, bool clear, ClientPlayer player)
        {
            Message01JoinGame.SerializeJoin(message, clear, Code, player.Client.Id, HostId);

            return SendToAllExceptAsync(message, player.Client.Id);
        }

        private IEnumerable<Connection> GetConnections(Func<IClientPlayer, bool> filter)
        {
            return Players
                .Where(filter)
                .Select(p => p.Client.Connection)
                .OfType<HazelConnection>()
                .Select(c => c.InnerConnection);
        }
    }
}