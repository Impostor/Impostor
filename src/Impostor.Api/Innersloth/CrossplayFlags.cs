namespace Impostor.Api.Innersloth;

public enum CrossplayFlags
{
    Unknown = 0,
    StandaloneEpicPC = 1 << 1,
    StandaloneSteamPC = 1 << 2,
    StandaloneMac = 1 << 3,
    StandaloneWin10 = 1 << 4,
    StandaloneItch = 1 << 5,
    IPhone = 1 << 6,
    Android = 1 << 7,
    Switch = 1 << 8,
    Xbox = 1 << 9,
    Playstation = 1 << 10,
    All = int.MaxValue,
}
