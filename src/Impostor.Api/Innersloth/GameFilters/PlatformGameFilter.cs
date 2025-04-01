using System;

namespace Impostor.Api.Innersloth.GameFilters
{
    [Serializable]
    public class PlatformGameFilter : ISubFilter
    {
        public string FilterType { get; } = "platform";

        public uint AcceptedValues;
    }
}
