using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api.Config;
using Impostor.Api.Innersloth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Account
{
    /// <summary>
    /// Binds the HTTP token session to the UDP connection without looking at addresses.
    /// Joiners get a one-off alias code in the listing, which comes back in the join packet.
    /// Hosts (and joins that only know the real code) hold a ticket that is unique per name.
    /// </summary>
    public sealed class GameCodeAuthManager : IDisposable
    {
        public const string HashPrefix = "impostor_";

        private const int HostKey = 0;
        private const int AliasAttempts = 16;

        private static readonly TimeSpan SessionLifetime = TimeSpan.FromHours(1);
        private static readonly TimeSpan HostLifetime = TimeSpan.FromSeconds(15);

        private readonly ILogger<GameCodeAuthManager> _logger;
        private readonly AccountConfig _config;
        private readonly IFriendCodeResolver? _friendCodes;
        private readonly TimeSpan _codeLifetime;

        private readonly ConcurrentDictionary<string, AccountSession> _sessions = new();
        private readonly ConcurrentDictionary<int, Ticket> _aliases = new();
        private readonly ConcurrentDictionary<(string Session, int Target), int> _aliasOwners = new();
        private readonly ConcurrentDictionary<int, List<Ticket>> _waiting = new();
        private readonly Timer _sweeper;

        public GameCodeAuthManager(ILogger<GameCodeAuthManager> logger, IOptions<AccountConfig> config, IFriendCodeResolver? friendCodes = null)
        {
            _logger = logger;
            _config = config.Value;
            _friendCodes = friendCodes;
            _codeLifetime = TimeSpan.FromSeconds(Math.Max(5, _config.GameCodeLifetimeSeconds));
            _sweeper = new Timer(_ => Sweep(), null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        }

        public bool Enabled => _config.Enabled;

        public bool RejectUnmatched => _config.RejectUnmatched;

        /// <summary>
        /// Stores a session and returns the hash to put into the token.
        /// </summary>
        /// <param name="puid">Product user id from the token request.</param>
        /// <param name="name">Player name from the token request.</param>
        /// <param name="eosToken">Login token of the player, kept to look up the friend code later.</param>
        /// <returns>The token hash.</returns>
        public string CreateSession(string puid, string name, string? eosToken)
        {
            var id = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
            _sessions[id] = new AccountSession(id, puid, name, eosToken, DateTime.UtcNow + SessionLifetime);
            return HashPrefix + id;
        }

        public AccountSession? GetSession(string? hash)
        {
            if (hash == null || !hash.StartsWith(HashPrefix, StringComparison.Ordinal))
            {
                return null;
            }

            if (!_sessions.TryGetValue(hash[HashPrefix.Length..], out var session))
            {
                return null;
            }

            return session.ExpiresAt > DateTime.UtcNow ? session : null;
        }

        public ValueTask<string?> ResolveFriendCodeAsync(AccountSession session)
        {
            return _friendCodes == null
                ? new ValueTask<string?>((string?)null)
                : _friendCodes.ResolveAsync(session.Puid, session.EosToken);
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

            for (var i = 0; i < AliasAttempts; i++)
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

            _logger.LogWarning(
                "[Account] No free temporary game code for {0} ({1}), falling back to a name ticket for game {2}.",
                session.Name,
                session.Puid,
                GameCodeParser.IntToGameName(target));
            TryExpectJoin(session, target);
            return target;
        }

        public bool TryExpectHost(AccountSession session) => Enqueue(HostKey, session, HostKey, HostLifetime < _codeLifetime ? HostLifetime : _codeLifetime);

        public bool TryExpectJoin(AccountSession session, int target) => Enqueue(target, session, target, _codeLifetime);

        public AccountClaim? ClaimHost(string name) => ClaimWaiting(HostKey, name);

        public AccountClaim? ClaimJoin(int code, string name)
        {
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

                return new AccountClaim(ticket.Session, ticket.Target, code);
            }

            return ClaimWaiting(code, name);
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

        private bool Enqueue(int key, AccountSession session, int target, TimeSpan lifetime)
        {
            var list = _waiting.GetOrAdd(key, static _ => new List<Ticket>());
            var now = DateTime.UtcNow;

            lock (list)
            {
                list.RemoveAll(t => t.ExpiresAt <= now);

                foreach (var other in list)
                {
                    if (string.Equals(other.Session.Name, session.Name, StringComparison.Ordinal)
                        && !string.Equals(other.Session.Puid, session.Puid, StringComparison.Ordinal))
                    {
                        _logger.LogWarning(
                            "[Account] {0} ({1}) has to wait, the name is held by another account.",
                            session.Name,
                            session.Puid);
                        return false;
                    }
                }

                list.RemoveAll(t => string.Equals(t.Session.Puid, session.Puid, StringComparison.Ordinal));
                list.Add(new Ticket(session, target, now + lifetime));
                return true;
            }
        }

        private AccountClaim? ClaimWaiting(int key, string name)
        {
            if (!_waiting.TryGetValue(key, out var list))
            {
                return null;
            }

            var now = DateTime.UtcNow;

            lock (list)
            {
                list.RemoveAll(t => t.ExpiresAt <= now);

                var index = list.FindIndex(t => string.Equals(t.Session.Name, name, StringComparison.Ordinal));
                if (index < 0)
                {
                    return null;
                }

                var hit = list[index];
                list.RemoveAt(index);
                return new AccountClaim(hit.Session, hit.Target, 0);
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
