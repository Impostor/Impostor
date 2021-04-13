namespace Impostor.Server.Net.Inner
{
    public static class GameDataTag
    {
        public const byte DataFlag = 1;
        public const byte RpcFlag = 2;
        public const byte SpawnFlag = 4;
        public const byte DespawnFlag = 5;
        public const byte SceneChangeFlag = 6;
        public const byte ReadyFlag = 7;
        public const byte ChangeSettingsFlag = 8;
        public const byte ConsoleDeclareClientPlatformFlag = 205;
        public const byte PS4RoomRequest = 206;
    }
}
