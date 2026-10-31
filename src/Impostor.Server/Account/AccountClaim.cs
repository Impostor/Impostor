namespace Impostor.Server.Account
{
    public sealed record AccountClaim(string Puid, string? FriendCode, int Target, int Alias);
}
