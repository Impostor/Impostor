namespace Impostor.Api.Innersloth
{
    public enum GameOverReason : byte
    {
        HumansByVote = 0,
        HumansByTask = 1,
        ImpostorByVote = 2,
        ImpostorByKill = 3,
        ImpostorBySabotage = 4,
        ImpostorDisconnect = 5,
        HumansDisconnect = 6,
    }
}
