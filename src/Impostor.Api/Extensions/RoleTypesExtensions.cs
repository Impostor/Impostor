using Impostor.Api.Innersloth;

namespace Impostor.Api;

public static class RoleTypesExtensions
{
    public static bool IsGhostRole(this RoleTypes role) => role.GetTraits().IsGhostRole;

    public static bool IsImpostorRole(this RoleTypes role) => role.GetTraits().IsImpostorRole;

    public static bool AlertsOnDeath(this RoleTypes role) => role.GetTraits().AlertsOnDeath;

    public static bool CanKill(this RoleTypes role) => role.GetTraits().CanKill;

    public static bool CanSabotage(this RoleTypes role) => role.GetTraits().CanSabotage;

    public static bool CanVent(this RoleTypes role) => role.GetTraits().CanVent;

    public static bool CanDoTasks(this RoleTypes role) => role.GetTraits().CanDoTasks;

    public static bool CanShapeshift(this RoleTypes role) => role.GetTraits().CanShapeshift;

    public static bool CanVanish(this RoleTypes role) => role.GetTraits().CanVanish;

    public static bool CanDissolve(this RoleTypes role) => role.GetTraits().CanDissolve;

    public static bool CanUseVitals(this RoleTypes role) => role.GetTraits().CanUseVitals;

    public static bool CanTrack(this RoleTypes role) => role.GetTraits().CanTrack;

    public static bool CanSuspect(this RoleTypes role) => role.GetTraits().CanSuspect;

    public static bool CanOverrule(this RoleTypes role) => role.GetTraits().CanOverrule;

    public static bool CanProtect(this RoleTypes role) => role.GetTraits().CanProtect;

    public static bool CanSendPhoto(this RoleTypes role) => role.GetTraits().CanSendPhoto;

    internal static RoleTraits GetTraits(this RoleTypes role)
    {
        return role switch
        {
            RoleTypes.Crewmate => new RoleTraits
            {
                CanDoTasks = true,
            },
            RoleTypes.Impostor => new RoleTraits
            {
                IsImpostorRole = true,
                CanKill = true,
                CanSabotage = true,
                CanVent = true,
            },
            RoleTypes.Scientist => new RoleTraits
            {
                CanDoTasks = true,
                CanUseVitals = true,
            },
            RoleTypes.Engineer => new RoleTraits
            {
                CanVent = true,
                CanDoTasks = true,
            },
            RoleTypes.GuardianAngel => new RoleTraits
            {
                IsGhostRole = true,
                CanDoTasks = true,
                CanProtect = true,
            },
            RoleTypes.Shapeshifter => new RoleTraits
            {
                IsImpostorRole = true,
                CanKill = true,
                CanSabotage = true,
                CanVent = true,
                CanShapeshift = true,
            },
            RoleTypes.CrewmateGhost => new RoleTraits
            {
                IsGhostRole = true,
                CanDoTasks = true,
            },
            RoleTypes.ImpostorGhost => new RoleTraits
            {
                IsGhostRole = true,
                IsImpostorRole = true,
            },
            RoleTypes.Noisemaker => new RoleTraits
            {
                AlertsOnDeath = true,
                CanDoTasks = true,
            },
            RoleTypes.Phantom => new RoleTraits
            {
                IsImpostorRole = true,
                CanKill = true,
                CanSabotage = true,
                CanVent = true,
                CanVanish = true,
            },
            RoleTypes.Tracker => new RoleTraits
            {
                CanDoTasks = true,
                CanTrack = true,
            },
            RoleTypes.Detective => new RoleTraits
            {
                CanDoTasks = true,
                CanSuspect = true,
            },
            RoleTypes.Viper => new RoleTraits
            {
                IsImpostorRole = true,
                CanKill = true,
                CanSabotage = true,
                CanVent = true,
                CanDissolve = true,
            },
            RoleTypes.Judge => new RoleTraits
            {
                CanDoTasks = true,
                CanOverrule = true,
            },
            RoleTypes.SpiritGuide => new RoleTraits
            {
                IsGhostRole = true,
                CanDoTasks = true,
                CanSendPhoto = true,
            },

            // A client can put any value in RpcSetRole, so this must not throw.
            _ => default,
        };
    }
}
