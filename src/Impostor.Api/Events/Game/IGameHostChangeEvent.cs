using Impostor.Api.Net;

namespace Impostor.Api.Events
{
    public interface IGameHostChangeEvent : IGameEvent
    {
        IClientPlayer Host { get; }
    }
}
