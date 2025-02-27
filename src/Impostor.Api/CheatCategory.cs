namespace Impostor.Api;

public enum CheatCategory
{
    /// <summary>A packet used a part of the network protocol that is unknown to Impostor, like a custom RPC.</summary>
    ProtocolExtension,

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

    /// <summary>Legacy category for unsorted anticheat checks.</summary>
    Other,
}
