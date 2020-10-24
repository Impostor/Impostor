using Impostor.Api.Games;
using Impostor.Api.Innersloth.Net.Objects;

namespace Impostor.Api.Events.Net
{
    public class PlayerChatEvent : IGameEvent
    {
        public PlayerChatEvent(IGame game, InnerPlayerControl playerControl, string message)
        {
            Game = game;
            PlayerControl = playerControl;
            Message = message;
        }

        public IGame Game { get; }

        public InnerPlayerControl PlayerControl { get; }

        public string Message { get; }
    }
}