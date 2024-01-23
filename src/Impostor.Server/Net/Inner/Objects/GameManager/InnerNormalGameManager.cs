using Impostor.Api.Config;
using Impostor.Api.Events.Managers;
using Impostor.Api.Net.Custom;
using Impostor.Api.Net.Inner.Objects.GameManager;
using Impostor.Server.Net.Inner.Objects.GameManager.Logic;
using Impostor.Server.Net.Inner.Objects.GameManager.Logic.Normal;
using Impostor.Server.Net.State;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Net.Inner.Objects.GameManager;

internal class InnerNormalGameManager : InnerGameManager, IInnerNormalGameManager
{
    public InnerNormalGameManager(ICustomMessageManager<ICustomRpc> customMessageManager, IOptions<AntiCheatConfig> antiCheatConfig, Game game, ILogger<InnerGameManager> logger, IEventManager eventManager) : base(customMessageManager, antiCheatConfig, game, logger)
    {
        LogicFlow = AddGameLogic(new LogicGameFlowNormal());
        LogicMinigame = AddGameLogic(new LogicMinigame());
        LogicRoleSelection = AddGameLogic(new LogicRoleSelectionNormal());
        LogicUsables = AddGameLogic(new LogicUsablesBasic());
        LogicOptions = AddGameLogic(new LogicOptionsNormal(game, eventManager));
    }
}
