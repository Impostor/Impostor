using System.Text.RegularExpressions;
using Impostor.Api.Events;
using Impostor.Api.Games;
using Impostor.Api.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GameCodePlugin;

[ImpostorPlugin("GameCodePlugin.Impostor.Next")]
public sealed partial class GameCodePlugin(IHostEnvironment env, ILogger<GameCodePlugin> logger) : IPlugin, IPluginStartup
{
    internal static List<CodeState> Codes = [];
    
    public async ValueTask EnableAsync()
    {
        logger.LogInformation("GameCodePlugin enabled!");
        var root = env.ContentRootPath;
        try
        {
            await LoadCodeAsync(new DirectoryInfo(Path.Combine(root, "GameCode")));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to load game code");
        }
    }

    private static readonly Regex Regex = MyRegex();
    private async ValueTask LoadCodeAsync(DirectoryInfo dir)
    {
        if (!dir.Exists)
        {
            dir.Create();
            return;
        }

        var hashSet = new HashSet<GameCode>();
        foreach (var file in dir.GetFiles("*.txt", SearchOption.AllDirectories))
        {
            await using var stream = file.OpenRead();
            using var reader = new StreamReader(stream);
            var code = 0;
            while (true)
            {
                var line = await reader.ReadLineAsync();
                if (line == null)
                    break;

                var trim = line.Trim();
                if (!Regex.IsMatch(trim))
                    continue;

                hashSet.Add(GameCode.From(trim.ToUpper()));
                code++;
            }
            logger.LogInformation("Load {code} codes from {file}", code, file.Name);
        }

        Codes = hashSet.Select(code => new CodeState(code)).ToList();
    }

    public class CodeState(GameCode code)
    {
        public GameCode Code { get; } = code;
        public bool Used { get; set; }

        public static implicit operator bool(CodeState state) => state.Used;
    }
    

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IEventListener, GameCodeEventListener>();
    }

    [GeneratedRegex("^(?:[a-zA-Z]{4}|[a-zA-Z]{6})$")]
    private static partial Regex MyRegex();
}
