using System;

namespace Impostor.Api.Innersloth.GameFilters
{
    [Serializable]
    public class MapGameFilter : ISubFilter
    {
        public string FilterType { get; } = "map";

        public byte AcceptedValues;
    }
}
