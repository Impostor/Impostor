using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AmongUs.Server.Exceptions;
using AmongUs.Server.Extensions;
using AmongUs.Server.Net.Response;
using AmongUs.Shared.Innersloth;
using AmongUs.Shared.Innersloth.Data;
using Hazel;
using Serilog;
using ILogger = Serilog.ILogger;

namespace AmongUs.Server.Net
{
    public class Game
    {
        private static readonly ILogger Logger = Log.ForContext<Game>();
        
        private readonly ConcurrentDictionary<int, ClientPlayer> _players;
        private int _hostId;
        
        public Game(int code, GameOptionsData options)
        {
            Code = code;
            CodeStr = GameCode.IntToGameName(code);
            GameState = GameStates.NotStarted;
            Options = options;

            _hostId = -1;
            _players = new ConcurrentDictionary<int, ClientPlayer>();
        }
        
        public int Code { get; }
        public string CodeStr { get; }
        public GameStates GameState { get; }
        public GameOptionsData Options { get; }

        public void SendToAllExcept(MessageWriter message, ClientPlayer sender)
        {
            foreach (var (_, player) in _players.Where(x => x.Value != sender))
            {
                player.Client.Connection.Send(message);
            }
        }

        public void SendTo(MessageWriter message, int playerId)
        {
            if (_players.TryGetValue(playerId, out var player))
            {
                player.Client.Connection.Send(message);
            }
            else
            {
                Logger.Warning("[{0}] Sending data to {1} failed, player does not exist.", CodeStr, playerId);
            }
        }

        public void HandleJoinGame(ClientPlayer player)
        {
            switch (GameState)
            {
                case GameStates.NotStarted:
                    HandleJoinGameNew(player);
                    break;
                case GameStates.Started:
                    HandleJoinGameNext(player);
                    break;
                case GameStates.Ended:
                case GameStates.Destroyed:
                    player.Client.Connection.Send(new Message1DisconnectReason(DisconnectReason.GameStarted));
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleJoinGameNew(ClientPlayer player)
        {
            Logger.Verbose("[{0}] Player joined.", CodeStr);
            
            // Store player.
            if (!_players.TryAdd(player.Client.Id, player))
            {
                throw new AmongUsException("Failed to add player to game.");
            }
            
            // Assign player to this game for future packets.
            player.Game = this;

            // Assign hostId if none is set.
            if (_hostId == -1)
            {
                _hostId = player.Client.Id;
            }

            if (_hostId == player.Client.Id)
            {
                player.LimboState = LimboStates.NotLimbo;
            }

            using (var message = MessageWriter.Get(SendOption.Reliable))
            {
                // TODO: WriteJoinedMessage - Move to own method / class
                message.StartMessage(7);
                message.Write(Code);
                message.Write(player.Client.Id);
                message.Write(_hostId);
                message.WritePacked(_players.Count - 1);
            
                foreach (var (_, p) in _players.Where(x => x.Value == player))
                {
                    message.WritePacked(p.Client.Id);
                }
            
                message.EndMessage();

                message.StartMessage(10);
                message.Write(Code);
                message.Write((sbyte)1);
                message.Write(false); // Private / Public
                message.EndMessage();
                
                player.Client.Connection.Send(message);
            
                // TODO: BroadcastJoinMessage - Move to own method / class
                message.Clear(SendOption.Reliable);
                message.StartMessage(1);
                message.Write(Code);
                message.Write(player.Client.Id);
                message.Write(_hostId);
                message.EndMessage();
                
                SendToAllExcept(message, player);
            }
        }

        private void HandleJoinGameNext(ClientPlayer player)
        {
            throw new NotImplementedException();
        }
    }
}