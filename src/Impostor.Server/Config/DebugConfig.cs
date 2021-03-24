using System.Text.Json.Serialization;

namespace Impostor.Server.Config
{
    public class DebugConfig
    {
        public const string Section = "Debug";

        [JsonConstructor]
        public DebugConfig(bool gameRecorderEnabled = false, string? gameRecorderPath = null)
        {
            GameRecorderEnabled = gameRecorderEnabled;
            GameRecorderPath = gameRecorderPath;
        }

        public bool GameRecorderEnabled { get; }

        public string? GameRecorderPath { get; }
    }
}
