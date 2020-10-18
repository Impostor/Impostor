using Impostor.Server.Net;

// ReSharper disable once CheckNamespace
namespace Impostor.Server.Hazel
{
    internal partial class HazelConnection : IConnection
    {
        IClient IConnection.Client => Client;
    }
}