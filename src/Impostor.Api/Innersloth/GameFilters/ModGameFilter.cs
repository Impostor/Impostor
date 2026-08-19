using System;

namespace Impostor.Api.Innersloth.GameFilters
{
    [Serializable]
    public class ModGameFilter : ISubFilter
    {
        public Guid AcceptedValues { get; set; }

        public string FilterType => "mod";
    }
}