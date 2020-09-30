using System.Threading;
using System.Threading.Tasks;
using Impostor.Server.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Net
{
    internal class MatchmakerService : IHostedService
    {
        private readonly ILogger<MatchmakerService> _logger;
        private readonly ServerConfig _serverConfig;
        private readonly ServerRedirectorConfig _redirectorConfig;
        private readonly Matchmaker _matchmaker;

        public MatchmakerService(
            ILogger<MatchmakerService> logger, 
            IOptions<ServerConfig> serverConfig, 
            IOptions<ServerRedirectorConfig> redirectorConfig,
            Matchmaker matchmaker)
        {
            _logger = logger;
            _serverConfig = serverConfig.Value;
            _redirectorConfig = redirectorConfig.Value;
            _matchmaker = matchmaker;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _matchmaker.Start();
            
            _logger.LogInformation("Matchmaker is listening on {0}:{1}, the public server ip is {2}:{3}.", 
                _matchmaker.EndPoint.Address, 
                _matchmaker.EndPoint.Port,
                _serverConfig.PublicIp, 
                _serverConfig.PublicPort);

            if (_redirectorConfig.Enabled)
            {
                _logger.LogWarning(_redirectorConfig.Master
                    ? "Server redirection is enabled as master, this instance will redirect clients to other nodes."
                    : "Server redirection is enabled as node, this instance will accept clients.");
            }
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Matchmaker is shutting down!");
            _matchmaker.Stop();
            
            return Task.CompletedTask;
        }
    }
}