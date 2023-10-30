using System.Threading.Tasks;
using Impostor.Api.Innersloth.Maps;

namespace Impostor.Api.Net.Inner.Objects
{
    public interface ITaskInfo
    {
        uint Id { get; }

        TaskData? Task { get; }

        bool Complete { get; }

        /// <summary>
        ///     Sets the task as complete.
        /// </summary>
        /// <returns>Task that must be awaited.</returns>
        ValueTask CompleteAsync();
    }
}
