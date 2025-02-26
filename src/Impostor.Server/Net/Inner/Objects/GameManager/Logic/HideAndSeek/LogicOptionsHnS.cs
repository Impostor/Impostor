using Impostor.Api.Events.Managers;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner.Objects.GameManager.Logic.HideAndSeek;

internal class LogicOptionsHnS(Game game, IEventManager eventManager) : LogicOptions(game, eventManager);
