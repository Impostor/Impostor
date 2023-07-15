namespace Impostor.Api.Config
{
    public class ModConfig
    {
        public const string Section = "Mod";

        public bool Enabled { get; set; } = false;

        public bool AllowModVersion { get; set; } = false;
    }
}
