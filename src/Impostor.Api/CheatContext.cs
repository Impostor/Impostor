using Impostor.Api.Net.Inner;

namespace Impostor.Api;

public class CheatContext(string name)
{
    public static CheatContext Deserialize { get; } = new(nameof(Deserialize));

    public static CheatContext Serialize { get; } = new(nameof(Serialize));

    public string Name { get; } = name;

    public static implicit operator CheatContext(RpcCalls rpcCalls)
    {
        return new CheatContext(rpcCalls.ToString());
    }
}
