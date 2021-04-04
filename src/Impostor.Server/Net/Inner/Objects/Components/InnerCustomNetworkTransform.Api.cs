using System.Numerics;
using System.Threading.Tasks;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Inner.Objects.Components;

namespace Impostor.Server.Net.Inner.Objects.Components
{
    internal partial class InnerCustomNetworkTransform : IInnerCustomNetworkTransform
    {
        public async ValueTask SnapToAsync(Vector2 position)
        {
            var minSid = (ushort)(_lastSequenceId + 5U);

            // Snap in the server.
            await SnapToAsync(Game.GetClientPlayer(OwnerId)!, position, minSid);

            // Broadcast to all clients.
            using (var writer = Game.StartRpc(NetId, RpcCalls.SnapTo))
            {
                writer.Write(position);
                writer.Write(_lastSequenceId);
                await Game.FinishRpcAsync(writer);
            }
        }
    }
}
