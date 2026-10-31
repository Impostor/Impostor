using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Account
{
    internal sealed class InnerslothFriendCodeResolver : IFriendCodeResolver, IDisposable
    {
        private const int PruneThreshold = 5000;

        private readonly ILogger<InnerslothFriendCodeResolver> _logger;
        private readonly AccountConfig _config;
        private readonly HttpClient _http = new();
        private readonly ConcurrentDictionary<string, (string Code, DateTime Expires)> _cache = new();

        public InnerslothFriendCodeResolver(ILogger<InnerslothFriendCodeResolver> logger, IOptions<AccountConfig> config)
        {
            _logger = logger;
            _config = config.Value;
        }

        public async ValueTask<string?> ResolveAsync(string puid, string? bearerToken)
        {
            if (!_config.ResolveFriendCode)
            {
                return null;
            }

            if (string.IsNullOrEmpty(bearerToken))
            {
                return null;
            }

            if (_cache.TryGetValue(puid, out var hit) && hit.Expires > DateTime.UtcNow)
            {
                return hit.Code;
            }

            // The backend answers for whoever owns the token, so it has to belong to the claimed puid.
            if (!TokenBelongsTo(bearerToken, puid))
            {
                _logger.LogWarning("Token does not belong to {Puid}, friend code skipped.", puid);
                return null;
            }

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(Math.Max(1, _config.FriendCodeTimeoutSeconds)));
                using var request = new HttpRequestMessage(HttpMethod.Get, _config.FriendCodeEndpoint);
                request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + bearerToken);
                request.Headers.TryAddWithoutValidation("Accept", "application/vnd.api+json");

                using var response = await _http.SendAsync(request, cts.Token);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Friend code request for {Puid} returned {Status}.", puid, (int)response.StatusCode);
                    return null;
                }

                var code = ParseFriendCode(await response.Content.ReadAsStringAsync(cts.Token));
                if (code == null)
                {
                    _logger.LogWarning("Friend code response for {Puid} had no username.", puid);
                    return null;
                }

                Remember(puid, code);
                return code;
            }
            catch (Exception ex) when (ex is HttpRequestException or OperationCanceledException or JsonException)
            {
                _logger.LogWarning("Friend code request for {Puid} failed: {Message}", puid, ex.Message);
                return null;
            }
        }

        public void Dispose() => _http.Dispose();

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
                using var doc = JsonDocument.Parse(Convert.FromBase64String(payload));

                foreach (var claim in new[] { "sub", "puid" })
                {
                    if (doc.RootElement.TryGetProperty(claim, out var value)
                        && value.ValueKind == JsonValueKind.String
                        && string.Equals(value.GetString(), puid, StringComparison.Ordinal))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex) when (ex is FormatException or JsonException)
            {
            }

            return false;
        }

        private static string? ParseFriendCode(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var node = doc.RootElement;

            if (TryGet(node, "data", out var data))
            {
                node = data;
            }

            if (TryGet(node, "attributes", out var attributes))
            {
                node = attributes;
            }

            if (!TryGet(node, "username", out var username) || !TryGet(node, "discriminator", out var discriminator))
            {
                return null;
            }

            var name = username.ToString();
            var tag = discriminator.ToString();
            return string.IsNullOrEmpty(name) || string.IsNullOrEmpty(tag) ? null : $"{name}#{tag}";
        }

        private static bool TryGet(JsonElement element, string name, out JsonElement value)
        {
            value = default;

            if (element.ValueKind != JsonValueKind.Object)
            {
                return false;
            }

            foreach (var property in element.EnumerateObject())
            {
                if (property.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    value = property.Value;
                    return true;
                }
            }

            return false;
        }

        private void Remember(string puid, string code)
        {
            var now = DateTime.UtcNow;

            if (_cache.Count > PruneThreshold)
            {
                foreach (var (key, entry) in _cache)
                {
                    if (entry.Expires <= now)
                    {
                        _cache.TryRemove(key, out _);
                    }
                }
            }

            _cache[puid] = (code, now.AddMinutes(Math.Max(1, _config.FriendCodeCacheMinutes)));
        }
    }
}
