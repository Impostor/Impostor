using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Impostor.Server.Net
{
    public abstract class ClientBase : IClient
    {
        protected ClientBase(int id, string name, IConnection connection)
        {
            Id = id;
            Name = name;
            Connection = connection;
            Items = new ConcurrentDictionary<object, object>();
        }

        public int Id { get; }
        
        public string Name { get; }
        
        public IConnection Connection { get; }
        
        public IDictionary<object, object> Items { get; }

        public virtual async ValueTask InitializeAsync()
        {
            await Connection.MessageReceived.SubscribeAsync(OnMessageReceived, OnDisconnected);
        }

        protected abstract ValueTask OnMessageReceived(IMessage message);

        protected abstract ValueTask OnDisconnected();
    }
}