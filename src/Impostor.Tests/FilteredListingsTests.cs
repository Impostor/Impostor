#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Impostor.Api.Config;
using Impostor.Api.Games;
using Impostor.Api.Games.Managers;
using Impostor.Api.Http;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.GameOptions;
using Impostor.Api.Net;
using Impostor.Server.Http;
using Impostor.Server.Net.Manager;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Impostor.Tests;

public sealed class FilteredListingsTests
{
    public static IEnumerable<object[]> VersionCases()
    {
        yield return new object[] { new GameVersion(2026, 7, 20), new GameVersion(2026, 7, 20), false, true };
        yield return new object[] { new GameVersion(2026, 7, 20), new GameVersion(2026, 7, 21), false, true };
        yield return new object[] { new GameVersion(2026, 7, 20, 25), new GameVersion(2026, 7, 21), false, true };
        yield return new object[] { new GameVersion(2026, 7, 20), new GameVersion(2026, 7, 15), false, false };
        yield return new object[] { new GameVersion(2026, 7, 15), new GameVersion(2026, 7, 20), false, false };
        yield return new object[] { new GameVersion(2025, 4, 15), new GameVersion(2025, 7, 15), false, false };
        yield return new object[] { new GameVersion(2025, 7, 15), new GameVersion(2025, 4, 15), false, false };
        yield return new object[] { new GameVersion(2024, 8, 11), new GameVersion(2025, 4, 15), false, false };
        yield return new object[] { new GameVersion(2026, 7, 20), new GameVersion(2026, 7, 15), true, true };
    }

    [Theory]
    [MemberData(nameof(VersionCases))]
    public void FilteredListingsUseClientVersionFromToken(GameVersion hostVersion, GameVersion clientVersion, bool allowVersionMixing, bool expectedListing)
    {
        var client = PropertyStub.Create<IClient>(
            ("GameVersion", hostVersion),
            ("ChatMode", QuickChatModes.FreeChatOrQuickChat),
            ("Name", "Host"),
            ("PlatformSpecificData", null));
        var host = PropertyStub.Create<IClientPlayer>(("Client", client));
        var game = PropertyStub.Create<IGame>(
            ("Host", host),
            ("IsPublic", true),
            ("GameState", GameStates.NotStarted),
            ("PlayerCount", 1),
            ("Options", new NormalGameOptions()),
            ("ModGuid", null),
            ("PublicIp", new IPEndPoint(IPAddress.Loopback, 22023)),
            ("Code", new GameCode(123)),
            ("DisplayName", null));
        var gameManager = PropertyStub.Create<IGameManager>(("Games", new[] { game }));
        var listingManager = new ListingManager(
            gameManager,
            Array.Empty<IListingFilter>(),
            new CompatibilityManager(NullLogger<CompatibilityManager>.Instance),
            Options.Create(new CompatibilityConfig { AllowVersionMixing = allowVersionMixing }));
        var controller = new GamesController(gameManager, listingManager, Options.Create(new ServerConfig()), NullLogger<GamesController>.Instance)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() },
        };
        var token = Assert.IsType<OkObjectResult>(new TokenController().GetToken(new TokenController.TokenRequest
        {
            ProductUserId = "listing-test",
            Username = "Player",
            ClientVersion = clientVersion.Value,
            Language = Language.English,
        }));
        var filter = JsonSerializer.Serialize(new
        {
            FilterSets = new[]
            {
                new
                {
                    GameMode = GameModes.Normal,
                    Filters = new[]
                    {
                        new { OptionType = "languages", Key = "", SubFilterString = JsonSerializer.Serialize(new { AcceptedValues = (uint)GameKeywords.English }) },
                        new { OptionType = "chat", Key = "", SubFilterString = JsonSerializer.Serialize(new { AcceptedValues = 3 }) },
                    },
                },
            },
        });

        var result = Assert.IsType<OkObjectResult>(controller.ShowFilteredLobbies(filter, new AuthenticationHeaderValue("Bearer", Assert.IsType<string>(token.Value))));

        var response = JsonSerializer.SerializeToElement(result.Value);
        Assert.Equal(expectedListing ? 1 : 0, response.GetProperty("games").GetArrayLength());
        Assert.Equal(1, response.GetProperty("metadata").GetProperty("allGamesCount").GetInt32());
        Assert.Equal(expectedListing ? 1 : 0, response.GetProperty("metadata").GetProperty("matchingGamesCount").GetInt32());
        if (expectedListing)
        {
            Assert.Equal(123, response.GetProperty("games")[0].GetProperty("GameId").GetInt32());
        }
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("Basic", "credentials")]
    [InlineData("Bearer", null)]
    [InlineData("Bearer", "not-base64")]
    [InlineData("Bearer", "bnVsbA==")]
    [InlineData("Bearer", "ew==")]
    [InlineData("Bearer", "e30=")]
    public void MissingOrInvalidAuthorizationReturnsMatchmakerResponse(string? scheme, string? parameter)
    {
        var controller = new GamesController(null!, null!, Options.Create(new ServerConfig()), NullLogger<GamesController>.Instance);
        var authorization = scheme == null ? null : new AuthenticationHeaderValue(scheme, parameter);

        var result = Assert.IsType<UnauthorizedObjectResult>(controller.ShowFilteredLobbies("{}", authorization));

        var response = JsonSerializer.SerializeToElement(result.Value);
        var error = Assert.Single(response.GetProperty("Errors").EnumerateArray());
        Assert.Equal((int)DisconnectReason.NotAuthorized, error.GetProperty("Reason").GetInt32());
        Assert.False(string.IsNullOrEmpty(error.GetProperty("Message").GetString()));
    }

    [Fact]
    public async Task MissingAuthorizationHeaderReturnsMatchmakerResponseOverHttp()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Logging.ClearProviders();
        builder.Services.AddControllers().AddApplicationPart(typeof(GamesController).Assembly);
        var gameManager = PropertyStub.Create<IGameManager>(("Games", Array.Empty<IGame>()));
        builder.Services.AddSingleton(gameManager);
        builder.Services.AddSingleton(new ListingManager(
            gameManager,
            Array.Empty<IListingFilter>(),
            new CompatibilityManager(NullLogger<CompatibilityManager>.Instance),
            Options.Create(new CompatibilityConfig())));
        await using var app = builder.Build();
        app.MapControllers();
        await app.StartAsync();
        using var http = new HttpClient { BaseAddress = new Uri(app.Urls.Single()) };

        using var result = await http.GetAsync("/api/games/filtered?filter=%7B%7D");

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        using var response = JsonDocument.Parse(await result.Content.ReadAsStringAsync());
        var error = Assert.Single(response.RootElement.GetProperty("Errors").EnumerateArray());
        Assert.Equal((int)DisconnectReason.NotAuthorized, error.GetProperty("Reason").GetInt32());
        Assert.Equal("Missing or invalid Authorization header", error.GetProperty("Message").GetString());
    }

    public class PropertyStub : DispatchProxy
    {
        private Dictionary<string, object?> _properties = new();

        public static T Create<T>(params (string Name, object? Value)[] properties)
            where T : class
        {
            var proxy = Create<T, PropertyStub>();
            ((PropertyStub)(object)proxy)._properties = properties.ToDictionary(p => "get_" + p.Name, p => p.Value);
            return proxy;
        }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod != null && _properties.TryGetValue(targetMethod.Name, out var value))
            {
                return value;
            }

            throw new InvalidOperationException($"Unexpected call: {targetMethod?.Name}");
        }
    }
}
