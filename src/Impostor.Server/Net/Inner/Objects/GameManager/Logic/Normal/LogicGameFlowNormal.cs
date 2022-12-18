namespace Impostor.Server.Net.Inner.Objects.GameManager.Logic.Normal;

internal class LogicGameFlowNormal : LogicGameFlow
{
    public LogicGameFlowNormal(InnerNormalGameManager gameManager)
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
