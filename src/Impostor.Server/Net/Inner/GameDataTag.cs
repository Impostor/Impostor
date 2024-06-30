namespace Impostor.Server.Net.Inner
{
    public enum GameDataTag : byte
    {
        DataFlag = 1,
        RpcFlag = 2,
        SpawnFlag = 4,
        DespawnFlag = 5,
        SceneChangeFlag = 6,
        ReadyFlag = 7,
        ChangeSettingsFlag = 8,
        ConsoleDeclareClientPlatformFlag = 205,
        PS4RoomRequest = 206,
        XboxDeclareXuid = 207,
    }
}
