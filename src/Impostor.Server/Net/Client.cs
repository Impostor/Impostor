using System;
using Hazel;
using Impostor.Server.Data;
using Impostor.Server.Net.Manager;
using Impostor.Server.Net.Messages;
using Impostor.Server.Net.State;
using Impostor.Shared.Innersloth.Data;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Impostor.Server.Net
{
    public class Client
    {
        private static readonly ILogger Logger = Log.ForContext<Client>();
        
        private readonly ClientManager _clientManager;
        private readonly GameManager _gameManager;

        public Client(ClientManager clientManager, GameManager gameManager, int id, string name, Connection connection)
        {
            _clientManager = clientManager;
            _gameManager = gameManager;
            Id = id;
            Name = name;
            Connection = connection;
            Connection.DataReceived += OnDataReceived;
            Connection.Disconnected += OnDisconnected;
            Player = new ClientPlayer(this);
        }

        public int Id { get; }
        public string Name { get; }
        public Connection Connection { get; }
        public ClientPlayer Player { get; }

        public void Send(MessageWriter writer)
        {
            Connection.Send(writer);
        }
        
        private bool IsPacketAllowed(MessageReader message, bool hostOnly)
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

        private void OnDataReceived(DataReceivedEventArgs e)
        {
            try
            {
                while (true)
                {
                    if (e.Message.Position >= e.Message.Length)
                    {
                        break;
                    }

                    OnMessageReceived(e.Message.ReadMessage(), e.SendOption);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception caught in client data handler.");
                Player.SendDisconnectReason(DisconnectReason.Custom, DisconnectMessages.Error);
            }
        }

        private void OnMessageReceived(MessageReader message, SendOption sendOption)
        {
            var flag = (RequestFlag) message.Tag;
            
            Logger.Verbose("[{0}] Server got {1}.", Id, flag);
            
            switch (flag)
            {
                case RequestFlag.HostGame:
                {
                    // Read game settings.
                    var gameInfo = Message00HostGame.Deserialize(message);
                    
                    // Create game.
                    var game = _gameManager.Create(this, gameInfo);
                    if (game == null)
                    {
                        Player.SendDisconnectReason(DisconnectReason.ServerFull);
                        return;
                    }

                    // Code in the packet below will be used in JoinGame.
                    using (var writer = MessageWriter.Get(SendOption.Reliable))
                    {
                        Message00HostGame.Serialize(writer, game.Code);
                
                        Connection.Send(writer);
                    }
                    break;
                }
                
                case RequestFlag.JoinGame:
                {
                    var gameCode = message.ReadInt32();
                    var unknown = message.ReadByte();
                    var game = _gameManager.Find(gameCode);
                    if (game == null)
                    {
                        Player.SendDisconnectReason(DisconnectReason.GameMissing);
                        return;
                    }

                    game.HandleJoinGame(Player);
                    break;
                }

                case RequestFlag.StartGame:
                {
                    if (!IsPacketAllowed(message, true))
                    {
                        return;
                    }

                    Player.Game.HandleStartGame(message);
                    break;
                }
                
                // No idea how this flag is triggered.
                case RequestFlag.RemoveGame:
                    break;
                
                case RequestFlag.RemovePlayer:
                {
                    if (!IsPacketAllowed(message, true))
                    {
                        return;
                    }

                    var playerId = message.ReadPackedInt32();
                    var reason = message.ReadByte();

                    Player.Game.HandleRemovePlayer(playerId, (DisconnectReason) reason);
                    break;
                }
                
                case RequestFlag.GameData:
                case RequestFlag.GameDataTo:
                {
                    if (!IsPacketAllowed(message, false))
                    {
                        return;
                    }

                    // Broadcast packet to all other players.
                    using (var writer = MessageWriter.Get(sendOption))
                    {
                        if (flag == RequestFlag.GameDataTo)
                        {
                            var target = message.ReadPackedInt32();
                            writer.CopyFrom(message);
                            Player.Game.SendTo(writer, target);
                        }
                        else
                        {
                            writer.CopyFrom(message);
                            Player.Game.SendToAllExcept(writer, Player);
                        }
                    }
                    break;
                }
                
                case RequestFlag.EndGame:
                {
                    if (!IsPacketAllowed(message, true))
                    {
                        return;
                    }

                    Player.Game.HandleEndGame(message);
                    break;
                }

                case RequestFlag.AlterGame:
                {
                    if (!IsPacketAllowed(message, true))
                    {
                        return;
                    }

                    if (message.ReadByte() != (byte) AlterGameTags.ChangePrivacy)
                    {
                        return;
                    }

                    var isPublic = message.ReadByte() == 1;
                    
                    Player.Game.HandleAlterGame(message, Player, isPublic);
                    break;
                }

                case RequestFlag.KickPlayer:
                {
                    if (!IsPacketAllowed(message, true))
                    {
                        return;
                    }

                    var playerId = message.ReadPackedInt32();
                    var isBan = message.ReadBoolean();

                    Player.Game.HandleKickPlayer(playerId, isBan);
                    break;
                }
                
                default:
                    Logger.Warning("Server received unknown flag {0}.", flag);
                    break;
            }
            
#if DEBUG
            if (flag != RequestFlag.GameData &&
                flag != RequestFlag.GameDataTo &&
                flag != RequestFlag.EndGame &&
                message.Position < message.Length)
            {
                Logger.Warning("Server did not consume all bytes from {0} ({1} < {2}).",
                    flag,
                    message.Position,
                    message.Length);
            }
#endif
        }
        
        private void OnDisconnected(object sender, DisconnectedEventArgs e)
        {
            try
            {
                if (Player.Game != null)
                {
                    Player.Game.HandleRemovePlayer(Id, DisconnectReason.ExitGame);
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