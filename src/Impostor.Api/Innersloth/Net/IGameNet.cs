using Impostor.Api.Innersloth.Net.Objects;
using Impostor.Api.Innersloth.Net.Objects.Components;

namespace Impostor.Api.Innersloth.Net
{
    public interface IGameNet
    {
        InnerLobbyBehaviour LobbyBehaviour { get; }

        InnerGameData GameData { get; }

        InnerVoteBanSystem VoteBan { get; }

        InnerShipStatus ShipStatus { get; }
    }
}