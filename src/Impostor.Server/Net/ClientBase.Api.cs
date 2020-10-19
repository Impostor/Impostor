using Impostor.Api.Net;

namespace Impostor.Server.Net
{
    internal abstract partial class ClientBase : IClient
    {
        IConnection IClient.Connection => Connection;

        IClientPlayer IClient.Player => Player;
    }
}