using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Inner.Objects.GameManager;
using Impostor.Api.Net.Inner.Objects.ShipStatus;

namespace Impostor.Server.Net.State;

/// <inheritdoc />
internal partial class GameNet : IGameNet
{
    IInnerGameManager? IGameNet.GameManager
    {
        get => GameManager;
    }

    IInnerLobbyBehaviour? IGameNet.LobbyBehaviour
    {
        get => LobbyBehaviour;
    }

    IInnerGameData IGameNet.GameData
    {
        get => GameData;
    }

    IInnerVoteBanSystem? IGameNet.VoteBan
    {
        get => VoteBan;
    }

    IInnerShipStatus? IGameNet.ShipStatus
    {
        get => ShipStatus;
    }
}
