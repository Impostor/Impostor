namespace Impostor.Server.Net.Inner.Objects.GameManager.Logic;

internal class LogicMinigame : GameLogicComponent
{
    public LogicMinigame(InnerGameManager gameManager)
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
