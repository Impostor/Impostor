using System.Threading.Tasks;
using Impostor.Server.Data;
using Impostor.Server.Net.Manager;
using Impostor.Server.Net.Messages;
using Impostor.Shared.Innersloth;
using Impostor.Shared.Innersloth.Data;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Impostor.Server.Net.Redirector
{
    internal class ClientRedirector : ClientBase
    {
        private static readonly ILogger Logger = Log.ForContext<ClientRedirector>();

        private readonly IClientManager _clientManager;
        private readonly INodeProvider _nodeProvider;
        private readonly INodeLocator _nodeLocator;

        public ClientRedirector(
            string name,
            IConnection connection,
            IClientManager clientManager,
            INodeProvider nodeProvider,
            INodeLocator nodeLocator)
            : base(name, connection)
        {
            _clientManager = clientManager;
            _nodeProvider = nodeProvider;
            _nodeLocator = nodeLocator;
        }

        public override async ValueTask HandleMessageAsync(IMessage message)
        {
            var reader = message.CreateReader();
            var flag = reader.Tag;

            Logger.Verbose("Server got {0}.", flag);

            switch (flag)
            {
                case MessageFlags.HostGame:
                {
                    using var packet = Connection.CreateMessage(MessageType.Reliable);
                    Message13Redirect.Serialize(packet, false, _nodeProvider.Get());
                    await packet.SendAsync();
                    break;
                }

                case MessageFlags.JoinGame:
                {
                    Message01JoinGame.Deserialize(
                        reader,
                        out var gameCode,
                        out _);

                    using var packet = Connection.CreateMessage(MessageType.Reliable);
                    var endpoint = _nodeLocator.Find(GameCodeParser.IntToGameName(gameCode));
                    if (endpoint == null)
                    {
                        Message01JoinGame.SerializeError(packet, false, DisconnectReason.GameMissing);
                    }
                    else
                    {
                        Message13Redirect.Serialize(packet, false, endpoint);
                    }

                    await packet.SendAsync();
                    break;
                }

                case MessageFlags.GetGameListV2:
                {
                    // TODO: Implement.
                    using var packet = Connection.CreateMessage(MessageType.Reliable);
                    Message01JoinGame.SerializeError(packet, false, DisconnectReason.Custom, DisconnectMessages.NotImplemented);
                    await packet.SendAsync();
                    break;
                }

                default:
                {
                    Logger.Warning("Received unsupported message flag on the redirector ({0}).", flag);
                    break;
                }
            }
        }

        public override ValueTask HandleDisconnectAsync(string reason)
        {
            _clientManager.Remove(this);
            return default;
        }
    }
}