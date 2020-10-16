using Impostor.Server.Net.Messages;

namespace Impostor.Server.GameData
{
    public abstract class InnerNetObject : GameObject
    {
        public uint NetId { get; set; }

        public int OwnerId { get; set; }

        public SpawnFlags SpawnFlags { get; internal set; }

        public abstract void HandleRpc(byte callId, IMessageReader reader);

        public abstract bool Serialize(IMessageWriter writer, bool initialState);

        public abstract void Deserialize(IMessageReader reader, bool initialState);
    }
}