using System;
using System.Collections.Generic;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerMeetingHud : IInnerMeetingHud
    {
        IReadOnlyCollection<IInnerMeetingHud.IPlayerVoteArea> IInnerMeetingHud.PlayerStates => Array.AsReadOnly(_playerStates);

        IInnerPlayerInfo? IInnerMeetingHud.Reporter => Reporter;
    }
}
