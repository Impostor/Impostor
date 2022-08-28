namespace Impostor.Server.Config
{
    public class CompatibilityConfig
    {
        public const string Section = "Compatibility";

        public bool AllowVersionMixing { get; set; } = false;
    }
}
