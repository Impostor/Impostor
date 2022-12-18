namespace Impostor.Server.Net.Inner.Objects.GameManager.Logic.HideAndSeek;

internal class LogicGameFlowHnS : LogicGameFlow
{
    private float currentFinalHideTime;
    private float currentHideTime;

    public LogicGameFlowHnS(InnerHideAndSeekManager gameManager)
    {
    }

    public override bool Serialize(IMessageWriter writer, bool initialState)
    {
        throw new System.NotImplementedException();
    }

    public override void Deserialize(IMessageReader reader, bool initialState)
    {
        var num = reader.ReadSingle();

        currentFinalHideTime = reader.ReadSingle();
        currentHideTime = num;
    }
}
