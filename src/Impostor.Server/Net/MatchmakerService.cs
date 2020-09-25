using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net
{
    public class MatchmakerService : IHostedService
    {
        private readonly ILogger<MatchmakerService> _logger;
        private readonly Matchmaker _matchmaker;

        public MatchmakerService(ILogger<MatchmakerService> logger, Matchmaker matchmaker)
        {
            _logger = logger;
            _matchmaker = matchmaker;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _matchmaker.Start();
            _logger.LogInformation("Matchmaker is running on {0}:{1}.", 
                _matchmaker.EndPoint.Address, 
                _matchmaker.EndPoint.Port);
            
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