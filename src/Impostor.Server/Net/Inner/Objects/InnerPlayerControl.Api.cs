using System.Threading.Tasks;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerPlayerControl : IInnerPlayerControl
    {
        IInnerPlayerInfo IInnerPlayerControl.PlayerInfo => PlayerInfo;

        public async ValueTask SetNameAsync(string name)
        {
            using (var writer = _game.StartRpc(NetId, RpcCalls.SetName))
            {
                writer.Write(name);
                await _game.FinishRpcAsync(writer);
            }
        }

        public async ValueTask SendChatAsync(string text)
        {
            using (var writer = _game.StartRpc(NetId, RpcCalls.SendChat))
            {
                writer.Write(text);
                await _game.FinishRpcAsync(writer);
            }
        }
    }
}