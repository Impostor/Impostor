using Impostor.Api.Games.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.GameOptions;
using Impostor.Api.Net;

namespace Impostor.Api.Events;

public class GameCreationEvent(IClient? client, IGameManager gameManager, IGameOptions gameOptions, GameFilterOptions filterOptions) : IEvent
{

    public IClient? Creator { get; } = client;
    
    public IGameManager GameManager { get; } = gameManager;

    public IGameOptions GameOptions { get; } = gameOptions;
    
    public GameFilterOptions FilterOptions { get; } = filterOptions;

    public NumberEventOutcome<bool> Cancel { get; set; } = new(false);
}
