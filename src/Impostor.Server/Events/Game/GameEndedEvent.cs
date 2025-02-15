using Impostor.Api.Events;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;

namespace Impostor.Server.Events;

public class GameEndedEvent(IGame game, GameOverReason gameOverReason) : IGameEndedEvent
{
    public IGame Game { get; } = game;

    public GameOverReason GameOverReason { get; } = gameOverReason;
}
