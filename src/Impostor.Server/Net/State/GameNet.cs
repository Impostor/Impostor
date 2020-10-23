using Impostor.Api.Innersloth.Net;
using Impostor.Api.Innersloth.Net.Objects;
using Impostor.Api.Innersloth.Net.Objects.Components;

namespace Impostor.Server.Net.State
{
    /// <summary>
    ///     Holds all data that is serialized over the network through GameData packets.
    /// </summary>
    public class GameNet : IGameNet
    {
        public InnerLobbyBehaviour LobbyBehaviour { get; internal set; }

        public InnerGameData GameData { get; internal set; }

        public InnerVoteBanSystem VoteBan { get; internal set; }

        public InnerShipStatus ShipStatus { get; internal set; }
    }
}