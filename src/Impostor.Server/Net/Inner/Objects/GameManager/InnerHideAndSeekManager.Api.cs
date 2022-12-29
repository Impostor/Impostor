using Impostor.Api.Net.Inner.Objects.GameManager;
using Impostor.Api.Net.Inner.Objects.GameManager.Logic.HideAndSeek;

namespace Impostor.Server.Net.Inner.Objects.GameManager;

internal partial class InnerHideAndSeekManager
{
    ILogicGameFlowHnS IInnerHideAndSeekManager.LogicFlowHnS => LogicFlowHnS;
}
