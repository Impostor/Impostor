namespace Impostor.Server.Net.Hazel
{
    internal partial class HazelConnection : IConnection
    {
        IClient IConnection.Client => Client;
    }
}