using Impostor.Api.Events;
using Impostor.Api.Games;
using Impostor.Api.Net;

namespace Impostor.Server.Events;

public class GameHostChangedEvent(IGame game, IClientPlayer previousHost, IClientPlayer? newHost)
    : IGameHostChangedEvent
{
    public IGame Game { get; } = game;

    public IClientPlayer PreviousHost { get; } = previousHost;

    public IClientPlayer? NewHost { get; } = newHost;
}
