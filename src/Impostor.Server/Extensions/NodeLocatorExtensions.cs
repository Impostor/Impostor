using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Impostor.Server.Net.Redirector;

namespace Impostor.Server.Extensions
{
    public static class NodeLocatorExtensions
    {
        public static async ValueTask<bool> NodeExistsAsync(this INodeLocator nodeLocator, string gameCode)
        {
            if (nodeLocator is IAsyncNodeLocator asyncNodeLocator)
            {
                return await asyncNodeLocator.FindAsync(gameCode) != null;
            }

            return nodeLocator.Find(gameCode) != null;
        }

        public static ValueTask SaveAsync(this INodeLocator nodeLocator, string gameCode, IPEndPoint endPoint)
        {
            if (nodeLocator is IAsyncNodeLocator asyncNodeLocator)
            {
                return asyncNodeLocator.SaveAsync(gameCode, endPoint);
            }

            nodeLocator.Save(gameCode, endPoint);
            return ValueTask.CompletedTask;
        }
    }
}