using Impostor.Api.Innersloth;

namespace Impostor.Api
{
    public static class SystemTypesExtensions
    {
        public static string GetFriendlyName(this SystemTypes type)
        {
            return SystemTypeHelpers.Names[(int)type];
        }
    }
}
