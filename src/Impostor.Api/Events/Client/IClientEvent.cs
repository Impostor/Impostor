using Impostor.Api.Net;

namespace Impostor.Api.Events.Client
{
    public interface IClientEvent : IEvent
    {
        IHazelConnection Connection { get; }
    }
}
