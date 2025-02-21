using System;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner;

namespace Impostor.Api.Events;

public interface IGameMessageEvent : IEvent
{
    public bool HasBreak { get; set; }
    public bool IsRpc { get; }
    public byte MessageId { get; }
    public IMessageReader Reader { get; }
    public IInnerNetObject? NetObject { get; }
    public Func<IClientPlayer, IClientPlayer?, bool>? HandleRpc { get; set; }
}
