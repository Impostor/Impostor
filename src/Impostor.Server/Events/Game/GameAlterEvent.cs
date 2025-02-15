using Impostor.Api.Events;
using Impostor.Api.Games;

namespace Impostor.Server.Events;

public class GameAlterEvent(IGame game, bool isPublic) : IGameAlterEvent
{
    public IGame Game { get; } = game;

    public bool IsPublic { get; } = isPublic;
}
