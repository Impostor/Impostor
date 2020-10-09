using System.Net;
using System.Threading.Tasks;

namespace Impostor.Server.Net.Manager
{
    public interface IMatchmaker
    {
        ValueTask StartAsync(IPEndPoint ipEndPoint);

        ValueTask StopAsync();

        IGameMessageWriter CreateGameMessageWriter(IGame game, MessageType messageType);
    }
}