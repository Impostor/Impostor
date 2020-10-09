using System;
using System.Threading.Tasks;
using Impostor.Server.Net.Manager;
using Impostor.Server.Net.Messages;
using Impostor.Server.Net.State;
using Impostor.Shared.Innersloth.Data;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Impostor.Server.Net
{
    internal class Client : ClientBase
    {
        private static readonly ILogger Logger = Log.ForContext<Client>();
        
        private readonly IClientManager _clientManager;
        private readonly GameManager _gameManager;

        public Client(IClientManager clientManager, GameManager gameManager, int id, string name, IConnection connection)
            : base(id, name, connection)
        {
            _clientManager = clientManager;
            _gameManager = gameManager;
            Player = new ClientPlayer(this, _gameManager);
        }

        public ClientPlayer Player { get; }
        
        private bool IsPacketAllowed(IMessageReader message, bool hostOnly)
        {
            var game = Player.Game;
            if (game == null)
            {
                return false;
            }

            // GameCode must match code of the current game assigned to the player.
            if (message.ReadInt32() != game.Code)
            {
                return false;
            }
            
            // Some packets should only be sent by the host of the game.
            if (hostOnly)
            {
                if (game.HostId == Id)
                {
                    return true;
                }
                
                Logger.Warning("[{0}] Client sent packet only allowed by the host ({1}).", Id, game.HostId);
                return false;
            }

            return true;
        }

        
        protected override async ValueTask OnMessageReceived(IMessage message)
        {
            var reader = message.CreateReader();
            
            var flag = reader.Tag;
            
            Logger.Verbose("[{0}] Server got {1}.", Id, flag);
            
            switch (flag)
            {
                case MessageFlags.HostGame:
                {
                    // Read game settings.
                    var gameInfo = Message00HostGame.Deserialize(reader);
                    
                    // Create game.
                    var game = _gameManager.Create(gameInfo);
                    if (game == null)
                    {
                        await Player.SendDisconnectReason(DisconnectReason.ServerFull);
                        return;
                    }

                    // Code in the packet below will be used in JoinGame.
                    using (var writer = Connection.CreateMessage(MessageType.Reliable))
                    {
                        Message00HostGame.Serialize(writer, game.Code);
                
                        await writer.SendAsync();
                    }
                    break;
                }
                
                case MessageFlags.JoinGame:
                {
                    Message01JoinGame.Deserialize(reader, 
                        out var gameCode, 
                        out var unknown);
                    
                    var game = _gameManager.Find(gameCode);
                    if (game == null)
                    {
                        await Player.SendDisconnectReason(DisconnectReason.GameMissing);
                        return;
                    }

                    await game.HandleJoinGame(Player);
                    break;
                }

                case MessageFlags.StartGame:
                {
                    if (!IsPacketAllowed(reader, true))
                    {
                        return;
                    }

                    await Player.Game.HandleStartGame(reader);
                    break;
                }
                
                // No idea how this flag is triggered.
                case MessageFlags.RemoveGame:
                    break;
                
                case MessageFlags.RemovePlayer:
                {
                    if (!IsPacketAllowed(reader, true))
                    {
                        return;
                    }
                    
                    Message04RemovePlayer.Deserialize(reader, 
                        out var playerId, 
                        out var reason);

                    await Player.Game.HandleRemovePlayer(playerId, (DisconnectReason) reason);
                    break;
                }
                
                case MessageFlags.GameData:
                case MessageFlags.GameDataTo:
                {
                    if (!IsPacketAllowed(reader, false))
                    {
                        return;
                    }

                    // Broadcast packet to all other players.
                    using var writer = Player.Game.CreateMessage(message.Type);
                    
                    if (flag == MessageFlags.GameDataTo)
                    {
                        var target = reader.ReadPackedInt32();
                        reader.CopyTo(writer);
                        await writer.SendToAsync(target);
                    }
                    else
                    {
                        reader.CopyTo(writer);
                        await writer.SendToAllExceptAsync(LimboStates.NotLimbo, Player.Client.Id);
                    }

                    break;
                }
                
                case MessageFlags.EndGame:
                {
                    if (!IsPacketAllowed(reader, true))
                    {
                        return;
                    }

                    await Player.Game.HandleEndGame(reader);
                    break;
                }

                case MessageFlags.AlterGame:
                {
                    if (!IsPacketAllowed(reader, true))
                    {
                        return;
                    }

                    Message10AlterGame.Deserialize(reader, 
                        out var gameTag, 
                        out var value);
                    
                    if (gameTag != AlterGameTags.ChangePrivacy)
                    {
                        return;
                    }

                    await Player.Game.HandleAlterGame(reader, Player, value);
                    break;
                }

                case MessageFlags.KickPlayer:
                {
                    if (!IsPacketAllowed(reader, true))
                    {
                        return;
                    }

                    Message11KickPlayer.Deserialize(reader, 
                        out var playerId, 
                        out var isBan);

                    await Player.Game.HandleKickPlayer(playerId, isBan);
                    break;
                }

                case MessageFlags.GetGameListV2:
                {
                    Message16GetGameListV2.Deserialize(reader, out var options);
                    await Player.OnRequestGameList(options);
                    break;
                }
                
                default:
                    Logger.Warning("Server received unknown flag {0}.", flag);
                    break;
            }
            
#if DEBUG
            if (flag != MessageFlags.GameData &&
                flag != MessageFlags.GameDataTo &&
                flag != MessageFlags.EndGame &&
                reader.Position < reader.Length)
            {
                Logger.Warning("Server did not consume all bytes from {0} ({1} < {2}).",
                    flag,
                    reader.Position,
                    reader.Length);
            }
#endif
        }
        
        protected override async ValueTask OnDisconnected()
        {
            try
            {
                if (Player.Game != null)
                {
                    await Player.Game.HandleRemovePlayer(Id, DisconnectReason.ExitGame);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception caught in client disconnection.");
            }

            _clientManager.Remove(this);
        }
    }
}