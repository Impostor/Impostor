namespace Impostor.Server.Net.Inner.Objects.GameManager.Logic.Normal;

internal class LogicRoleSelectionNormal : LogicRoleSelection
{
    public LogicRoleSelectionNormal(InnerNormalGameManager gameManager)
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
