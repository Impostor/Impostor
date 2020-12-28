using System;
using Impostor.Api.Events.Player;
using Impostor.Api.Games;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Events.Player
{
    public class PlayerChatEvent : IPlayerChatEvent
    {
        public PlayerChatEvent(IGame game, IClientPlayer clientPlayer, IInnerPlayerControl playerControl, string message, Action<bool> cancelEvent)
        {
            Game = game;
            ClientPlayer = clientPlayer;
            PlayerControl = playerControl;
            Message = message;
            CancelEvent = cancelEvent;
        }

        public IGame Game { get; }

        public IClientPlayer ClientPlayer { get; }

        public IInnerPlayerControl PlayerControl { get; }

        public string Message { get; }

        public Action<bool> CancelEvent { get; }
    }
}
