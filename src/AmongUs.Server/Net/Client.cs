using System;
using AmongUs.Server.Data;
using AmongUs.Server.Exceptions;
using AmongUs.Server.Extensions;
using AmongUs.Server.Net.Request;
using AmongUs.Server.Net.Response;
using AmongUs.Shared.Innersloth;
using AmongUs.Shared.Innersloth.Data;
using Hazel;
using Serilog;
using ILogger = Serilog.ILogger;

namespace AmongUs.Server.Net
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
                Connection.Send(new Message1DisconnectReason(DisconnectReason.Custom, DisconnectMessages.Error));
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
                    var gameInfoBytes = message.ReadBytesAndSize();
                    var gameInfo = GameOptionsData.Deserialize(gameInfoBytes);
                    
                    // Create game.
                    var game = _gameManager.Create(this, gameInfo);
                    if (game == null)
                    {
                        Connection.Send(new Message1DisconnectReason(DisconnectReason.ServerFull));
                        return;
                    }

                    // Code in the packet below will be used in JoinGame.
                    using (var writer = MessageWriter.Get(SendOption.Reliable))
                    {
                        writer.StartMessage(0);
                        writer.Write(game.Code);
                        writer.EndMessage();
                
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
                        Connection.Send(new Message1DisconnectReason(DisconnectReason.GameMissing));
                        return;
                    }

                    game.HandleJoinGame(Player);
                    break;
                }
                
                // 101A3960
                case RequestFlag.StartGame:
                    break;
                
                // 101A39EC
                case RequestFlag.RemoveGame:
                    break;
                
                case RequestFlag.RemovePlayer:
                    break;
                
                case RequestFlag.GameData:
                case RequestFlag.GameDataTo:
                {
                    var game = Player.Game;
                    if (game == null)
                    {
                        throw new NullReferenceException("Game was not set for the client.");
                    }
                    
                    var code = message.ReadInt32();
                    if (code != game.Code)
                    {
                        // Packet was meant for another game.
                        return;
                    }

                    // Broadcast packet to all other players.
                    using (var writer = MessageWriter.Get(sendOption))
                    {
                        if (flag == RequestFlag.GameDataTo)
                        {
                            var target = message.ReadPackedInt32();
                            writer.CopyFrom(message);
                            game.SendTo(writer, target);
                        }
                        else
                        {
                            writer.CopyFrom(message);
                            game.SendToAllExcept(writer, Player);
                        }
                    }
                    break;
                }
                
                // 101A3BA6
                case RequestFlag.JoinedGame:
                    break;
                
                // 101A3BD0
                case RequestFlag.EndGame:
                    break;
                
                default:
                    Logger.Warning("Server received unknown flag {0}.", flag);
                    break;
            }

            if (flag != RequestFlag.GameData &&
                flag != RequestFlag.GameDataTo &&
                message.Position < message.Length)
            {
                Logger.Warning("Server did not consume all bytes from {0} ({1} < {2}).", 
                    flag, 
                    message.Position, 
                    message.Length);
            }
        }
        
        private void OnDisconnected(object sender, DisconnectedEventArgs e)
        {
            _clientManager.Remove(this);
        }
    }
}