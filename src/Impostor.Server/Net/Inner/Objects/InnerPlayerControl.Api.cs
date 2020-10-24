using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerPlayerControl : IInnerPlayerControl
    {
        IInnerPlayerInfo IInnerPlayerControl.PlayerInfo => PlayerInfo;
    }
}