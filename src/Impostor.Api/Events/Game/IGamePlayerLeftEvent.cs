using Impostor.Api.Net;

namespace Impostor.Api.Events
{
    public interface IGamePlayerLeftEvent : IGameEvent
    {
        IClientPlayer Player { get; }

        bool IsBan { get; }
    }
}
