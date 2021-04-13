using System.Threading.Tasks;
using Impostor.Api.Net.Messages;
using Impostor.Server.Config;
using Impostor.Server.Net;
using Impostor.Server.Net.Hazel;
using Impostor.Server.Net.Manager;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Recorder
{
    internal class ClientRecorder : Client
    {
        private readonly PacketRecorder _recorder;
        private bool _isFirst;
        private bool _createdGame;
        private bool _recordAfter;

        public ClientRecorder(ILogger<Client> logger, IOptions<AntiCheatConfig> antiCheatOptions, ClientManager clientManager, GameManager gameManager, string name, int gameVersion, HazelConnection connection, PacketRecorder recorder)
            : base(logger, antiCheatOptions, clientManager, gameManager, name, gameVersion, connection)
        {
            _recorder = recorder;
            _isFirst = true;
            _createdGame = false;
            _recordAfter = true;
        }

        public override async ValueTask HandleMessageAsync(IMessageReader reader, MessageType messageType)
        {
            using var messageCopy = reader.Copy();

            // Trigger connect event.
            if (_isFirst)
            {
                _isFirst = false;

                await _recorder.WriteConnectAsync(this);
            }

            // Check if we were in-game before handling the message.
            var inGame = Player?.Game != null;

            if (!_recordAfter)
            {
                // Trigger message event.
                await _recorder.WriteMessageAsync(this, messageCopy, messageType);
            }

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

                // We created a game and are now in-game, store that event.
                if (!inGame && Player?.Game != null)
                {
                    await _recorder.WriteGameCreatedAsync(this, Player.Game.Code);
                }

                _recordAfter = false;

                // Trigger message event.
                await _recorder.WriteMessageAsync(this, messageCopy, messageType);
            }

            if (_recordAfter)
            {
                // Trigger message event.
                await _recorder.WriteMessageAsync(this, messageCopy, messageType);
            }
        }

        public override async ValueTask HandleDisconnectAsync(string reason)
        {
            await _recorder.WriteDisconnectAsync(this, reason);
            await base.HandleDisconnectAsync(reason);
        }
    }
}
