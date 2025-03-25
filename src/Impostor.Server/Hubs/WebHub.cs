using System;
using System.Threading.Tasks;
using Impostor.Api.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Serilog.Core;
using Serilog.Events;

namespace Impostor.Server.Hubs;

[Authorize]
internal sealed class WebHub : Hub
{
    internal WebHub(IOptions<ExtensionServerConfig> config, WebSink webSink)
    {
        webSink.OnMessage += async message =>
        {
            await SendLogAsync(message);
        };
    }
    

    internal ValueTask HandleCommandAsync(string command)
    {
        return ValueTask.CompletedTask;
    }

    internal async ValueTask SendLogAsync(string message)
    {
        await Clients.All.SendAsync("OnLog", message);
    }

    internal class WebSink : ILogEventSink
    {
        internal static readonly WebSink Sink = new();
        internal Func<string, Task>? OnMessage { get; set; }

        public void Emit(LogEvent logEvent)
        {
            if (OnMessage == null)
            {
                return;
            }

            var message = logEvent.RenderMessage();
            OnMessage(message);
        }
    }
}
