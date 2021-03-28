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
        public const byte GetGameList = 9;
        public const byte GetGameListV2 = 16;
    }
}
