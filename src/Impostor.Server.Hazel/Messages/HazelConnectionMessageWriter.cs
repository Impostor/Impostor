using System.Threading.Tasks;
using Impostor.Server.Net;
using Impostor.Server.Net.Messages;

namespace Impostor.Server.Hazel.Messages
{
    internal class HazelConnectionMessageWriter : HazelMessageWriter, IConnectionMessageWriter
    {
        private readonly HazelConnection _connection;

        public HazelConnectionMessageWriter(MessageType type, HazelConnection connection)
            : base(type)
        {
            _connection = connection;
        }

        public IConnection Connection => _connection;

        public async ValueTask SendAsync()
        {
            await _connection.InnerConnection.Send(Writer);
        }
    }
}