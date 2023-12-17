using Impostor.Api.Events.Player;
using Impostor.Api.Games;
using Impostor.Api.Innersloth.Maps;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Events.Player
{
    public class PlayerVentEvent : IPlayerVentEvent
    {
        public PlayerVentEvent(IGame game, IClientPlayer sender, IInnerPlayerControl innerPlayerPhysics, VentData vent)
        {
            Game = game;
            ClientPlayer = sender;
            PlayerControl = innerPlayerPhysics;
            NewVent = vent;
        }

        public IGame Game { get; }

        public IClientPlayer ClientPlayer { get; }

        public IInnerPlayerControl PlayerControl { get; }

        public VentData NewVent { get; }
    }
}
