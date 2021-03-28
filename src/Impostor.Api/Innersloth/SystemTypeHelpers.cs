using System;
using System.Linq;

namespace Impostor.Api.Innersloth
{
    internal class SystemTypeHelpers
    {
        public static readonly SystemTypes[] AllTypes;
        public static readonly string[] Names;

        static SystemTypeHelpers()
        {
            AllTypes = Enum.GetValues(typeof(SystemTypes)).Cast<SystemTypes>().ToArray();
            Names = AllTypes.Select(x =>
            {
                return x switch
                {
                    SystemTypes.UpperEngine => "Upper Engine",
                    SystemTypes.Nav => "Navigations",
                    SystemTypes.LifeSupp => "O2",
                    SystemTypes.LowerEngine => "Lower Engine",
                    SystemTypes.LockerRoom => "Locker Room",
                    _ => x.ToString(),
                };
            }).ToArray();
        }
    }
}
