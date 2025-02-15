using Impostor.Api.Events.Player;
using Impostor.Api.Games;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Events.Player;

public class PlayerVotedEvent(
    IGame game,
    IClientPlayer clientPlayer,
    IInnerPlayerControl playerControl,
    VoteType voteType,
    IInnerPlayerControl? votedFor)
    : IPlayerVotedEvent
{
    public IGame Game { get; } = game;

    public IClientPlayer ClientPlayer { get; } = clientPlayer;

    public IInnerPlayerControl PlayerControl { get; } = playerControl;

    public IInnerPlayerControl? VotedFor { get; } = votedFor;

    public VoteType VoteType { get; } = voteType;
}
