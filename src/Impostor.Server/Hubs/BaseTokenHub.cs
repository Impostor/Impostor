using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Impostor.Api.Config;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Hubs;

public class BaseTokenHub(IOptions<ExtensionServerConfig> extensionConfig) : Hub
{
    public string Token { get; } = extensionConfig.Value.Token;
    public List<HubConnection> Connections { get; protected set; } = [];

    public override async Task OnConnectedAsync()
    {
        var connection = new HubConnection(Context, Clients.Caller, Context.ConnectionId);
        Connections.Add(connection);
        
        if (Token != "")
        {
            await SetTokenAuthorized(connection);
        }
    }

    public Task AuthorizeAsync(string token)
    {
        if (token != Token)
        {
            return Task.CompletedTask;
        }

        var connection = Connections.FirstOrDefault(n => n.ConnectionId == Context.ConnectionId);
        if (connection == null)
        {
            return Task.CompletedTask;
        }

        connection.HasAuthorized = true;
        return Task.CompletedTask;
    }

    private Task SetTokenAuthorized(HubConnection connection)
    {
        var timer = new Timer(TimeSpan.FromSeconds(30));
        timer.Enabled = true;
        timer.Elapsed += (sender, e) =>
        {
            if (connection.HasAuthorized)
            {
                return;
            }

            connection.Context.Abort();
            Connections.Remove(connection);
        };
        timer.AutoReset = false;
        timer.Start();
        return Task.CompletedTask;
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Connections.RemoveAll(n => n.ConnectionId == Context.ConnectionId || n.Context == Context);
        return base.OnDisconnectedAsync(exception);
    }

    public record HubConnection(HubCallerContext Context, ISingleClientProxy Client, string ConnectionId)
    {
        public bool HasAuthorized { get; set; }
    }
}
