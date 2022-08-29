namespace Impostor.Api.Config
{
    public class AntiCheatConfig
    {
        public const string Section = "AntiCheat";

        public bool Enabled { get; set; } = true;

        public bool BanIpFromGame { get; set; } = true;
    }
}
