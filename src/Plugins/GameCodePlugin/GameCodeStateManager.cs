using System.Text.RegularExpressions;
using Impostor.Api.Games;
using Microsoft.Extensions.Logging;

namespace GameCodePlugin;

public partial class GameCodeStateManager(ILogger<GameCodeStateManager> logger)
{
    private static readonly Regex Regex = MyRegex();
    internal List<CodeState> _codes = [];

    internal void ReleaseCode(GameCode code)
    {
        var state = _codes.FirstOrDefault(used => used.Code == code);
        if (state == null) return;
        state.Used = false;
    }

    internal GameCode? GetCode()
    {
        var state = _codes.FirstOrDefault(used => !used);
        if (state == null) return null;
        state.Used = true;
        return state.Code;
    }
    
    internal async ValueTask LoadCodeAsync(DirectoryInfo dir)
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

        _codes = hashSet.Select(code => new CodeState(code)).ToList();
    }

    public class CodeState(GameCode code)
    {
        public GameCode Code { get; } = code;
        public bool Used { get; set; }

        public static implicit operator bool(CodeState state) => state.Used;
    }
    

    [GeneratedRegex("^(?:[a-zA-Z]{4}|[a-zA-Z]{6})$")]
    private static partial Regex MyRegex();
}
