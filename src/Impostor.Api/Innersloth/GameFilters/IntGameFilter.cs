using System;
using System.Collections.Generic;

namespace Impostor.Api.Innersloth.GameFilters
{
    [Serializable]
    public class IntGameFilter : ISubFilter
    {
        public string FilterType { get; } = "int";

        public List<int> AcceptedValues;

        public Int32OptionNames OptionEnum;
    }
}
