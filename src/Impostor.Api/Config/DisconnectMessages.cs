using Impostor.Api.Localization;

namespace Impostor.Api.Config
{
    public static class DisconnectMessages
    {
        public static LocalizedMessageKey Error => Key("Error");

        public static LocalizedMessageKey ClientOutdated => Key("ClientOutdated");

        public static LocalizedMessageKey ClientTooNew => Key("ClientTooNew");

        public static LocalizedMessageKey Destroyed => Key("Destroyed");

        public static LocalizedMessageKey UsernameLength => Key("UsernameLength");

        public static LocalizedMessageKey UsernameIllegalCharacters => Key("UsernameIllegalCharacters");

        public static LocalizedMessageKey VersionClientTooOld => Key("VersionClientTooOld");

        public static LocalizedMessageKey VersionServerTooOld => Key("VersionServerTooOld");

        public static LocalizedMessageKey VersionUnsupported => Key("VersionUnsupported");

        public static LocalizedMessageKey UdpMatchmakingUnsupported => Key("UdpMatchmakingUnsupported");

        public static LocalizedMessageKey HostAuthorityUnsupported => Key("HostAuthorityUnsupported");

        public static LocalizedMessageKey InvalidClient => Key("InvalidClient");

        public static LocalizedMessageKey InvalidLimbo => Key("InvalidLimbo");

        public static LocalizedMessageKey UnknownError => Key("UnknownError");

        public static LocalizedMessageKey CheatBanned => Key("CheatBanned");

        public static LocalizedMessageKey CheatKicked => Key("CheatKicked");

        private static LocalizedMessageKey Key(string key) => new("impostor", key);
    }
}
