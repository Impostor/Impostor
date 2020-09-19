using Hazel;

namespace AmongUs.Server.Net
{
    public class ClientState
    {
        private readonly Connection _connection;

        public ClientState(Connection connection)
        {
            _connection = connection;
        }
    }
}