using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Inner.Objects.GameManager;
using Impostor.Api.Net.Inner.Objects.ShipStatus;

namespace Impostor.Api.Net.Inner
{
    /// <summary>
    ///     Holds all data that is serialized over the network through GameData packets.
    /// </summary>
    public interface IGameNet
    {
        IInnerGameManager? GameManager { get; }

        IInnerLobbyBehaviour? LobbyBehaviour { get; }

        IInnerGameData? GameData { get; }

        IInnerVoteBanSystem? VoteBan { get; }

        IInnerShipStatus? ShipStatus { get; }
    }
}
