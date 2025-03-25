using Impostor.Api.Innersloth;

namespace SelfHttpMatchmaker.Types;

public class GameFilterSet
{
    public GameModes GameMode { get; set; }


    public List<GameFilter> Filters { get; set; }
}
