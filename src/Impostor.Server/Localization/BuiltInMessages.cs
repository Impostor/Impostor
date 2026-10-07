using Impostor.Api.Localization;

namespace Impostor.Server.Localization
{
    public static class BuiltInMessages
    {
        public static LocalizationCatalog Load() => new(
            "impostor",
            typeof(BuiltInMessages).Assembly,
            "Impostor.Server.Localization.Resources");
    }
}
