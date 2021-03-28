using System.Collections.Generic;
using Impostor.Api.Net;
using Impostor.Api.Reactor;

namespace Impostor.Server.Net.Factories
{
    internal interface IClientFactory
    {
        ClientBase Create(IHazelConnection connection, string name, int clientVersion, ISet<Mod> mods);
    }
}
