using System;
using System.Threading;
using System.Threading.Tasks;
using Agones;
using Impostor.Server.Net.Redirector;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Impostor.Server
{
    public class AgonesHealthCheck : IHostedService, IDisposable
    {
        private static readonly ILogger Logger = Log.ForContext<AgonesHealthCheck>();
        private readonly AgonesSDK _agones;
        private Timer _timer;

        public AgonesHealthCheck(AgonesSDK agones)
        {
            _agones = agones;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            Logger.Information("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var result = _agones.HealthAsync().Result;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            Logger.Information("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}