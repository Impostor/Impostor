using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hazel;
using Impostor.Server.Net.Messages;

namespace Impostor.Server.Net
{
    public abstract class ClientBase : IClient
    {
        protected ClientBase(string name, IConnection connection)
        {
            Name = name;
            Connection = connection;
            Items = new ConcurrentDictionary<object, object>();
        }

        public int Id { get; set; }

        public string Name { get; }

        public IConnection Connection { get; }

        public bool IsBot => false;

        public IDictionary<object, object> Items { get; }

        public IClientPlayer Player { get; set; }

        public abstract ValueTask HandleMessageAsync(IMessage message);

        public abstract ValueTask HandleDisconnectAsync(string reason);
    }
}