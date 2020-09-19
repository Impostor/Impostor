using AmongUs.Shared.Innersloth;

namespace AmongUs.Server.Net
{
    public class Game
    {
        public Game(int code, GameOptionsData options)
        {
            Code = code;
            Options = options;
        }
        
        public int Code { get; }
        public GameOptionsData Options { get; }
    }
}