using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Inner.Objects
{
    public interface ITaskInfo
    {
        uint Id { get; }

        TaskTypes Type { get; }

        bool Complete { get; }
    }
}
