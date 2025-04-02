using System;
using System.Collections.Generic;

namespace Impostor.Api.Innersloth.GameFilters
{
    [Serializable]
    public class GameFiltersList
    {
        public List<GameFilterSet> FilterSets = new List<GameFilterSet>
        {
            new GameFilterSet()
        };
    }
}
