using System;
using Hazel;
using Impostor.Server.Data;
using Impostor.Server.Net.Messages;
using Impostor.Shared.Innersloth;
using Impostor.Shared.Innersloth.Data;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Impostor.Server.Net.Redirector
{
    internal class ClientRedirector
    {
        private static readonly ILogger Logger = Log.ForContext<ClientRedirector>();

        private readonly string _name;
        private readonly Connection _connection;
        private readonly ClientManagerRedirector _clientManager;
        private readonly INodeProvider _nodeProvider;
        private readonly INodeLocator _nodeLocator;

        public ClientRedirector(string name, Connection connection, ClientManagerRedirector clientManager, INodeProvider nodeProvider, INodeLocator nodeLocator)
        {
            _name = name;
            _connection = connection;
            _connection.DataReceived += OnDataReceived;
            _connection.Disconnected += OnDisconnected;
            _clientManager = clientManager;
            _nodeProvider = nodeProvider;
            _nodeLocator = nodeLocator;
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

                    OnMessageReceived(e.Message.ReadMessage());
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception caught in client data handler.");
            }
        }

        private void OnMessageReceived(MessageReader message)
        {
            var flag = message.Tag;
            
            Logger.Verbose("Server got {0}.", flag);

            switch (flag)
            {
                case MessageFlags.HostGame:
                {
                    using (var packet = MessageWriter.Get(SendOption.Reliable))
                    {
                        Message13Redirect.Serialize(packet, false, _nodeProvider.Get());
                        _connection.Send(packet);
                    }
                    break;
                }

                case MessageFlags.JoinGame:
                {
                    Message01JoinGame.Deserialize(message, 
                        out var gameCode, 
                        out var unknown);

                    using (var packet = MessageWriter.Get(SendOption.Reliable))
                    {
                        var endpoint = _nodeLocator.Find(GameCode.IntToGameName(gameCode));
                        if (endpoint == null)
                        {
                            Message01JoinGame.SerializeError(packet, false, DisconnectReason.GameMissing);
                        }
                        else
                        {
                            Message13Redirect.Serialize(packet, false, endpoint);
                        }
                        
                        _connection.Send(packet);
                    }
                    break;
                }

                case MessageFlags.GetGameListV2:
                {
                    // TODO: Implement.
                    using (var packet = MessageWriter.Get(SendOption.Reliable))
                    {
                        Message01JoinGame.SerializeError(packet, false, DisconnectReason.Custom, DisconnectMessages.NotImplemented);
                        _connection.Send(packet);
                    }
                    break;
                }

                default:
                {
                    Logger.Warning("Received unsupported message flag on the redirector ({0}).", flag);
                    break;
                }
            }
        }

        private void OnDisconnected(object sender, DisconnectedEventArgs e)
        {
            _clientManager.Remove(this);
        }
    }
}