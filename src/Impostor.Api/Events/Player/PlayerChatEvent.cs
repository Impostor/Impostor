using Impostor.Api.Games;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events.Player
{
    public class PlayerChatEvent : IGameEvent
    {
        public PlayerChatEvent(IGame game, IInnerPlayerControl playerControl, string message)
        {
            Game = game;
            PlayerControl = playerControl;
            Message = message;
        }

        public IGame Game { get; }

        public IInnerPlayerControl PlayerControl { get; }

        public string Message { get; }
    }
}