using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Hazel;
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
        
        private readonly GameManager _gameManager;
        private readonly INodeLocator _nodeLocator;
        private readonly ConcurrentDictionary<int, ClientPlayer> _players;
        private readonly HashSet<IPAddress> _bannedIps;

        public Game(GameManager gameManager, INodeLocator nodeLocator, IPEndPoint publicIp, int code, GameOptionsData options)
        {
            _gameManager = gameManager;
            _nodeLocator = nodeLocator;
            _players = new ConcurrentDictionary<int, ClientPlayer>();
            _bannedIps = new HashSet<IPAddress>();

            PublicIp = publicIp;
            Code = code;
            CodeStr = GameCode.IntToGameName(code);
            HostId = -1;
            GameState = GameStates.NotStarted;
            Options = options;
        }

        public IPEndPoint PublicIp { get; }
        public int Code { get; }
        public string CodeStr { get; }
        public bool IsPublic { get; private set; }
        public int HostId { get; private set; }
        public GameStates GameState { get; private set; }
        public GameOptionsData Options { get; }
        
        public int PlayerCount => _players.Count;
        public ClientPlayer Host => _players[HostId];
        
        /// <summary>
        ///     Send a message to all players except one.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="senderId">
        ///     The player to exclude from sending the message.
        ///     Set to null to send a message to everyone.
        /// </param>
        public void SendToAllExcept(MessageWriter message, int? senderId)
        {
            foreach (var (_, player) in _players.Where(x => 
                x.Value.Limbo == LimboStates.NotLimbo && 
                x.Value.Client.Id != senderId))
            {
                if (player.Client.Connection.State != ConnectionState.Connected)
                {
                    Logger.Warning("[{0}] Tried to send data to a disconnected player ({1}).", senderId, player.Client.Id);
                    continue;
                }
                
                player.Client.Send(message);
            }
        }

        /// <summary>
        ///     Send a message to a specific player.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="playerId"></param>
        public void SendTo(MessageWriter message, int playerId)
        {
            if (_players.TryGetValue(playerId, out var player))
            {
                if (player.Client.Connection.State != ConnectionState.Connected)
                {
                    Logger.Warning("[{0}] Sending data to {1} failed, player is not connected.", CodeStr, player.Client.Id);
                    return;
                }
                
                player.Client.Send(message);
            }
            else
            {
                Logger.Warning("[{0}] Sending data to {1} failed, player does not exist.", CodeStr, playerId);
            }
        }
        
        private void BroadcastJoinMessage(MessageWriter message, bool clear, ClientPlayer player)
        {
            Message01JoinGame.SerializeJoin(message, clear, Code, player.Client.Id, HostId);
            
            SendToAllExcept(message, player.Client.Id);
        }
    }
}