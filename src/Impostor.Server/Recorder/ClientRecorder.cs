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
        private bool _isFirst;
        private bool _createdGame;

        public ClientRecorder(ILogger<Client> logger, ClientManager clientManager, GameManager gameManager, string name, HazelConnection connection, PacketRecorder recorder)
            : base(logger, clientManager, gameManager, name, connection)
        {
            _recorder = recorder;
            _isFirst = true;
            _createdGame = false;
        }

        public override async ValueTask HandleMessageAsync(IMessageReader reader, MessageType messageType)
        {
            var messageCopy = reader.Slice(0);

            // Trigger connect event.
            if (_isFirst)
            {
                _isFirst = false;

                await _recorder.WriteConnectAsync(this);
            }

            // Check if we were in-game before handling the message.
            var inGame = Player?.Game != null;

            // Handle the message.
            await base.HandleMessageAsync(reader, messageType);

            // Player created a game.
            if (reader.Tag == MessageFlags.HostGame)
            {
                _createdGame = true;
            }
            else if (reader.Tag == MessageFlags.JoinGame && _createdGame)
            {
                _createdGame = false;

                // We created a game and are now in-game, stored that event.
                if (!inGame && Player?.Game != null)
                {
                    await _recorder.WriteGameCreatedAsync(this, Player.Game.Code);
                }
            }

            // Trigger message event.
            await _recorder.WriteMessageAsync(this, messageCopy, messageType);
        }

        public override async ValueTask HandleDisconnectAsync(string reason)
        {
            await _recorder.WriteDisconnectAsync(this);
            await base.HandleDisconnectAsync(reason);
        }
    }
}