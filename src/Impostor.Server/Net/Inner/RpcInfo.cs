namespace Impostor.Server.Net.Inner
{
    public class RpcInfo
    {
        public RpcTargetType TargetType { get; init; }

        public bool CheckOwnership { get; init; } = true;

        public bool RequireHost { get; init; }
    }

    public enum RpcTargetType
    {
        Broadcast,
        Target,
        Both,
        Cmd
    }
}
