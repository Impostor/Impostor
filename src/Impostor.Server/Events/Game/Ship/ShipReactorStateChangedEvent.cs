using Impostor.Api.Events.Ship;
using Impostor.Api.Games;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Inner.Objects.ShipSystems;

namespace Impostor.Server.Events.Ship
{
    public class ShipReactorStateChangedEvent : IShipReactorStateChangedEvent
    {
        public ShipReactorStateChangedEvent(IGame game, IReactorSystem reactor)
        {
            Game = game;
            Reactor = reactor;
        }

        public IGame Game { get; }

        public IReactorSystem Reactor { get; }

        public IInnerShipStatus ShipStatus => Game.GameNet.ShipStatus;
    }
}
