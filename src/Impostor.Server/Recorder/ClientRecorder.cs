using System.Threading.Tasks;
using Impostor.Api.Net.Messages;
using Impostor.Server.Net;
using Impostor.Server.Net.Hazel;
using Impostor.Server.Net.Manager;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Recorder
{
    internal class ClientRecorder : Client
    {
        private readonly PacketRecorder _recorder;

        public ClientRecorder(ILogger<Client> logger, ClientManager clientManager, GameManager gameManager, string name, HazelConnection connection, PacketRecorder recorder)
            : base(logger, clientManager, gameManager, name, connection)
        {
            _recorder = recorder;
        }

        public override async ValueTask HandleMessageAsync(IMessageReader reader, MessageType messageType)
        {
            await _recorder.WriteMessageAsync(this, reader.Tag, reader.Buffer);
            await base.HandleMessageAsync(reader, messageType);
        }

        public override async ValueTask HandleDisconnectAsync(string reason)
        {
            await _recorder.WriteDisconnectAsync(this);
            await base.HandleDisconnectAsync(reason);
        }
    }
}