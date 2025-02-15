using System.Threading.Tasks;
using Impostor.Api.Events;
using Impostor.Api.Events.Managers;
using Impostor.Api.Innersloth.GameOptions;
using Impostor.Server.Events;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner.Objects.GameManager.Logic;

internal abstract class LogicOptions : GameLogicComponent
{
    private readonly IEventManager _eventManager;
    private readonly Game _game;

    protected LogicOptions(Game game, IEventManager eventManager)
    {
        _game = game;
        _eventManager = eventManager;
    }

    public override ValueTask<bool> SerializeAsync(IMessageWriter writer, bool initialState)
    {
        GameOptionsFactory.Serialize(writer, _game.Options);
        return ValueTask.FromResult(true);
    }

    public override async ValueTask DeserializeAsync(IMessageReader reader, bool initialState)
    {
        GameOptionsFactory.DeserializeInto(reader, _game.Options);
        await _eventManager.CallAsync(new GameOptionsChangedEvent(
            _game,
            IGameOptionsChangedEvent.ChangeReason.Host
        ));
    }
}
