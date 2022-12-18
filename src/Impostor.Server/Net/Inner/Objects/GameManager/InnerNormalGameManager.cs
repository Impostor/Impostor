using Impostor.Api.Net.Custom;
using Impostor.Api.Net.Inner.Objects.GameManager;
using Impostor.Server.Net.Inner.Objects.GameManager.Logic;
using Impostor.Server.Net.Inner.Objects.GameManager.Logic.Normal;
using Impostor.Server.Net.State;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Inner.Objects.GameManager;

internal class InnerNormalGameManager : InnerGameManager, IInnerNormalGameManager
{
    public InnerNormalGameManager(ICustomMessageManager<ICustomRpc> customMessageManager, Game game, ILogger<InnerGameManager> logger) : base(customMessageManager, game, logger)
    {
        LogicFlow = AddGameLogic(new LogicGameFlowNormal(this));
        LogicMinigame = AddGameLogic(new LogicMinigame(this));
        LogicRoleSelection = AddGameLogic(new LogicRoleSelectionNormal(this));
        LogicUsables = AddGameLogic(new LogicUsablesBasic(this));
        LogicOptions = AddGameLogic(new LogicOptionsNormal(this, game));
    }
}
