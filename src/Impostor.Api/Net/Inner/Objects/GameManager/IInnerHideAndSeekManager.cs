using Impostor.Api.Net.Inner.Objects.GameManager.Logic.HideAndSeek;

namespace Impostor.Api.Net.Inner.Objects.GameManager;

public interface IInnerHideAndSeekManager : IInnerGameManager
{
    ILogicGameFlowHnS LogicFlowHnS { get; }
}
