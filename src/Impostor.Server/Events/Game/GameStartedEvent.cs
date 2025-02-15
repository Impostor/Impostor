using Impostor.Api.Events;
using Impostor.Api.Games;

namespace Impostor.Server.Events;

public class GameStartedEvent(IGame game) : IGameStartedEvent
{
    public IGame Game { get; } = game;
}
