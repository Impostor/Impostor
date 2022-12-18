namespace Impostor.Server.Net.Inner.Objects.GameManager.Logic.HideAndSeek;

internal class LogicUsablesHnS : LogicUsables
{
    public override bool Serialize(IMessageWriter writer, bool initialState)
    {
        return false;
    }

    public override void Deserialize(IMessageReader reader, bool initialState)
    {
    }
}
