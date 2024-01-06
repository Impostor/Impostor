using System.Collections.Generic;
using Impostor.Api.ServerCore;

namespace Impostor.Server.ServerCore;

public class ServerCoreManager : IServerCoreManager
{
    private List<IServerCore> _allServerCore = [];

    public void AddServerCore(IServerCore serverCore) => _allServerCore.Add(serverCore);
}
