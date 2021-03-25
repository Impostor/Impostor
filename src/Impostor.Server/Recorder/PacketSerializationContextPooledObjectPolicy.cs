using Microsoft.Extensions.ObjectPool;

namespace Impostor.Server.Recorder
{
    public class PacketSerializationContextPooledObjectPolicy : IPooledObjectPolicy<PacketSerializationContext>
    {
        public PacketSerializationContext Create()
        {
            return new PacketSerializationContext();
        }

        public bool Return(PacketSerializationContext obj)
        {
            obj.Reset();
            return true;
        }
    }
}
