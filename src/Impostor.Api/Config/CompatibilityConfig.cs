namespace Impostor.Api.Config
{
    public class CompatibilityConfig
    {
        public const string Section = "Compatibility";

        public bool AllowFutureGameVersions { get; set; } = false;

        public bool AllowHostAuthority { get; set; } = false;

        public bool AllowVersionMixing { get; set; } = false;
    }
}
