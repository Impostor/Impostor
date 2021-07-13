namespace Impostor.Server.Config
{
    public static class DisconnectMessages
    {
        public const string Error = "There was an internal server error. " +
                                    "Check the server console for more information. " +
                                    "Please report the issue on the Impostor GitHub if it keeps happening.";

        public const string Destroyed = "The game you tried to join is being destroyed. " +
                                        "Please create a new game.";

        public const string NotImplemented = "Game listing has not been implemented in Impostor yet for servers " +
                                             "running in server redirection mode.";

        public const string UsernameLength = "Your username is too long, please make it shorter.";

        public const string UsernameIllegalCharacters = "Your username contains illegal characters, please remove them.";

        public const string VersionClientTooOld = "Please update your game to play on this server.";

        public const string VersionServerTooOld = "Your client is too new, please update your Impostor server to play.";

        public const string VersionUnsupported = "Your client version is unsupported, please update your Game and/or Impostor server.";
    }
}
