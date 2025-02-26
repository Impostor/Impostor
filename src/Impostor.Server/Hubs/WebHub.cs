using System;
using System.Threading.Tasks;
using Impostor.Api.Config;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Serilog.Core;
using Serilog.Events;

namespace Impostor.Server.Hubs;

internal sealed class WebHub : BaseTokenHub
{
    internal WebHub(IOptions<ExtensionServerConfig> config, WebSink webSink) : base(config)
    {
        webSink.OnMessage += async message =>
        {
            await SendLogAsync(message);
        };
    }

    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }

    internal ValueTask HandleCommandAsync(string command)
    {
        return ValueTask.CompletedTask;
    }

    internal async ValueTask SendLogAsync(string message)
    {
        foreach (var connection in Connections)
        {
            if (!connection.HasAuthorized)
            {
                return;
            }

            await connection.Client.SendAsync("SendLog", message);
        }
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
