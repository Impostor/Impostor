using System.Threading.Tasks;
using Impostor.Api.Config;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Custom;
using Impostor.Api.Net.Messages;
using Impostor.Server.Net;
using Impostor.Server.Net.Manager;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Recorder;

internal class ClientRecorder(
    ILogger<Client> logger,
    IOptions<AntiCheatConfig> antiCheatOptions,
    ClientManager clientManager,
    ICustomMessageManager<ICustomRootMessage> customMessageManager,
    GameManager gameManager,
    string name,
    GameVersion gameVersion,
    SupportedLanguages language,
    QuickChatModes chatMode,
    PlatformSpecificData platformSpecificData,
    IHazelConnection connection,
    PacketRecorder recorder,
    IConnectionData connectionData)
    : Client(logger, antiCheatOptions, clientManager, gameManager, customMessageManager, name, gameVersion, language,
        chatMode, platformSpecificData, connection, connectionData)
{
    private bool _createdGame;
    private bool _isFirst = true;
    private bool _recordAfter = true;

    public override async ValueTask HandleMessageAsync(IMessageReader reader, MessageType messageType)
    {
        using var messageCopy = reader.Copy();

        // Trigger connect event.
        if (_isFirst)
        {
            _isFirst = false;

            await recorder.WriteConnectAsync(this);
        }

        // Check if we were in-game before handling the message.
        var inGame = Player?.Game != null;

        if (!_recordAfter)
        {
            // Trigger message event.
            await recorder.WriteMessageAsync(this, messageCopy, messageType);
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
                await recorder.WriteGameCreatedAsync(this, Player.Game.Code);
            }

            _recordAfter = false;

            // Trigger message event.
            await recorder.WriteMessageAsync(this, messageCopy, messageType);
        }

        if (_recordAfter)
        {
            // Trigger message event.
            await recorder.WriteMessageAsync(this, messageCopy, messageType);
        }
    }

    public override async ValueTask HandleDisconnectAsync(string reason)
    {
        await recorder.WriteDisconnectAsync(this, reason);
        await base.HandleDisconnectAsync(reason);
    }
}
