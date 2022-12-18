using Impostor.Api.Net.Custom;
using Impostor.Api.Net.Inner.Objects.GameManager;
using Impostor.Server.Net.Inner.Objects.GameManager.Logic.HideAndSeek;
using Impostor.Server.Net.State;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Inner.Objects.GameManager;

internal class InnerHideAndSeekManager : InnerGameManager, IInnerHideAndSeekManager
{
    public InnerHideAndSeekManager(ICustomMessageManager<ICustomRpc> customMessageManager, Game game, ILogger<InnerGameManager> logger) : base(customMessageManager, game, logger)
    {
        LogicMusic = AddGameLogic(new LogicHnSMusic(this));
        LogicMinigame = AddGameLogic(new LogicMinigameHnS(this));
        LogicFlowHnS = new LogicGameFlowHnS(this);
        LogicFlow = AddGameLogic(LogicFlowHnS);
        LogicUsables = AddGameLogic(new LogicUsablesHnS());
        LogicRoleSelection = AddGameLogic(new LogicRoleSelectionHnS(this));
        LogicOptionsHnS = new LogicOptionsHnS(this, game);
        LogicOptions = AddGameLogic(this.LogicOptionsHnS);
        LogicDangerLevel = AddGameLogic(new LogicHnSDangerLevel(this));
        LogicPing = AddGameLogic(new LogicPingsHnS(this));
        LogicDeathPopup = AddGameLogic(new LogicHnSDeathPopup(this));
    }

    public LogicHnSMusic LogicMusic { get; private set; }

    public LogicGameFlowHnS LogicFlowHnS { get; private set; }

    public LogicOptionsHnS LogicOptionsHnS { get; private set; }

    public LogicHnSDangerLevel LogicDangerLevel { get; private set; }

    public LogicPingsHnS LogicPing { get; private set; }

    public LogicHnSDeathPopup LogicDeathPopup { get; private set; }
}
