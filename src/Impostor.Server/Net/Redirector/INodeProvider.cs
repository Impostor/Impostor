using System.Net;

namespace Impostor.Server.Net.Redirector
{
    internal interface INodeProvider
    {
        IPEndPoint Get();
    }
}
