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
    Language language,
    QuickChatModes chatMode,
    PlatformSpecificData platformSpecificData,
    IHazelConnection connection)
    : IClient
{
    public ClientPlayer? Player { get; set; }

    public int Id { get; set; }

    public string Name { get; } = name;

    public Language Language { get; } = language;

    public QuickChatModes ChatMode { get; } = chatMode;

    public PlatformSpecificData PlatformSpecificData { get; } = platformSpecificData;

    public GameVersion GameVersion { get; } = gameVersion;

    public IHazelConnection Connection { get; } = connection;

    public IDictionary<object, object> Items { get; } = new ConcurrentDictionary<object, object>();

    IClientPlayer? IClient.Player
    {
        get => Player;
    }

    public virtual ValueTask<bool> ReportCheatAsync(CheatContext context, CheatCategory category, string message)
    {
        return new ValueTask<bool>(false);
    }

    public ValueTask<bool> ReportCheatAsync(CheatContext context, string message)
    {
        return ReportCheatAsync(context, CheatCategory.Other, message);
    }

    public abstract ValueTask HandleMessageAsync(IMessageReader message, MessageType messageType);

    public abstract ValueTask HandleDisconnectAsync(string reason);

    public async ValueTask DisconnectAsync(DisconnectReason reason, string? message = null)
    {
        await Connection.CustomDisconnectAsync(reason, message);
    }

    public bool Equals(IClient? other)
    {
        return other != null && Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ClientBase);
    }

    public override int GetHashCode()
    {
        return Id;
    }
}
