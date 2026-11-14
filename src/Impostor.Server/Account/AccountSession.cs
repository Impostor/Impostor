using System;

namespace Impostor.Server.Account
{
    public sealed record AccountSession(string Id, string Puid, string Name, string? EosToken, DateTime ExpiresAt);
}
