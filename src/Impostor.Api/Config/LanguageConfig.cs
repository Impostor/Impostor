using System;
using Impostor.Api.Innersloth;

namespace Impostor.Api.Config;

public class LanguageConfig
{
    public const string Section = "LanguageManager";

    public SupportedLanguages _Language;

    public string Language { get; set; }

    public SupportedLanguages GetLang()
    {
        return _Language = Enum.Parse<SupportedLanguages>(Language);
    }
}
