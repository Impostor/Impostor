using Impostor.Api.Utils;

namespace Impostor.Server.Utils;

public class ServerEnvironment : IServerEnvironment
{
    public bool IsReplay { get; init; }
    public string Version { get; } = DotnetUtils.Version;
}
