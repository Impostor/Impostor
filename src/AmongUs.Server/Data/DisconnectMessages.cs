namespace AmongUs.Server.Data
{
    public static class DisconnectMessages
    {
        public const string Error = "There was an internal server error. " +
                                    "Check the server console for more information. " +
                                    "Please report the issue on the AmongUsServer GitHub if it keeps happening.";

        public const string Destroyed = "The game you tried to join is being destroyed. " +
                                        "Please create a new game.";
    }
}