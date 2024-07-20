using System.Collections.Generic;
using Impostor.Api.Innersloth;

namespace Impostor.Api
{
    public static class SystemTypesExtensions
    {
        public static readonly HashSet<SystemTypes> CriticalSabotages = new()
        {
            SystemTypes.Reactor,
            SystemTypes.Electrical,
            SystemTypes.LifeSupp,
            SystemTypes.Comms,
            SystemTypes.Laboratory,
            SystemTypes.MushroomMixupSabotage,
            SystemTypes.HeliSabotage,
        };

        public static string GetFriendlyName(this SystemTypes type)
        {
            return SystemTypeHelpers.Names[(int)type];
        }

        public static bool IsCriticalSabotage(this SystemTypes type)
        {
            return CriticalSabotages.Contains(type);
        }
    }
}
