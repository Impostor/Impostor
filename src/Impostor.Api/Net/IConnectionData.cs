using Impostor.Api.Innersloth;

namespace Impostor.Api.Net;

public interface IConnectionData
{
    IHazelConnection _connection { get; set; }

    IMessageReader HandshakeData { get; set; }

    string? MatchmakerToken { get; set; }

    uint? LastId { get; set; }

    Platforms Platforms { get; set; }

    string FriendCode { get; set; }

    GameVersion? Version { get; set; }

    string Name { get; set; }

    SupportedLanguages Language { get; set; }

    QuickChatModes ChatMode { get; set; }

    string PlatformName { get; set; }

    uint PlatformId { get; set; }
}
