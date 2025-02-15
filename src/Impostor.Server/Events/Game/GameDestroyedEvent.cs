using Impostor.Api.Events;
using Impostor.Api.Games;

namespace Impostor.Server.Events;

public class GameDestroyedEvent(IGame game) : IGameDestroyedEvent
{
    public IGame Game { get; } = game;
}
