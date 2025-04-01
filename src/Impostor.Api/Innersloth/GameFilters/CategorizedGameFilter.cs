using System;
using System.Collections.Generic;

namespace Impostor.Api.Innersloth.GameFilters
{
    [Serializable]
    public class CategorizedGameFilter : ISubFilter
    {
        public string FilterType { get; } = "cat";

        public List<int> AcceptedValues;

        public CategorizedOptionNames OptionEnum;
    }
}
