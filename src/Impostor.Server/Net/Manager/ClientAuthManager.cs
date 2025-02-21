using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Impostor.Api.Innersloth;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Manager;

public class ClientAuthManager(ILogger<ClientAuthManager> logger)
{
    public uint LastId { get; private set; }

    private List<AuthInfo> AuthInfos { get; } = [];

    private uint GetNextId()
    {
        LastId++;
        return LastId;
    }

    public void RemoveAuthInfo(uint id)
    {
        AuthInfos.RemoveAll(n => n.LastId == id);
        logger.LogInformation("Remove authInfo:{id}", id);
    }

    public uint CreateAuthInfo(GameVersion version, Platforms platform, string matchmakerToken, string friendCode)
    {
        var id = GetNextId();
        if (TryGetAuthInfo(n => n.MatchmakerToken == matchmakerToken || n.FriendCode == friendCode, out var authInfo))
        {
            authInfo.LastId = id;
            authInfo.Version = version;
            authInfo.Platform = platform;
            authInfo.MatchmakerToken = matchmakerToken;
            authInfo.FriendCode = friendCode;
            return id;
        }

        var info = new AuthInfo(id, version, platform, matchmakerToken, friendCode);
        AuthInfos.Add(info);
        logger.LogInformation("Create authInfo:{id} {version}, {platform}, {token}, {code}", id, version, platform,
            matchmakerToken, friendCode);
        return id;
    }

    private bool TryGetAuthInfo(Func<AuthInfo, bool> predicate, [MaybeNullWhen(false)] out AuthInfo info)
    {
        var find = AuthInfos.FirstOrDefault(predicate);
        info = find;
        return find != null;
    }

    public bool TryGetAuthInfo(uint id, [MaybeNullWhen(false)] out AuthInfo info)
    {
        return TryGetAuthInfo(n => n.LastId == LastId, out info);
    }

    public class AuthInfo(
        uint lastId,
        GameVersion version,
        Platforms platform,
        string matchmakerToken,
        string friendCode)
    {
        public uint LastId { get; set; } = lastId;
        public GameVersion Version { get; set; } = version;
        public Platforms Platform { get; set; } = platform;
        public string MatchmakerToken { get; set; } = matchmakerToken;
        public string FriendCode { get; set; } = friendCode;
    }
}
