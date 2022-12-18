namespace Impostor.Server.Net.Inner.Objects.GameManager.Logic.HideAndSeek;

internal class LogicHnSDangerLevel : GameLogicComponent
{
    public LogicHnSDangerLevel(InnerHideAndSeekManager gameManager)
    {
    }

    public override bool Serialize(IMessageWriter writer, bool initialState)
    {
        return false;
    }

    public override void Deserialize(IMessageReader reader, bool initialState)
    {
    }
}
