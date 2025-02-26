using System;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Controllers;

public sealed class WebSocketController(IOptions<ExtensionServerConfig> config) : ControllerBase
{
    private readonly ExtensionServerConfig _config = config.Value;

    [Route("/websocket")]
    public async Task WebSocketAsync()
    {
        if (!_config.EnabledWebSocket)
        {
            return;
        }

        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            return;
        }

        var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        var buffer = new byte[1024 * 4];
        await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
    }
}
