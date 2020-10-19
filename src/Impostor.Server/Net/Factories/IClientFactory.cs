using Impostor.Server.Net.Hazel;

namespace Impostor.Server.Net.Factories
{
    internal interface IClientFactory
    {
        ClientBase Create(HazelConnection connection, string name, int clientVersion);
    }
}