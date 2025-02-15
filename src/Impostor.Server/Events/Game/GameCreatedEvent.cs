using Impostor.Api.Events;
using Impostor.Api.Games;
using Impostor.Api.Net;

namespace Impostor.Server.Events;

public class GameCreatedEvent(IGame game, IClient? host) : IGameCreatedEvent
{
    public IGame Game { get; } = game;

    public IClient? Host { get; } = host;
}
