using System;
using Impostor.Api.Innersloth;

namespace Impostor.Server.Account
{
    public sealed record AccountSession(string Id, string Puid, string? FriendCode, string Name, int ClientVersion, Language Language, DateTime ExpiresAt);
}
