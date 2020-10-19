using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Impostor.Api.Net.Messages;
using Impostor.Server.Net.Hazel;
using Impostor.Server.Net.Messages;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net
{
    internal abstract partial class ClientBase
    {
        protected ClientBase(string name, HazelConnection connection)
        {
            Name = name;
            Connection = connection;
            Items = new ConcurrentDictionary<object, object>();
        }

        public int Id { get; set; }

        public string Name { get; }

        public HazelConnection Connection { get; }

        public bool IsBot => false;

        public IDictionary<object, object> Items { get; }

        public ClientPlayer Player { get; set; }

        public abstract ValueTask HandleMessageAsync(IMessage message);

        public abstract ValueTask HandleDisconnectAsync(string reason);
    }
}