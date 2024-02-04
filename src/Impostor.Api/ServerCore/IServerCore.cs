using Impostor.Api.Net.Manager;

namespace Impostor.Api.ServerCore;

public interface IServerCore
{
    public ICompatibilityManager.CompatibilityGroup CompatibilityGroup { get; set; }

    public T? Get<T>();
}
