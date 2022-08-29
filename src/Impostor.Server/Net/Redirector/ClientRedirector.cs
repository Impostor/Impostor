using System.Threading.Tasks;
using Impostor.Api.Config;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Messages;
using Impostor.Api.Net.Messages.C2S;
using Impostor.Api.Net.Messages.S2C;
using Impostor.Hazel;
using Impostor.Server.Net.Hazel;
using Impostor.Server.Net.Manager;
using Serilog;

namespace Impostor.Server.Net.Redirector
{
    internal class ClientRedirector : ClientBase
    {
        private static readonly ILogger Logger = Log.ForContext<ClientRedirector>();

        private readonly ClientManager _clientManager;
        private readonly INodeProvider _nodeProvider;
        private readonly INodeLocator _nodeLocator;

        public ClientRedirector(
            string name,
            int gameVersion,
            Language language,
            QuickChatModes chatMode,
            PlatformSpecificData platformSpecificData,
            HazelConnection connection,
            ClientManager clientManager,
            INodeProvider nodeProvider,
            INodeLocator nodeLocator)
            : base(name, gameVersion, language, chatMode, platformSpecificData, connection)
        {
            _clientManager = clientManager;
            _nodeProvider = nodeProvider;
            _nodeLocator = nodeLocator;
        }

        public override async ValueTask HandleMessageAsync(IMessageReader reader, MessageType messageType)
        {
            var flag = reader.Tag;

            Logger.Verbose("Server got {0}.", flag);

            switch (flag)
            {
                case MessageFlags.HostGame:
                {
                    using var packet = MessageWriter.Get(MessageType.Reliable);
                    Message13RedirectS2C.Serialize(packet, false, _nodeProvider.Get());
                    await Connection.SendAsync(packet);
                    break;
                }

                case MessageFlags.JoinGame:
                {
                    Message01JoinGameC2S.Deserialize(reader, out var gameCode);

                    var endpoint = await _nodeLocator.FindAsync(gameCode);
                    if (endpoint == null)
                    {
                        await DisconnectAsync(DisconnectReason.GameMissing);
                    }
                    else
                    {
                        using var packet = MessageWriter.Get(MessageType.Reliable);
                        Message13RedirectS2C.Serialize(packet, false, endpoint);
                        await Connection.SendAsync(packet);
                    }

                    break;
                }

                case MessageFlags.GetGameListV2:
                {
                    // TODO: Implement.
                    await DisconnectAsync(DisconnectReason.Custom, DisconnectMessages.NotImplemented);
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
