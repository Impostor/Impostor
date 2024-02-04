namespace Impostor.Api.Languages;

public static class LanguageExtension
{
    public static ILanguageManager? Manager;

    public static void SetLanguageManager(ILanguageManager? manager)
    {
        Manager = manager;
    }

    public static string Translate(this string key)
    {
        return Manager?.GetString(key)!;
    }
}
