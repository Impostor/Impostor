using Impostor.Api.Innersloth.GameOptions;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner.Objects.GameManager.Logic;

internal abstract class LogicOptions : GameLogicComponent
{
    private readonly Game _game;

    protected LogicOptions(Game game)
    {
        _game = game;
    }

    public override bool Serialize(IMessageWriter writer, bool initialState)
    {
        GameOptionsFactory.Serialize(writer, _game.Options);
        return true;
    }

    public override void Deserialize(IMessageReader reader, bool initialState)
    {
        GameOptionsFactory.DeserializeInto(reader, _game.Options);
    }
}
