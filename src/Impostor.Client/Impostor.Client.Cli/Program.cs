using System;
using System.Threading.Tasks;
using Impostor.Client.Shared;
using Impostor.Client.Shared.Events;

namespace Impostor.Client.Cli
{
    internal static class Program
    {
        private static readonly AmongUsModifier _modifier = new AmongUsModifier();

        /// <param name="address">IP Address of the server, will prompt if not specified</param>
        /// <param name="name">Name for server region</param>
        private static Task Main(string address = null, string name = AmongUsModifier.DefaultRegionName)
        {
            _modifier.RegionName = name;
            _modifier.Error += ModifierOnError;
            _modifier.Saved += ModifierOnSaved;

            Console.WriteLine("Welcome to Impostor");

            if (_modifier.TryLoadRegionInfo(out var regionInfo))
            {
                Console.WriteLine($"Currently selected region: {regionInfo.Name} ({regionInfo.Ping}, {regionInfo.Servers.Count} server(s))");
            }

            if (address != null)
            {
                return _modifier.SaveIpAsync(address);
            }

            return PromptAsync();
        }

        private static void ModifierOnSaved(object sender, SavedEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("The IP Address was saved, please (re)start Among Us.");
            Console.ResetColor();
        }

        private static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static void ModifierOnError(object sender, ErrorEventArgs e)
        {
            WriteError(e.Message);
        }

        private static async Task PromptAsync()
        {
            Console.WriteLine("Please enter in the IP Address of the server you would like to use for Among Us");
            Console.WriteLine("If you want to stop playing on the server, simply select another region");

            while (true)
            {
                Console.Write("> ");

                if (await _modifier.SaveIpAsync(Console.ReadLine()))
                {
                    return;
                }
            }
        }
    }
}