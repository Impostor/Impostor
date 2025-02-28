using System.Threading.Tasks;
using Impostor.Api.Events.Managers;
using Impostor.Api.Extension.Events;
using Impostor.Api.Games;

namespace Impostor.Server.Net;

public class GameCodeFactory(IEventManager eventManager) : IGameCodeFactory
{
    public async ValueTask<GameCode> CreateAsync(GameCreationEvent? creationEvent = null)
    {
        var @event = new GameCodeCreateEvent(creationEvent);
        await eventManager.CallAsync(@event);
        return GameCode.Create();
    }
    
    public async ValueTask<GameCode> Create()
    {
        var @event = new GameCodeCreateEvent();
        await eventManager.CallAsync(@event);
        return @event.Result ?? GameCode.Create();
    }
}
