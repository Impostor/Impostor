using Impostor.Api.Events.Managers;
using Impostor.Api.Net.Custom;
using Impostor.Api.Net.Inner.Objects.GameManager;
using Impostor.Server.Net.Inner.Objects.GameManager.Logic;
using Impostor.Server.Net.Inner.Objects.GameManager.Logic.Normal;
using Impostor.Server.Net.State;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Inner.Objects.GameManager;

internal class InnerNormalGameManager : InnerGameManager, IInnerNormalGameManager
{
    public InnerNormalGameManager(ICustomMessageManager<ICustomRpc> customMessageManager, Game game, ILogger<InnerGameManager> logger, IEventManager eventManager) : base(customMessageManager, game, logger)
    {
        LogicFlow = AddGameLogic(new LogicGameFlowNormal());
        LogicMinigame = AddGameLogic(new LogicMinigame());
        LogicRoleSelection = AddGameLogic(new LogicRoleSelectionNormal());
        LogicUsables = AddGameLogic(new LogicUsablesBasic());
        LogicOptions = AddGameLogic(new LogicOptionsNormal(game, eventManager));
    }
}
