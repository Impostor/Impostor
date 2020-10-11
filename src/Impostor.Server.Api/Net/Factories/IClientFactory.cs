using System.Threading.Tasks;

namespace Impostor.Server.Net.Factories
{
    public interface IClientFactory
    {
        /// <summary>
        ///     Creates a client for the Hazel <see cref="connection"/>.
        /// </summary>
        /// <param name="connection">Hazel connection.</param>
        /// <param name="name"></param>
        /// <param name="clientVersion"></param>
        IClient Create(IConnection connection, string name, int clientVersion);
    }
}