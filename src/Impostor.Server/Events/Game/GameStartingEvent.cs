using Impostor.Api.Events;
using Impostor.Api.Games;

namespace Impostor.Server.Events;

public class GameStartingEvent(IGame game) : IGameStartingEvent
{
    public IGame Game { get; } = game;
}
