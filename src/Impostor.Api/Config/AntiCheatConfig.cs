namespace Impostor.Api.Config
{
    public class AntiCheatConfig
    {
        public const string Section = "AntiCheat";

        public bool Enabled { get; set; } = true;

        public bool BanIpFromGame { get; set; } = true;

        public CheatingHostMode AllowCheatingHosts { get; set; } = CheatingHostMode.Never;

        public CheatingHostMode AllowHostOnlyExtensions { get; set; } = CheatingHostMode.IfRequested;

        public bool EnableGameFlowChecks { get; set; } = true;

        public bool EnableMustBeHostChecks { get; set; } = true;

        public bool EnableInvalidObjectChecks { get; set; } = true;

        public bool EnableColorLimitChecks { get; set; } = true;

        public bool EnableNameLimitChecks { get; set; } = true;

        public bool EnableItemLimitChecks { get; set; } = true;

        public bool EnableOwnershipChecks { get; set; } = true;

        public bool EnableRoleChecks { get; set; } = true;

        public bool EnableTargetChecks { get; set; } = true;

        public bool EnableMurderChecks { get; set; } = true;

        public bool EnableSabotageChecks { get; set; } = true;

        public bool EnableMeetingChecks { get; set; } = true;

        public bool EnableVotingChecks { get; set; } = true;

        public bool EnableRateLimits { get; set; } = true;

        public int RpcRateLimitPerSecond { get; set; } = 20;

        public bool ForbidProtocolExtensions { get; set; } = true;

        public bool EnablePacketSizeChecks { get; set; } = true;

        public int PacketSizeLimit { get; set; } = 1203;
    }
}
