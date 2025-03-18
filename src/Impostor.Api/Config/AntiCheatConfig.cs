namespace Impostor.Api.Config
{
    public class AntiCheatConfig
    {
        public const string Section = "AntiCheat";

        public bool Enabled { get; set; } = true;

        public bool BanIpFromGame { get; set; } = true;

        public CheatingHostMode AllowCheatingHosts { get; set; } = CheatingHostMode.Never;

        public bool EnableGameFlowChecks { get; set; } = true;

        public bool EnableMustBeHostChecks { get; set; } = true;

        public bool EnableInvalidObjectChecks { get; set; } = true;

        public bool EnableColorLimitChecks { get; set; } = true;

        public bool EnableNameLimitChecks { get; set; } = true;

        public bool EnableOwnershipChecks { get; set; } = true;

        public bool EnableRoleChecks { get; set; } = true;

        public bool EnableTargetChecks { get; set; } = true;

        public bool ForbidProtocolExtensions { get; set; } = true;
    }
}
