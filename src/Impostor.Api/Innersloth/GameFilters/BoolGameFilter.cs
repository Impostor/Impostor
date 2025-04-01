using System;
using System.Collections.Generic;

namespace Impostor.Api.Innersloth.GameFilters
{
    [Serializable]
    public class BoolGameFilter : ISubFilter
    {
        public string FilterType { get; } = "bool";

        public List<bool> AcceptedValues;

        public BoolOptionNames OptionEnum;
    }
}
