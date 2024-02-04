namespace Impostor.Api.Languages;

public interface ILanguageManager
{
    public string GetString(string key);

    public void AddTranslate(string key, string value);

    public void AddTranslate(LanguageTag tag);

    public void RemoveTranslate(string key);

    public void RemoveTranslate(LanguageTag tag);
}
