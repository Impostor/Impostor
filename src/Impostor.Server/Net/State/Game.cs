using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
        
        private readonly GameManager _gameManager;
        private readonly INodeLocator _nodeLocator;
        private readonly IMatchmaker matchmaker;
        private readonly ConcurrentDictionary<int, ClientPlayer> _players;
        private readonly HashSet<IPAddress> _bannedIps;

        public Game(
            GameManager gameManager,
            INodeLocator nodeLocator,
            IPEndPoint publicIp,
            GameCode code,
            GameOptionsData options,
            IMatchmaker matchmaker)
        {
            _gameManager = gameManager;
            _nodeLocator = nodeLocator;
            _players = new ConcurrentDictionary<int, ClientPlayer>();
            _bannedIps = new HashSet<IPAddress>();

            PublicIp = publicIp;
            Code = code;
            HostId = -1;
            GameState = GameStates.NotStarted;
            Options = options;
            this.matchmaker = matchmaker;
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
        
        private ValueTask BroadcastJoinMessage(IGameMessageWriter message, bool clear, ClientPlayer player)
        {
            Message01JoinGame.SerializeJoin(message, clear, Code, player.Client.Id, HostId);
            
            return message.SendToAllExceptAsync(LimboStates.NotLimbo, player.Client.Id);
        }

        public IEnumerable<IClientPlayer> Players => _players.Select(p => p.Value);

        public IGameMessageWriter CreateMessage(MessageType type)
        {
            return matchmaker.CreateGameMessageWriter(this, type);
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
    }
}