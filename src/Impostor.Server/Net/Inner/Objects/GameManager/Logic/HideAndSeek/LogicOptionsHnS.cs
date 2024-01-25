using Impostor.Api.Events.Managers;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner.Objects.GameManager.Logic.HideAndSeek;

internal class LogicOptionsHnS : LogicOptions
{
    public LogicOptionsHnS(Game game, IEventManager eventManager) : base(game, eventManager)
    {
    }
}
