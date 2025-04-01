using System;

namespace Impostor.Api.Innersloth.GameFilters
{
    [Serializable]
    public class LanguageFilter : ISubFilter
    {
        public string FilterType { get; } = "languages";

        public uint AcceptedValues;
    }
}
