using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Account
{
    internal sealed class InnerslothFriendCodeResolver : IFriendCodeResolver, IDisposable
    {
        private const string Endpoint = "https://backend.innersloth.com/api/user/username";

        private readonly ILogger<InnerslothFriendCodeResolver> _logger;
        private readonly AccountConfig _config;
        private readonly HttpClient _http = new();
        private readonly ConcurrentDictionary<string, (string Code, DateTime Expires)> _cache = new();
        private readonly ConcurrentDictionary<string, Lazy<Task<string?>>> _pending = new();
        private readonly Timer _cleaner;

        public InnerslothFriendCodeResolver(ILogger<InnerslothFriendCodeResolver> logger, IOptions<AccountConfig> config)
        {
            _logger = logger;
            _config = config.Value;
            _cleaner = new Timer(_ => Cleanup(), null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        }

        public async ValueTask<string?> ResolveAsync(string puid, string? bearerToken)
        {
            if (!_config.ResolveFriendCode || string.IsNullOrEmpty(bearerToken))
            {
                return null;
            }

            if (_cache.TryGetValue(puid, out var hit) && hit.Expires > DateTime.UtcNow)
            {
                return hit.Code;
            }

            // The backend answers for the token owner, so it must match the claimed puid.
            if (!TokenBelongsTo(bearerToken, puid))
            {
                _logger.LogWarning("Token does not belong to {Puid}, friend code skipped.", puid);
                return null;
            }

            // One request per player at a time.
            var lazy = _pending.GetOrAdd(puid, _ => new Lazy<Task<string?>>(() => FetchAsync(puid, bearerToken)));

            try
            {
                return await lazy.Value;
            }
            finally
            {
                _pending.TryRemove(puid, out _);
            }
        }

        public void Dispose()
        {
            _cleaner.Dispose();
            _http.Dispose();
        }

        private static bool TokenBelongsTo(string token, string puid)
        {
            var parts = token.Split('.');
            if (parts.Length < 2)
            {
                return false;
            }

            var payload = parts[1].Replace('-', '+').Replace('_', '/');
            payload = payload.PadRight(payload.Length + ((4 - (payload.Length % 4)) % 4), '=');

            try
            {
                var claims = JsonSerializer.Deserialize<TokenClaims>(Convert.FromBase64String(payload));
                return string.Equals(claims?.Subject, puid, StringComparison.Ordinal)
                    || string.Equals(claims?.ProductUserId, puid, StringComparison.Ordinal);
            }
            catch (Exception ex) when (ex is FormatException or JsonException)
            {
                return false;
            }
        }

        private async Task<string?> FetchAsync(string puid, string bearerToken)
        {
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(Math.Max(1, _config.FriendCodeTimeoutSeconds)));
                using var request = new HttpRequestMessage(HttpMethod.Get, Endpoint);
                request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + bearerToken);
                request.Headers.TryAddWithoutValidation("Accept", "application/vnd.api+json");

                using var response = await _http.SendAsync(request, cts.Token);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Friend code request for {Puid} returned {Status}.", puid, (int)response.StatusCode);
                    return null;
                }

                await using var body = await response.Content.ReadAsStreamAsync(cts.Token);
                var attributes = (await JsonSerializer.DeserializeAsync<UsernameResponse>(body, cancellationToken: cts.Token))?.Data?.Attributes;

                if (string.IsNullOrEmpty(attributes?.Username) || string.IsNullOrEmpty(attributes.Discriminator))
                {
                    _logger.LogWarning("Friend code response for {Puid} had no username.", puid);
                    return null;
                }

                var code = $"{attributes.Username}#{attributes.Discriminator}";
                _cache[puid] = (code, DateTime.UtcNow.AddMinutes(Math.Max(1, _config.FriendCodeCacheMinutes)));
                return code;
            }
            catch (Exception ex) when (ex is HttpRequestException or OperationCanceledException or JsonException)
            {
                _logger.LogWarning("Friend code request for {Puid} failed: {Message}", puid, ex.Message);
                return null;
            }
        }

        private void Cleanup()
        {
            var now = DateTime.UtcNow;

            foreach (var (puid, entry) in _cache)
            {
                if (entry.Expires <= now)
                {
                    _cache.TryRemove(puid, out _);
                }
            }
        }

        private sealed class TokenClaims
        {
            [JsonPropertyName("sub")]
            public string? Subject { get; init; }

            [JsonPropertyName("puid")]
            public string? ProductUserId { get; init; }
        }

        private sealed class UsernameResponse
        {
            [JsonPropertyName("data")]
            public UsernameData? Data { get; init; }
        }

        private sealed class UsernameData
        {
            [JsonPropertyName("attributes")]
            public UsernameAttributes? Attributes { get; init; }
        }

        private sealed class UsernameAttributes
        {
            [JsonPropertyName("username")]
            public string? Username { get; init; }

            [JsonPropertyName("discriminator")]
            public string? Discriminator { get; init; }
        }
    }
}
