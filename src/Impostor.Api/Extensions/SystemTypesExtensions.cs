using Impostor.Api.Innersloth.Net.Objects.Systems;

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