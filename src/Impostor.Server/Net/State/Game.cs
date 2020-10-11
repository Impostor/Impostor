using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Impostor.Server.Games;
using Impostor.Server.Games.Managers;
using Impostor.Server.Net.Manager;
using Impostor.Server.Net.Messages;
using Impostor.Server.Net.Redirector;
using Impostor.Shared.Innersloth;
using Impostor.Shared.Innersloth.Data;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Impostor.Server.Net.State
{
    internal partial class Game : IGame
    {
        private static readonly ILogger Logger = Log.ForContext<Game>();

        private readonly IGameManager _gameManager;
        private readonly IClientManager _clientManager;
        private readonly IMatchmaker _matchmaker;
        private readonly ConcurrentDictionary<int, IClientPlayer> _players;
        private readonly HashSet<IPAddress> _bannedIps;

        public Game(
            IGameManager gameManager,
            INodeLocator nodeLocator,
            IPEndPoint publicIp,
            GameCode code,
            GameOptionsData options,
            IMatchmaker matchmaker,
            IClientManager clientManager)
        {
            _gameManager = gameManager;
            _players = new ConcurrentDictionary<int, IClientPlayer>();
            _bannedIps = new HashSet<IPAddress>();

            PublicIp = publicIp;
            Code = code;
            HostId = -1;
            GameState = GameStates.NotStarted;
            Options = options;
            _matchmaker = matchmaker;
            _clientManager = clientManager;
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

        public IClientPlayer Host => _players[HostId];

        public IEnumerable<IClientPlayer> Players => _players.Select(p => p.Value);

        public IGameMessageWriter CreateMessage(MessageType type)
        {
            return _matchmaker.CreateGameMessageWriter(this, type);
        }

        public bool TryGetPlayer(int id, out IClientPlayer player)
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

        private ValueTask BroadcastJoinMessage(IGameMessageWriter message, bool clear, IClientPlayer player)
        {
            Message01JoinGame.SerializeJoin(message, clear, Code, player.Client.Id, HostId);

            return message.SendToAllExceptAsync(player.Client.Id);
        }
    }
}