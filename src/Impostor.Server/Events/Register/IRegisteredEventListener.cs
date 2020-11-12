using System;
using System.Threading.Tasks;
using Impostor.Api.Events;

namespace Impostor.Server.Events.Register
{
    internal interface IRegisteredEventListener
    {
        Type EventType { get; }

        EventPriority Priority { get; }
        
        EventCallStep CallStep { get; }

        ValueTask InvokeAsync(object eventHandler, object @event, IServiceProvider provider);
    }
}