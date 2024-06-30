using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Inner.Objects.GameManager;
using Impostor.Api.Net.Inner.Objects.ShipStatus;

namespace Impostor.Server.Net.State
{
    /// <inheritdoc />
    internal partial class GameNet : IGameNet
    {
        IInnerGameManager? IGameNet.GameManager => GameManager;

        IInnerLobbyBehaviour? IGameNet.LobbyBehaviour => LobbyBehaviour;

        IInnerGameData IGameNet.GameData => GameData;

        IInnerVoteBanSystem? IGameNet.VoteBan => VoteBan;

        IInnerShipStatus? IGameNet.ShipStatus => ShipStatus;
    }
}
