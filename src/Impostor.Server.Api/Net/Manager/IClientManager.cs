using System.Threading.Tasks;

namespace Impostor.Server.Net.Manager
{
    public interface IClientManager
    {
        ValueTask RegisterConnectionAsync(IConnection connection, string name, int clientVersion);

        void Register(IClient client);

        void Remove(IClient client);

        bool Validate(IClient client);
    }
}