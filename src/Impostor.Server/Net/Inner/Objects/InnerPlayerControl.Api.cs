using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.Customization;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Inner.Objects.Components;
using Impostor.Server.Events.Player;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerPlayerControl : IInnerPlayerControl
    {
        IInnerPlayerPhysics IInnerPlayerControl.Physics => Physics;

        IInnerCustomNetworkTransform IInnerPlayerControl.NetworkTransform => NetworkTransform;

        IInnerPlayerInfo IInnerPlayerControl.PlayerInfo => PlayerInfo;

        public async ValueTask SetNameAsync(string name)
        {
            PlayerInfo.PlayerName = name;

            using var writer = _game.StartRpc(NetId, RpcCalls.SetName);
            writer.Write(name);
            await _game.FinishRpcAsync(writer);
        }

        public async ValueTask SetColorAsync(ColorType color)
        {
            PlayerInfo.Color = color;

            using var writer = _game.StartRpc(NetId, RpcCalls.SetColor);
            writer.Write((byte)color);
            await _game.FinishRpcAsync(writer);
        }

        public async ValueTask SetHatAsync(HatType hat)
        {
            PlayerInfo.Hat = hat;

            using var writer = _game.StartRpc(NetId, RpcCalls.SetHat);
            writer.WritePacked((uint)hat);
            await _game.FinishRpcAsync(writer);
        }

        public async ValueTask SetPetAsync(PetType pet)
        {
            PlayerInfo.Pet = pet;

            using var writer = _game.StartRpc(NetId, RpcCalls.SetPet);
            writer.WritePacked((uint)pet);
            await _game.FinishRpcAsync(writer);
        }

        public async ValueTask SetSkinAsync(SkinType skin)
        {
            PlayerInfo.Skin = skin;

            using var writer = _game.StartRpc(NetId, RpcCalls.SetSkin);
            writer.WritePacked((uint)skin);
            await _game.FinishRpcAsync(writer);
        }

        public async ValueTask SendChatAsync(string text)
        {
            using var writer = _game.StartRpc(NetId, RpcCalls.SendChat);
            writer.Write(text);
            await _game.FinishRpcAsync(writer);
        }

        public async ValueTask SendChatToPlayerAsync(string text, IInnerPlayerControl? player = null)
        {
            if (player == null)
            {
                player = this;
            }

            using var writer = _game.StartRpc(NetId, RpcCalls.SendChat);
            writer.Write(text);
            await _game.FinishRpcAsync(writer, player.OwnerId);
        }

        public async ValueTask MurderPlayerAsync(IInnerPlayerControl target, bool fireEvent)
        {
            if (!PlayerInfo.IsImpostor)
            {
                throw new ImpostorProtocolException("Tried to murder a player, but murderer was not the impostor.");
            }

            if (PlayerInfo.IsDead)
            {
                throw new ImpostorProtocolException("Tried to murder a player, but murderer was not alive.");
            }

            if (target.PlayerInfo.IsDead)
            {
                throw new ImpostorProtocolException("Tried to murder a player, but target was not alive.");
            }

            ((InnerPlayerControl)target).Die(DeathReason.Kill);

            using var writer = _game.StartRpc(NetId, RpcCalls.MurderPlayer);
            writer.WritePacked(target.NetId);
            await _game.FinishRpcAsync(writer);

            if (fireEvent)
                await _eventManager.CallAsync(new PlayerMurderEvent(_game, _game.GetClientPlayer(OwnerId), this, target));
        }
    }
}
