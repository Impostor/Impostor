using System.Numerics;
using System.Threading.Tasks;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Inner.Objects.Components;
using Impostor.Api.Net.Messages.Rpcs;

namespace Impostor.Server.Net.Inner.Objects.Components
{
    internal partial class InnerPlayerPhysics : IInnerPlayerPhysics
    {
        public async ValueTask EnterVentAsync(int ventId)
        {
            using var writer = Game.StartRpc(NetId, RpcCalls.EnterVent);
            Rpc19EnterVent.Serialize(writer, ventId);
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask ExitVentAsync(int ventId)
        {
            using var writer = Game.StartRpc(NetId, RpcCalls.ExitVent);
            Rpc20ExitVent.Serialize(writer, ventId);
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask BootFromVentAsync(int ventId)
        {
            using var writer = Game.StartRpc(NetId, RpcCalls.BootFromVent);
            Rpc34BootFromVent.Serialize(writer, ventId);
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask ClimbLadderAsync(byte ladderId)
        {
            using var writer = Game.StartRpc(NetId, RpcCalls.ClimbLadder);
            Rpc31ClimbLadder.Serialize(writer, ladderId, ++_lastClimbLadderSid);
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask PetAsync(Vector2 position, Vector2 petPosition)
        {
            using var writer = Game.StartRpc(NetId, RpcCalls.Pet);
            Rpc49Pet.Serialize(writer, position, petPosition);
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask CancelPetAsync()
        {
            using var writer = Game.StartRpc(NetId, RpcCalls.CancelPet);
            Rpc50CancelPet.Serialize(writer);
            await Game.FinishRpcAsync(writer);
        }
    }
}
