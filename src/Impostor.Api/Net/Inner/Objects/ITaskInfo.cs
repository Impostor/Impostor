using Impostor.Api.Net.Messages;

namespace Impostor.Api.Net.Inner.Objects
{
    public interface ITaskInfo
    {
        uint Id { get; }
        
        bool Complete { get; }

        void Serialize(IMessageWriter writer);

        void Deserialize(IMessageReader reader);
    }
}