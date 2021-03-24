namespace Impostor.Server.Config
{
    public static class DisconnectMessages
    {
        public const string Error = "There was an internal server error. " +
                                    "Check the server console for more information. " +
                                    "Please report the issue on the AmongUsServer GitHub if it keeps happening.";

        public const string Destroyed = "The game you tried to join is being destroyed. " +
                                        "Please create a new game.";

        public const string NotImplemented = "Game listing has not been implemented in Impostor yet for servers " +
                                             "running in server redirection mode.";

        public const string UsernameLength = "Your username is too long, please make it shorter.";

        public const string UsernameIllegalCharacters = "Your username contains illegal characters, please remove them.";
    }
}
