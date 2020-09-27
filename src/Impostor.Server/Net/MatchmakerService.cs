using System.Threading;
using System.Threading.Tasks;
using Agones;
using Grpc.Core;
using Impostor.Server.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace Impostor.Server.Net
{
    internal class MatchmakerService : IHostedService
    {
        private readonly ILogger<MatchmakerService> _logger;
        private readonly ServerConfig _serverConfig;
        private readonly ServerRedirectorConfig _redirectorConfig;
        private readonly Matchmaker _matchmaker;
        private readonly AgonesSDK _agones;

        public MatchmakerService(
            ILogger<MatchmakerService> logger, 
            IOptions<ServerConfig> serverConfig, 
            IOptions<ServerRedirectorConfig> redirectorConfig,
            Matchmaker matchmaker,
            AgonesSDK agones)
        {
            _logger = logger;
            _serverConfig = serverConfig.Value;
            _redirectorConfig = redirectorConfig.Value;
            _matchmaker = matchmaker;
            _agones = agones;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _matchmaker.Start();
            if (_redirectorConfig.Enabled && _redirectorConfig.Master == false)
            {
                var result = _agones.ReadyAsync().Result;
                if (result.StatusCode != StatusCode.OK)
                {
                    Log.Fatal("failed to set agones ready state");
                }
            }
            
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