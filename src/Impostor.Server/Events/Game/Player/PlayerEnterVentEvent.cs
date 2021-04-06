using Impostor.Api.Events.Player;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Events.Player
{
    public class PlayerEnterVentEvent : IPlayerEnterVentEvent
    {
        public PlayerEnterVentEvent(IGame game, IClientPlayer sender, IInnerPlayerControl innerPlayerPhysics, Vent vent)
        {
            Game = game;
            ClientPlayer = sender;
            PlayerControl = innerPlayerPhysics;
            Vent = vent;
        }

        public IGame Game { get; }

        public IClientPlayer ClientPlayer { get; }

        public IInnerPlayerControl PlayerControl { get; }

        public Vent Vent { get; }
    }
}
