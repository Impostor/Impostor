using Impostor.Server.Net.Inner.Objects;
using Impostor.Server.Net.Inner.Objects.Components;
using Impostor.Server.Net.Inner.Objects.ShipStatus;

namespace Impostor.Server.Net.State
{
    internal partial class GameNet
    {
        public InnerLobbyBehaviour? LobbyBehaviour { get; internal set; }

        public InnerGameData? GameData { get; internal set; }

        public InnerVoteBanSystem? VoteBan { get; internal set; }

        public InnerShipStatus? ShipStatus { get; internal set; }
    }
}
