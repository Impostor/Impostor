using Impostor.Api.Net.Inner;

namespace Impostor.Api;

public class CheatContext
{
    public CheatContext(string name)
    {
        Name = name;
    }

    public static CheatContext Deserialize { get; } = new(nameof(Deserialize));

    public static CheatContext Serialize { get; } = new(nameof(Serialize));

    public string Name { get; }

    public static implicit operator CheatContext(RpcCalls rpcCalls)
    {
        return new CheatContext(rpcCalls.ToString());
    }
}
