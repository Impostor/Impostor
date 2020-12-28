using System.Numerics;
using System.Threading.Tasks;
using Impostor.Api.Net.Inner.Objects.Components;

namespace Impostor.Server.Net.Inner.Objects.Components
{
    internal partial class InnerCustomNetworkTransform : IInnerCustomNetworkTransform
    {
        public Vector2 Position => _targetSyncPosition;

        public async ValueTask SnapToAsync(Vector2 position)
        {
            var minSid = (ushort)(_lastSequenceId + 5U);

            // Snap in the server.
            SnapTo(position, minSid);

            // Broadcast to all clients.
            using (var writer = _game.StartRpc(NetId, RpcCalls.SnapTo))
            {
                WriteVector2(writer, position);
                writer.Write(_lastSequenceId);
                await _game.FinishRpcAsync(writer);
            }
        }
    }
}
