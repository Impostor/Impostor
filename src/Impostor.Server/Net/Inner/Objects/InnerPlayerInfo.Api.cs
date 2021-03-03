using System.Collections.Generic;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerPlayerInfo : IInnerPlayerInfo
    {
        IEnumerable<ITaskInfo> IInnerPlayerInfo.Tasks => Tasks;
    }
}
