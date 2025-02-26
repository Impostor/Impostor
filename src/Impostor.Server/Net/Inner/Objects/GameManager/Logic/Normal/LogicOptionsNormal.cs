using Impostor.Api.Events.Managers;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner.Objects.GameManager.Logic.Normal;

internal class LogicOptionsNormal(Game game, IEventManager eventManager) : LogicOptions(game, eventManager);
