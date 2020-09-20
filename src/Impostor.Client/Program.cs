using System;
using System.IO;
using Impostor.Shared.Innersloth;

namespace Impostor.Client
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "..\\LocalLow");
            var regionFile = Path.Combine(appData, "Innersloth", "Among Us", "regionInfo.dat");
            var region = new RegionInfo("Private", "192.168.1.211", new []
            {
                new ServerInfo("Private-Master-1", "192.168.1.211", 22023)
            });
            
            using (var file = File.Open(regionFile, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(file))
            {
                region.Serialize(writer);
            }
        }
    }
}