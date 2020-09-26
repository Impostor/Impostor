using Hazel;

namespace Impostor.Server.Net.Manager
{
    internal interface IClientManager
    {
        void Create(string name, Connection connection);
    }
}