using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using Impostor.Api.Config;
using Impostor.Api.Innersloth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Account
{
    /// <summary>
    /// Binds the HTTP token session to the UDP connection without looking at addresses.
    /// Joiners get a one-off alias code in the listing, which comes back in the join packet.
    /// Hosts (and joins that only know the real code) are matched by name, language and version.
    /// </summary>
    public sealed class GameCodeAuthManager : IDisposable
    {
        private const int HostKey = 0;

        private readonly ILogger<GameCodeAuthManager> _logger;
        private readonly AccountConfig _config;
        private readonly TimeSpan _codeLifetime;
        private readonly TimeSpan _sessionLifetime;

        private readonly TimeSpan _hostLifetime;

        private readonly ConcurrentDictionary<string, AccountSession> _sessions = new();
        private readonly ConcurrentDictionary<int, Ticket> _aliases = new();
        private readonly ConcurrentDictionary<(string Session, int Target), int> _aliasOwners = new();
        private readonly ConcurrentDictionary<int, List<Ticket>> _waiting = new();
        private readonly Timer _sweeper;

        public GameCodeAuthManager(ILogger<GameCodeAuthManager> logger, IOptions<AccountConfig> config)
        {
            _logger = logger;
            _config = config.Value;
            _codeLifetime = TimeSpan.FromSeconds(Math.Max(5, _config.GameCodeLifetimeSeconds));
            _sessionLifetime = TimeSpan.FromMinutes(Math.Max(1, _config.SessionLifetimeMinutes));
            _hostLifetime = TimeSpan.FromSeconds(Math.Min(_codeLifetime.TotalSeconds, 30));
            _sweeper = new Timer(_ => Sweep(), null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        }

        public bool Enabled => _config.Enabled;

        public bool RejectUnmatched => _config.RejectUnmatched;

        public TimeSpan SessionLifetime => _sessionLifetime;

        public string CreateSession(string puid, string? friendCode, string name, int clientVersion, Language language)
        {
            var id = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
            _sessions[id] = new AccountSession(id, puid, friendCode, name, clientVersion, language, DateTime.UtcNow + _sessionLifetime);
            return id;
        }

        public AccountSession? GetSession(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            if (!_sessions.TryGetValue(id, out var session))
            {
                return null;
            }

            if (session.ExpiresAt <= DateTime.UtcNow)
            {
                return null;
            }

            return session;
        }

        public bool IsAlias(int code) => _aliases.ContainsKey(code);

        /// <summary>
        /// Returns the code the client should use to join <paramref name="target"/>.
        /// </summary>
        /// <param name="session">Session of the player asking for the game.</param>
        /// <param name="target">Real code of the game.</param>
        /// <param name="inUse">Tells whether a code already belongs to a game.</param>
        /// <returns>A one-off code, or the real code when no free one was found.</returns>
        public int IssueAlias(AccountSession session, int target, Func<int, bool> inUse)
        {
            var now = DateTime.UtcNow;
            var owner = (session.Id, target);

            if (_aliasOwners.TryGetValue(owner, out var current)
                && _aliases.TryGetValue(current, out var live)
                && live.ExpiresAt > now)
            {
                return current;
            }

            for (var i = 0; i < 16; i++)
            {
                var code = GameCodeParser.GenerateCode(6);
                if (inUse(code))
                {
                    continue;
                }

                if (_aliases.TryAdd(code, new Ticket(session, target, now + _codeLifetime)))
                {
                    _aliasOwners[owner] = code;
                    return code;
                }
            }

            // No free alias, fall back to name matching on the real code.
            _logger.LogWarning(
                "[Account] Could not find a free temporary game code for {0} ({1}) and game {2}, falling back to name matching.",
                session.Name,
                session.Puid,
                GameCodeParser.IntToGameName(target));
            ExpectJoin(session, target);
            return target;
        }

        public void ExpectHost(AccountSession session) => Enqueue(HostKey, session, HostKey, _hostLifetime);

        public void ExpectJoin(AccountSession session, int target) => Enqueue(target, session, target);

        public AccountClaim? ClaimHost(string name, Language language, int clientVersion, out bool conflict) => ClaimWaiting(HostKey, name, language, clientVersion, out conflict);

        public AccountClaim? ClaimJoin(int code, string name, Language language, int clientVersion, out bool conflict)
        {
            conflict = false;

            if (_aliases.TryRemove(code, out var ticket))
            {
                _aliasOwners.TryRemove((ticket.Session.Id, ticket.Target), out _);

                if (ticket.ExpiresAt <= DateTime.UtcNow)
                {
                    _logger.LogWarning(
                        "[Account] Temporary game code {0} ({1}) used by {2} had already expired.",
                        GameCodeParser.IntToGameName(code),
                        code,
                        name);
                    return null;
                }

                return new AccountClaim(ticket.Session.Puid, ticket.Session.FriendCode, ticket.Target, code);
            }

            return ClaimWaiting(code, name, language, clientVersion, out conflict);
        }

        // Swaps a temporary code for the real one on an already bound connection.
        public bool TryResolveAlias(int code, string puid, out int target)
        {
            target = code;

            if (!_aliases.TryRemove(code, out var ticket))
            {
                return false;
            }

            _aliasOwners.TryRemove((ticket.Session.Id, ticket.Target), out _);

            if (ticket.ExpiresAt <= DateTime.UtcNow)
            {
                _logger.LogWarning(
                    "[Account] Temporary game code {0} ({1}) had already expired.",
                    GameCodeParser.IntToGameName(code),
                    code);
                return false;
            }

            if (!string.Equals(ticket.Session.Puid, puid, StringComparison.Ordinal))
            {
                _logger.LogWarning(
                    "[Account] Temporary game code {0} ({1}) belongs to another account, not handing it over.",
                    GameCodeParser.IntToGameName(code),
                    code);
                return false;
            }

            target = ticket.Target;
            return true;
        }

        public void Dispose() => _sweeper.Dispose();

        private void Enqueue(int key, AccountSession session, int target, TimeSpan? lifetime = null)
        {
            var list = _waiting.GetOrAdd(key, static _ => new List<Ticket>());

            lock (list)
            {
                list.RemoveAll(t => t.Session.Id == session.Id);
                list.Add(new Ticket(session, target, DateTime.UtcNow + (lifetime ?? _codeLifetime)));
            }
        }

        private AccountClaim? ClaimWaiting(int key, string name, Language language, int clientVersion, out bool conflict)
        {
            conflict = false;

            if (!_waiting.TryGetValue(key, out var list))
            {
                return null;
            }

            lock (list)
            {
                list.RemoveAll(t => t.ExpiresAt <= DateTime.UtcNow);

                var best = -1;
                var bestScore = -1;
                var ambiguous = false;

                for (var i = 0; i < list.Count; i++)
                {
                    var session = list[i].Session;
                    if (!string.Equals(session.Name, name, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    var score = (session.Language == language ? 2 : 0) + (session.ClientVersion == clientVersion ? 1 : 0);
                    if (score > bestScore)
                    {
                        best = i;
                        bestScore = score;
                        ambiguous = false;
                    }
                    else if (score == bestScore)
                    {
                        ambiguous = true;
                    }
                }

                if (ambiguous)
                {
                    string? pendingPuid = null;
                    AccountClaim? sameAccount = null;
                    var tied = new List<int>();

                    for (var i = 0; i < list.Count; i++)
                    {
                        var session = list[i].Session;
                        if (!string.Equals(session.Name, name, StringComparison.Ordinal))
                        {
                            continue;
                        }

                        var score = (session.Language == language ? 2 : 0) + (session.ClientVersion == clientVersion ? 1 : 0);
                        if (score != bestScore)
                        {
                            continue;
                        }

                        tied.Add(i);

                        if (tied.Count == 1)
                        {
                            pendingPuid = session.Puid;
                            sameAccount = new AccountClaim(session.Puid, session.FriendCode, list[i].Target, 0);
                        }
                        else if (!string.Equals(pendingPuid, session.Puid, StringComparison.Ordinal))
                        {
                            conflict = true;
                        }
                    }

                    if (!conflict)
                    {
                        // Same player requested a token several times, keep a single ticket.
                        for (var i = tied.Count - 1; i >= 0; i--)
                        {
                            list.RemoveAt(tied[i]);
                        }

                        return sameAccount;
                    }

                    // Different players share this name, both have to retry shortly.
                    var retryAt = DateTime.UtcNow + TimeSpan.FromSeconds(3);
                    foreach (var i in tied)
                    {
                        list[i] = new Ticket(list[i].Session, list[i].Target, retryAt);
                    }

                    return null;
                }

                if (best < 0)
                {
                    return null;
                }

                var hit = list[best];
                list.RemoveAt(best);
                return new AccountClaim(hit.Session.Puid, hit.Session.FriendCode, hit.Target, 0);
            }
        }

        private void Sweep()
        {
            try
            {
                var now = DateTime.UtcNow;

                foreach (var (id, session) in _sessions)
                {
                    if (session.ExpiresAt <= now)
                    {
                        _sessions.TryRemove(id, out _);
                    }
                }

                foreach (var (code, ticket) in _aliases)
                {
                    if (ticket.ExpiresAt <= now && _aliases.TryRemove(code, out _))
                    {
                        _aliasOwners.TryRemove((ticket.Session.Id, ticket.Target), out _);
                    }
                }

                foreach (var (_, list) in _waiting)
                {
                    lock (list)
                    {
                        list.RemoveAll(t => t.ExpiresAt <= now);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Account cache sweep failed");
            }
        }

        private sealed record Ticket(AccountSession Session, int Target, DateTime ExpiresAt);
    }
}
