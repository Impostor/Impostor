using System;

namespace Impostor.Api.Events
{
    public interface ICancellableEvent
    {
        public Action<bool> CancelEvent { get; }
    }
}
