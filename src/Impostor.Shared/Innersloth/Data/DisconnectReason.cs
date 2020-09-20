namespace Impostor.Shared.Innersloth.Data
{
    public enum DisconnectReason
    {
        ExitGame = 0,
        Destroy = 16,
        // The game you tried to join is full.
        // Check with the host to see if you can join next round.
        GameFull = 1,
        // The game you tried to join already started.
        // Check with the host to see if you can join next round.
        GameStarted = 2,
        // Could not find the game you're looking for.
        GameMissing = 3,
        IncorrectGame = 18,
        // For these a message can be given, specifying an empty message shows
        // "An unknown error disconnected you from the server."
        CustomMessage1 = 4,
        Custom = 8,
        // CustomMessage3 = 11,
        // CustomMessage4 = 12,
        // CustomMessage5 = 13,
        // CustomMessage6 = 14,
        // CustomMessage7 = 15,
        // You are running an older version of the game.
        // Please update to play with others.
        IncorrectVersion = 5,
        // You cannot rejoin that room.
        // You were banned
        Banned = 6,
        // You can rejoin if the room hasn't started
        // You were kicked
        Kicked = 7,
        // Server refused username: %USERNAME%
        InvalidName = 9,
        // You were banned for hacking.
        // Please stop.
        Hacking = 10,
        // You disconnected from the host.
        // If this happens often, check your WiFi strength.
        //
        // You disconnected from the server.
        // If this happens often, check your network strength.
        // This may also be a server issue.
        Error = 17,
        // The server stopped this game. Possibly due to inactivity.
        ServerRequest = 19,
        // The Among Us servers are overloaded.
        // Sorry! Please try again later!
        ServerFull = 20,
        FocusLostBackground = 207,
        IntentionalLeaving = 208,
        FocusLost = 209,
        NewConnection = 210
    }
}