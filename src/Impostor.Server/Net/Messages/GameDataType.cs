namespace Impostor.Server.Net.Messages
{
    internal static class GameDataType
    {
        public const byte Data = 1;
        public const byte Rpc = 2;
        public const byte Spawn = 4;
        public const byte Despawn = 5;
        public const byte SceneChange = 6;
        public const byte Ready = 7;
        public const byte ChangeSettings = 8;
    }
}