using System.Threading.Tasks;
using Impostor.Api.Events;
using Impostor.Api.Events.Managers;
using Impostor.Api.Innersloth.GameOptions;
using Impostor.Server.Events;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner.Objects.GameManager.Logic;

internal abstract class LogicOptions(Game game, IEventManager eventManager) : GameLogicComponent
{
    public override ValueTask<bool> SerializeAsync(IMessageWriter writer, bool initialState)
    {
        GameOptionsFactory.Serialize(writer, game.Options);
        return ValueTask.FromResult(true);
    }

    public override async ValueTask DeserializeAsync(IMessageReader reader, bool initialState)
    {
        GameOptionsFactory.DeserializeInto(reader, game.Options);
        await eventManager.CallAsync(new GameOptionsChangedEvent(
            game,
            IGameOptionsChangedEvent.ChangeReason.Host
        ));
    }
}
