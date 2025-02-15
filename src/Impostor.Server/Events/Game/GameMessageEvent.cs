using System;
using System.Threading.Tasks;
using Impostor.Api.Events;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner;

namespace Impostor.Server.Events;

public class GameMessageEvent(bool isRpc, byte messageId, IMessageReader reader) : IGameMessageEvent
{
    public bool HasBreak { get; set; }
    public bool IsRpc { get; } = isRpc;
    public byte MessageId { get; } = messageId;
    public IMessageReader Reader { get; } = reader;

    public GameMessageEvent(IInnerNetObject netObject, byte messageId, IMessageReader reader) : this(true, messageId, reader)
    {
        NetObject = netObject;
    }
    
    public IInnerNetObject? NetObject { get; }
    public Func<IClientPlayer, IClientPlayer?, bool>? HandleRpc { get; set; }
}
