namespace Impostor.Api;

/// <summary>
///     Traits of a role. The only input is the role itself.
/// </summary>
internal readonly record struct RoleTraits
{
    public bool IsGhostRole { get; init; }

    public bool IsImpostorRole { get; init; }

    public bool AlertsOnDeath { get; init; }

    public bool CanKill { get; init; }

    public bool CanSabotage { get; init; }

    public bool CanVent { get; init; }

    public bool CanDoTasks { get; init; }

    public bool CanShapeshift { get; init; }

    public bool CanVanish { get; init; }

    public bool CanDissolve { get; init; }

    public bool CanUseVitals { get; init; }

    public bool CanTrack { get; init; }

    public bool CanSuspect { get; init; }

    public bool CanOverrule { get; init; }

    public bool CanProtect { get; init; }

    public bool CanSendPhoto { get; init; }
}
