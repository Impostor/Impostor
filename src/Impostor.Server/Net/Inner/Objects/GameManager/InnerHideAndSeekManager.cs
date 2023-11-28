using Impostor.Api.Events.Managers;
using Impostor.Api.Net.Custom;
using Impostor.Api.Net.Inner.Objects.GameManager;
using Impostor.Server.Net.Inner.Objects.GameManager.Logic.HideAndSeek;
using Impostor.Server.Net.State;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Inner.Objects.GameManager;

internal partial class InnerHideAndSeekManager : InnerGameManager, IInnerHideAndSeekManager
{
    public InnerHideAndSeekManager(ICustomMessageManager<ICustomRpc> customMessageManager, Game game, ILogger<InnerGameManager> logger, IEventManager eventManager) : base(customMessageManager, game, logger)
    {
        LogicMusic = AddGameLogic(new LogicHnSMusic());
        LogicMinigame = AddGameLogic(new LogicMinigameHnS());
        LogicFlowHnS = new LogicGameFlowHnS();
        LogicFlow = AddGameLogic(LogicFlowHnS);
        LogicUsables = AddGameLogic(new LogicUsablesHnS());
        LogicRoleSelection = AddGameLogic(new LogicRoleSelectionHnS());
        LogicOptionsHnS = new LogicOptionsHnS(game, eventManager);
        LogicOptions = AddGameLogic(this.LogicOptionsHnS);
        LogicDangerLevel = AddGameLogic(new LogicHnSDangerLevel());
        LogicPing = AddGameLogic(new LogicPingsHnS());
        LogicDeathPopup = AddGameLogic(new LogicHnSDeathPopup());
    }

    public LogicHnSMusic LogicMusic { get; private set; }

    public LogicGameFlowHnS LogicFlowHnS { get; private set; }

    public LogicOptionsHnS LogicOptionsHnS { get; private set; }

    public LogicHnSDangerLevel LogicDangerLevel { get; private set; }

    public LogicPingsHnS LogicPing { get; private set; }

    public LogicHnSDeathPopup LogicDeathPopup { get; private set; }
}
