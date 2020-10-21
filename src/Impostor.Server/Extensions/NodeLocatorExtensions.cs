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
        public static async ValueTask<bool> ExistsAsync(this INodeLocator nodeLocator, string gameCode)
        {
            return await nodeLocator.FindAsync(gameCode) != null;
        }
    }
}