using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;

namespace Impostor.Hazel
{
    public class MessageReaderPolicy : IPooledObjectPolicy<MessageReader>
    {
        private readonly IServiceProvider _serviceProvider;

        public MessageReaderPolicy(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public MessageReader Create()
        {
            return new MessageReader(_serviceProvider.GetRequiredService<ObjectPool<MessageReader>>());
        }

        public bool Return(MessageReader obj)
        {
            obj.Reset();
            return true;
        }
    }
}
