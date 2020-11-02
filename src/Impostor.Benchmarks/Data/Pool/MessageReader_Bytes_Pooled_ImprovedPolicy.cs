using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;

namespace Impostor.Benchmarks.Data.Pool
{
    public class MessageReader_Bytes_Pooled_ImprovedPolicy : IPooledObjectPolicy<MessageReader_Bytes_Pooled_Improved>
    {
        private readonly IServiceProvider _serviceProvider;

        public MessageReader_Bytes_Pooled_ImprovedPolicy(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public MessageReader_Bytes_Pooled_Improved Create()
        {
            return new MessageReader_Bytes_Pooled_Improved(_serviceProvider.GetRequiredService<ObjectPool<MessageReader_Bytes_Pooled_Improved>>());
        }

        public bool Return(MessageReader_Bytes_Pooled_Improved obj)
        {
            return true;
        }
    }
}
