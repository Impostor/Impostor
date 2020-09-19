using System;
using System.Text.Json;
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
        private readonly Connection _connection;
        private readonly ClientState _state;
        private readonly int _version;
        private readonly string _name;

        public Client(ClientManager clientManager, GameManager gameManager, Connection connection, int version, string name)
        {
            _clientManager = clientManager;
            _gameManager = gameManager;
            _connection = connection;
            _connection.DataReceived += OnDataReceived;
            _connection.Disconnected += OnDisconnected;
            _state = new ClientState(_connection);
            _version = version;
            _name = name;
        }

        private void OnDataReceived(DataReceivedEventArgs e)
        {
            while (true)
            {
                if (e.Message.Position >= e.Message.Length)
                {
                    break;
                }
                
                OnMessageReceived(e.Message.ReadMessage());
            }
        }

        private void OnMessageReceived(MessageReader message)
        {
            var flag = (RequestFlag) message.Tag;
            
            switch (flag)
            {
                // 101A3813
                case RequestFlag.HostGame:
                    Logger.Debug("Server got host game");
                    
                    // Read game settings.
                    var gameInfoBytes = message.ReadBytesAndSize();
                    var gameInfo = GameOptionsData.Deserialize(gameInfoBytes);
                    
                    // Create game.
                    var game = _gameManager.Create(this, gameInfo);
                    if (game == null)
                    {
                        _connection.Send(new Message1DisconnectReason(DisconnectReason.ServerFull));
                        return;
                    }

                    // Code (32) in the packet below will be used in JoinGame.
                    using (var writer = MessageWriter.Get(SendOption.Reliable))
                    {
                        writer.StartMessage(0);
                        writer.Write(32);
                        writer.EndMessage();
                
                        _connection.Send(writer);
                    }
                    break;
                // 101A388C
                case RequestFlag.JoinGame:
                    Logger.Debug("Server got join game");

                    var gameCode = message.ReadInt32();
                    if (gameCode != 32)
                    {
                        Logger.Debug("- Code {0}", gameCode, GameCode.IntToGameName(gameCode));
                        
                        _connection.Send(new Message1DisconnectReason(DisconnectReason.GameMissing));
                        return;
                    }
                    
                    // TODO: JoinGame
                    Logger.Debug("JoinGame {0} {1}", gameCode, message.ReadByte());
                    break;
                // 101A3960
                case RequestFlag.StartGame:
                    Logger.Debug("Server got StartGame");
                    break;
                // 101A39EC
                case RequestFlag.RemoveGame:
                    Logger.Debug("Server got RemoveGame");
                    break;
                case RequestFlag.RemovePlayer:
                    Logger.Debug("Server got RemovePlayer");
                    break;
                // 101A3A15
                case RequestFlag.GameData:
                    Logger.Debug("Server got GameData");
                    break;
                // 101A3AAB
                case RequestFlag.GameDataTo:
                    Logger.Debug("Server got GameDataTo");
                    break;
                // 101A3BA6
                case RequestFlag.JoinedGame:
                    Logger.Debug("Server got JoinedGame");
                    break;
                // 101A3BD0
                case RequestFlag.EndGame:
                    Logger.Debug("Server got EndGame");
                    break;
                default:
                    Logger.Debug("Server received unknown {0}", flag);
                    break;
            }

            if (message.Position < message.Length)
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