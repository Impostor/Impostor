using Impostor.Api.Innersloth;
using Impostor.Api.Net;

namespace Impostor.Server.Net;

public class ConnectionData : IConnectionData
{
    public IHazelConnection _connection { get; set; }

    public IMessageReader HandshakeData { get; set; }

    public string? MatchmakerToken { get; set; }

    public uint? LastId { get; set; }

    public Platforms Platforms { get; set; }

    public string FriendCode { get; set; }

    public GameVersion? Version { get; set; }

    public string Name { get; set; }

    public SupportedLanguages Language { get; set; }

    public QuickChatModes ChatMode { get; set; }

    public string PlatformName { get; set; }

    public uint PlatformId { get; set; }
}
