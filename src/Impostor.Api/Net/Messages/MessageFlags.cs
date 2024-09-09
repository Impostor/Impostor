using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Impostor.Api.Net.Messages
{
    public static class MessageFlags
    {
        public const byte HostGame = 0;
        public const byte JoinGame = 1;
        public const byte StartGame = 2;
        public const byte RemoveGame = 3;
        public const byte RemovePlayer = 4;
        public const byte GameData = 5;
        public const byte GameDataTo = 6;
        public const byte JoinedGame = 7;
        public const byte EndGame = 8;
        public const byte AlterGame = 10;
        public const byte KickPlayer = 11;
        public const byte WaitForHost = 12;
        public const byte Redirect = 13;
        public const byte ReselectServer = 14;
        public const byte GetGameListV2 = 16;
        public const byte ReportPlayer = 17;
        public const byte QuickMatch = 18;
        public const byte QuickMatchHost = 19;
        public const byte SetGameSession = 20;
        public const byte SetActivePodType = 21;
        public const byte QueryPlatformIds = 22;
        public const byte QueryLobbyInfo = 23;
        public const byte EndGameHostMigration = 24;

        private static readonly Dictionary<byte, string> FlagCache;

        static MessageFlags()
        {
            FlagCache = typeof(MessageFlags)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                .ToDictionary(x => (byte)x.GetValue(null)!, y => y.Name);
        }

        public static string FlagToString(byte flag)
        {
            return FlagCache.TryGetValue(flag, out var res) ? res : $"Unknown Flag {flag}";
        }
    }
}
