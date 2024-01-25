using Impostor.Api.Events.Managers;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner.Objects.GameManager.Logic.Normal;

internal class LogicOptionsNormal : LogicOptions
{
    public LogicOptionsNormal(Game game, IEventManager eventManager) : base(game, eventManager)
    {
    }
}
