using System.Threading.Tasks;
using Hazel;
using Impostor.Server.Net;

namespace Impostor.Server.Hazel
{
    internal class HazelConnectionMessageWriter : HazelMessageWriter, IConnectionMessageWriter
    {
        private readonly Connection _connection;

        public HazelConnectionMessageWriter(MessageType type, Connection connection)
            : base(type)
        {
            _connection = connection;
        }
        
        public ValueTask SendAsync()
        {
            _connection.Send(Writer);
            return default;
        }
    }
}