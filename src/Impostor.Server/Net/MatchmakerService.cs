using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api.Config;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Net;

internal class MatchmakerService(
    ILogger<MatchmakerService> logger,
    IOptions<ServerConfig> serverConfig,
    Matchmaker matchmaker)
    : IHostedService
{
    private readonly ServerConfig _serverConfig = serverConfig.Value;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var endpoint = new IPEndPoint(IPAddress.Parse(_serverConfig.ResolveListenIp()), _serverConfig.ListenPort);

        await matchmaker.StartAsync(endpoint);

        logger.LogInformation(
            "Matchmaker is listening on {0}:{1}, the public server ip is {2}:{3}.",
            endpoint.Address,
            endpoint.Port,
            _serverConfig.ResolvePublicIp(),
            _serverConfig.PublicPort);

        // NOTE: If this warning annoys you, set your PublicIp to "localhost"
        if (_serverConfig.PublicIp == "127.0.0.1")
        {
            logger.LogWarning("Your PublicIp is set to the default value of 127.0.0.1.");
            logger.LogWarning(
                "To allow people on other devices to connect to your server, change this value to your Public IP address");
            logger.LogWarning(
                "For more info on how to do this see https://github.com/Impostor/Impostor/blob/master/docs/Server-configuration.md");
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogWarning("Matchmaker is shutting down!");
        await matchmaker.StopAsync();
    }
}
