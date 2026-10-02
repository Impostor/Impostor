namespace Impostor.Api;

public enum CheatCategory
{
    /// <summary>A packet used a part of the network protocol that is unknown to Impostor, like a custom RPC.</summary>
    ProtocolExtension,

    /// <summary>A host-only mod extension is used outside of host-authoritive mode.</summary>
    HostOnlyExtension,

    /// <summary>A packet was sent at an inappropriate moment.</summary>
    GameFlow,

    /// <summary>A packet was sent by a non-host player that should normally only be sent by the host.</summary>
    MustBeHost,

    /// <summary>A packet was sent that violated limits on the selection of player colors.</summary>
    ColorLimits,

    /// <summary>A packet was sent that exceeded the limits of possible nicknames to enter ingame.</summary>
    NameLimits,

    /// <summary>A packet was sent on behalf of another player.</summary>
    Ownership,

    /// <summary>An ability was used that the current role cannot access.</summary>
    Role,

    /// <summary>A packet was sent to a player that should be broadcasted, or vice versa.</summary>
    Target,

    /// <summary>A packet was sent on an invalid network object, like a PlayerControl without PlayerInfo.</summary>
    InvalidObject,

    /// <summary>A packet was sent that exceeded the maximum allowed RPC size.</summary>
    PacketSize,

    /// <summary>A packet was sent with more items than possible in the game.</summary>
    ItemLimits,

    /// <summary>A kill that the killer's own cooldown cannot explain, like a second victim inside the window or another hit on a dead one.</summary>
    Murder,

    /// <summary>A sabotage the current game state cannot produce, like switching systems too fast or sabotaging during a meeting.</summary>
    Sabotage,

    /// <summary>A meeting or a report that this round cannot produce yet.</summary>
    Meeting,

    /// <summary>A vote cast outside a meeting, or by a player who is already dead.</summary>
    Voting,

    /// <summary>A client sent more messages in a window than the room can legitimately produce.</summary>
    RateLimit,

    /// <summary>Legacy category for unsorted anticheat checks.</summary>
    Other,
}
