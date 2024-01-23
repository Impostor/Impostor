namespace Impostor.Api.Config
{
    public class AntiCheatConfig
    {
        public const string Section = "AntiCheat";

        public bool Enabled { get; set; } = true;

        public bool AllowProtocolExtensions { get; set; } = false;

        public bool EnableGameFlowChecks { get; set; } = true;

        public bool EnableHostPrivilegeChecks { get; set; } = true;

        public bool EnableLimitChecks { get; set; } = true;

        public bool EnableOwnershipChecks { get; set; } = true;

        public bool EnableRoleChecks { get; set; } = true;

        public bool EnableTargetChecks { get; set; } = true;

        public bool ExemptHost { get; set; } = true;

        public bool BanIpFromGame { get; set; } = true;
    }
}
