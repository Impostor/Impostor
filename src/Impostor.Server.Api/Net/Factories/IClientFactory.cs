using System.Threading.Tasks;

namespace Impostor.Server.Net.Factories
{
    public interface IClientFactory
    {
        /// <summary>
        ///     Get the next ID for <see cref="IClient"/>.
        /// </summary>
        int NextId();

        /// <summary>
        ///     Creates a client for the Hazel <see cref="connection"/>.
        /// </summary>
        /// <param name="connection">Hazel connection.</param>
        /// <param name="name"></param>
        /// <param name="clientVersion"></param>
        ValueTask<IClient> CreateAsync(IConnection connection, string name, int clientVersion);
    }
}