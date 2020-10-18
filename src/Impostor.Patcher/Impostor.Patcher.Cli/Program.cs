using System;
using System.Threading.Tasks;
using Impostor.Patcher.Shared;
using Impostor.Patcher.Shared.Events;

namespace Impostor.Patcher.Cli
{
    internal static class Program
    {
        private static readonly AmongUsModifier Modifier = new AmongUsModifier();

        /// <param name="address">IP Address of the server, will prompt if not specified</param>
        /// <param name="name">Name for server region</param>
        private static Task Main(string address = null, string name = AmongUsModifier.DefaultRegionName)
        {
            Modifier.RegionName = name;
            Modifier.Error += ModifierOnError;
            Modifier.Saved += ModifierOnSaved;

            Console.WriteLine("Welcome to Impostor");

            if (Modifier.TryLoadRegionInfo(out var regionInfo))
            {
                Console.WriteLine($"Currently selected region: {regionInfo.Name} ({regionInfo.Ping}, {regionInfo.Servers.Count} server(s))");
            }

            if (address != null)
            {
                return Modifier.SaveIpAsync(address);
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

                if (await Modifier.SaveIpAsync(Console.ReadLine()))
                {
                    return;
                }
            }
        }
    }
}