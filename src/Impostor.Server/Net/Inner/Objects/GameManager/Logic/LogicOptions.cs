using Impostor.Api.Events.Managers;
using Impostor.Api.Innersloth.GameOptions;
using Impostor.Server.Events;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner.Objects.GameManager.Logic;

internal abstract class LogicOptions : GameLogicComponent
{
    private readonly Game _game;
    private readonly IEventManager _eventManager;

    protected LogicOptions(Game game, IEventManager eventManager)
    {
        _game = game;
        _eventManager = eventManager;
    }

    public override bool Serialize(IMessageWriter writer, bool initialState)
    {
        GameOptionsFactory.Serialize(writer, _game.Options);
        return true;
    }

    public override async void Deserialize(IMessageReader reader, bool initialState)
    {
        GameOptionsFactory.DeserializeInto(reader, _game.Options);
        await _eventManager.CallAsync(new GameOptionsChangedEvent(
            _game,
            Api.Events.IGameOptionsChangedEvent.ChangeReason.Rpc));
    }
}
