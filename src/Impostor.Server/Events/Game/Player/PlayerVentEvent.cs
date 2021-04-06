using Impostor.Api.Events.Player;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Events.Player
{
    public class PlayerVentEvent : IPlayerVentEvent
    {
        public PlayerVentEvent(IGame game, IClientPlayer sender, IInnerPlayerControl innerPlayerPhysics, Vent vent, bool ventEnter)
        {
            Game = game;
            ClientPlayer = sender;
            PlayerControl = innerPlayerPhysics;
            Vent = vent;
            VentEnter = ventEnter;
        }

        public IGame Game { get; }

        public IClientPlayer ClientPlayer { get; }

        public IInnerPlayerControl PlayerControl { get; }

        public Vent Vent { get; }

        public bool VentEnter { get; }
    }
}
