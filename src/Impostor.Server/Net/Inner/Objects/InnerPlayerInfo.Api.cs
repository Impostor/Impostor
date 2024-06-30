using System.Collections.Generic;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerPlayerInfo : InnerNetObject, IInnerPlayerInfo
    {
        IEnumerable<ITaskInfo> IInnerPlayerInfo.Tasks => Tasks;
    }
}
