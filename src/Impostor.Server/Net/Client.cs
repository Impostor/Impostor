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
    internal class Client
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
            Player = new ClientPlayer(this, _gameManager);
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
            finally
            {
                e.Message.Recycle();
            }
        }

        private void OnMessageReceived(MessageReader message, SendOption sendOption)
        {
            var flag = message.Tag;
            
            Logger.Verbose("[{0}] Server got {1}.", Id, flag);
            
            switch (flag)
            {
                case MessageFlags.HostGame:
                {
                    // Read game settings.
                    var gameInfo = Message00HostGame.Deserialize(message);
                    
                    // Create game.
                    var game = _gameManager.Create(gameInfo);
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
                
                case MessageFlags.JoinGame:
                {
                    Message01JoinGame.Deserialize(message, 
                        out var gameCode, 
                        out var unknown);
                    
                    var game = _gameManager.Find(gameCode);
                    if (game == null)
                    {
                        Player.SendDisconnectReason(DisconnectReason.GameMissing);
                        return;
                    }

                    game.HandleJoinGame(Player);
                    break;
                }

                case MessageFlags.StartGame:
                {
                    if (!IsPacketAllowed(message, true))
                    {
                        return;
                    }

                    Player.Game.HandleStartGame(message);
                    break;
                }
                
                // No idea how this flag is triggered.
                case MessageFlags.RemoveGame:
                    break;
                
                case MessageFlags.RemovePlayer:
                {
                    if (!IsPacketAllowed(message, true))
                    {
                        return;
                    }
                    
                    Message04RemovePlayer.Deserialize(message, 
                        out var playerId, 
                        out var reason);

                    Player.Game.HandleRemovePlayer(playerId, (DisconnectReason) reason);
                    break;
                }
                
                case MessageFlags.GameData:
                case MessageFlags.GameDataTo:
                {
                    if (!IsPacketAllowed(message, false))
                    {
                        return;
                    }

                    // Broadcast packet to all other players.
                    using (var writer = MessageWriter.Get(sendOption))
                    {
                        if (flag == MessageFlags.GameDataTo)
                        {
                            var target = message.ReadPackedInt32();
                            writer.CopyFrom(message);
                            Player.Game.SendTo(writer, target);
                        }
                        else
                        {
                            writer.CopyFrom(message);
                            Player.Game.SendToAllExcept(writer, Player.Client.Id);
                        }
                    }
                    break;
                }
                
                case MessageFlags.EndGame:
                {
                    if (!IsPacketAllowed(message, true))
                    {
                        return;
                    }

                    Player.Game.HandleEndGame(message);
                    break;
                }

                case MessageFlags.AlterGame:
                {
                    if (!IsPacketAllowed(message, true))
                    {
                        return;
                    }

                    Message10AlterGame.Deserialize(message, 
                        out var gameTag, 
                        out var value);
                    
                    if (gameTag != AlterGameTags.ChangePrivacy)
                    {
                        return;
                    }

                    Player.Game.HandleAlterGame(message, Player, value);
                    break;
                }

                case MessageFlags.KickPlayer:
                {
                    if (!IsPacketAllowed(message, true))
                    {
                        return;
                    }

                    Message11KickPlayer.Deserialize(message, 
                        out var playerId, 
                        out var isBan);

                    Player.Game.HandleKickPlayer(playerId, isBan);
                    break;
                }

                case MessageFlags.GetGameListV2:
                {
                    Message16GetGameListV2.Deserialize(message, out var options);
                    Player.OnRequestGameList(options);
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