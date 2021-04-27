using Impostor.Api.Utils;

namespace Impostor.Server.Utils
{
    public class ServerEnvironment : IServerEnvironment
    {
        public string Version { get; } = DotnetUtils.Version;

        public bool IsReplay { get; init; }
    }
}
