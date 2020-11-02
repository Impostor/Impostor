using System.Numerics;
using System.Threading.Tasks;
using Impostor.Api.Net.Inner.Objects.Components;

namespace Impostor.Server.Net.Inner.Objects.Components
{
    internal partial class InnerCustomNetworkTransform : IInnerCustomNetworkTransform
    {
        public async ValueTask SnapTo(Vector2 position)
        {
            var writer = _game.StartRpc(NetId, RpcCalls.SnapTo);
            WriteVector2(writer, position);
            writer.Write(_lastSequenceId + 5U);
            await _game.FinishRpcAsync(writer);
        }
    }
}
