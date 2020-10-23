using Impostor.Api.Net.Messages;

namespace Impostor.Api.Innersloth.Net.Objects.Systems
{
    public interface ISystemType
    {
        void Serialize(IMessageWriter writer, bool initialState);

        void Deserialize(IMessageReader reader, bool initialState);
    }
}