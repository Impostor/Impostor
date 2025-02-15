using Impostor.Api.Games;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Net.State;

internal partial class ClientPlayer
{
    /// <inheritdoc />
    IClient IClientPlayer.Client
    {
        get => Client;
    }

    /// <inheritdoc />
    IGame IClientPlayer.Game
    {
        get => Game;
    }

    /// <inheritdoc />
    IInnerPlayerControl? IClientPlayer.Character
    {
        get => Character;
    }
}
