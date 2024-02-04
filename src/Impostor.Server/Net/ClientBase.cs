using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net;

internal abstract class ClientBase(
    string name,
    GameVersion gameVersion,
    SupportedLanguages language,
    QuickChatModes chatMode,
    PlatformSpecificData platformSpecificData,
    IHazelConnection connection,
    IConnectionData connectionData)
    : IClient
{
    public ClientPlayer? Player { get; set; }

    public int Id { get; set; }

    public IConnectionData _connectionData { get; set; }

    public string Name { get; } = name;

    public SupportedLanguages Language { get; } = language;

    public QuickChatModes ChatMode { get; } = chatMode;

    public PlatformSpecificData PlatformSpecificData { get; } = platformSpecificData;

    public GameVersion GameVersion { get; } = gameVersion;

    public IHazelConnection Connection { get; } = connection;

    public IDictionary<object, object> Items { get; } = new ConcurrentDictionary<object, object>();

    IClientPlayer? IClient.Player => Player;

    public virtual ValueTask<bool> ReportCheatAsync(CheatContext context, string message)
    {
        return new ValueTask<bool>(false);
    }

    public abstract ValueTask HandleMessageAsync(IMessageReader message, MessageType messageType);

    public abstract ValueTask HandleDisconnectAsync(string reason);

    public async ValueTask DisconnectAsync(DisconnectReason reason, string? message = null)
    {
        await Connection.CustomDisconnectAsync(reason, message);
    }
}
