using System.Threading.Tasks;
using Impostor.Server.Net.Redirector;

namespace Impostor.Server
{
    public static class NodeLocatorExtensions
    {
        public static async ValueTask<bool> ExistsAsync(this INodeLocator nodeLocator, string gameCode)
        {
            return await nodeLocator.FindAsync(gameCode) != null;
        }
    }
}
