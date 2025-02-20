using Impostor.Api.Config;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Hubs;

public class WebHub(IOptions<ExtensionServerConfig> config) : BaseTokenHub
{
    public override string Token { get; protected set; } = config.Value.SignalRToken;
}
