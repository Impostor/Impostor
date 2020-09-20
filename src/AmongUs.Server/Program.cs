using System;
using System.Net;
using System.Threading;
using AmongUs.Server.Net;
using Serilog;

namespace AmongUs.Server
{
    internal static class Program
    {
        private static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);
        
        private static void Main(string[] args)
        {
            // Listen for CTRL+C.
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                QuitEvent.Set();
            };
            
            // Configure logger.
            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Verbose()
#else
                .MinimumLevel.Information()
#endif
                .WriteTo.Console()
                .CreateLogger();

            // Initialize matchmaker.
            var matchMaker = new Matchmaker(IPAddress.Any, 22023);
            matchMaker.Start();
            Log.Logger.Information("Matchmaker is running on *:22023.");
            QuitEvent.WaitOne();
            Log.Logger.Warning("Matchmaker is shutting down!");
            matchMaker.Stop();
        }
    }
}