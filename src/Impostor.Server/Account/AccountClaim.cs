namespace Impostor.Server.Account
{
    public sealed record AccountClaim(AccountSession Session, int Target, int Alias);
}
