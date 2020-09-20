namespace AmongUs.Shared.Innersloth.Data
{
    public enum RequestFlag : byte
    {
        HostGame = 0,
        JoinGame = 1,
        StartGame = 2,
        RemoveGame = 3,
        RemovePlayer = 4,
        GameData = 5,
        GameDataTo = 6,
        JoinedGame = 7,
        EndGame = 8,
        GetGameList = 9,
        AlterGame = 10,
        KickPlayer = 11,
        WaitForHost = 12,
        Redirect = 13,
        ReselectServer = 14,
        GetGameListV2 = 16,
    }
}