using System.Text.Json.Serialization;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;

namespace SelfHttpMatchmaker.Types;

public class GameListing
{
    [JsonPropertyName("IP")] public required long Ip { get; init; }

    [JsonPropertyName("Port")] public required ushort Port { get; init; }

    [JsonPropertyName("GameId")] public required int GameId { get; init; }

    [JsonPropertyName("PlayerCount")] public required int PlayerCount { get; init; }

    [JsonPropertyName("HostName")] public required string HostName { get; init; }

    [JsonPropertyName("HostPlatformName")] public required string HostPlatformName { get; init; }

    [JsonPropertyName("Platform")] public required Platforms Platform { get; init; }

    [JsonPropertyName("Age")] public required int Age { get; init; }

    [JsonPropertyName("MaxPlayers")] public required int MaxPlayers { get; init; }

    [JsonPropertyName("NumImpostors")] public required int NumImpostors { get; init; }

    [JsonPropertyName("MapId")] public required MapTypes MapId { get; init; }

    [JsonPropertyName("Language")] public required GameKeywords Language { get; init; }

    public static GameListing From(IGame game, long ip, ushort port)
    {
        var platform = game.Host?.Client.PlatformSpecificData;

        return new GameListing
        {
            Ip = ip,
            Port = port,
            GameId = game.Code,
            PlayerCount = game.PlayerCount,
            HostName = game.DisplayName ?? game.Host?.Client.Name ?? "Unknown host",
            HostPlatformName = platform?.PlatformName ?? string.Empty,
            Platform = platform?.Platform ?? Platforms.Unknown,
            Age = 0,
            MaxPlayers = game.Options.MaxPlayers,
            NumImpostors = game.Options.NumImpostors,
            MapId = game.Options.Map,
            Language = game.Options.Keywords,
        };
    }
}
