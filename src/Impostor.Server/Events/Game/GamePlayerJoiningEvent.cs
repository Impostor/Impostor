using Impostor.Api.Events;
using Impostor.Api.Games;
using Impostor.Api.Net;

namespace Impostor.Server.Events;

public class GamePlayerJoiningEvent(IGame game, IClientPlayer player) : IGamePlayerJoiningEvent
{
    public IGame Game { get; } = game;

    public IClientPlayer Player { get; } = player;

    public GameJoinResult? JoinResult { get; set; }
}
