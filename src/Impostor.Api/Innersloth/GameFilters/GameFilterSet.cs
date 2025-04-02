using System;
using System.Collections.Generic;

namespace Impostor.Api.Innersloth.GameFilters
{
    [Serializable]
    public class GameFilterSet
    {
        public GameFilterSet(GameModes mode)
        {
            this.GameMode = mode;
            this.Filters = new List<GameFilter>();
        }

        public GameFilterSet()
        {
            this.GameMode = GameModes.Normal;
            this.Filters = new List<GameFilter>();
        }

        public GameModes GameMode;

        public List<GameFilter> Filters;
    }
}
