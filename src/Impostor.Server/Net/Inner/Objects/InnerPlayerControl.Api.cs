using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.Customization;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Inner.Objects.Components;
using Impostor.Api.Net.Messages.Rpcs;
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
            PlayerInfo.CurrentOutfit.PlayerName = name;

            using var writer = Game.StartRpc(NetId, RpcCalls.SetName);
            Rpc06SetName.Serialize(writer, PlayerInfo.NetId, name);
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask SetColorAsync(ColorType color)
        {
            PlayerInfo.CurrentOutfit.Color = color;

            using var writer = Game.StartRpc(NetId, RpcCalls.SetColor);
            Rpc08SetColor.Serialize(writer, PlayerInfo.NetId, color);
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask SetHatAsync(string hatId)
        {
            PlayerInfo.CurrentOutfit.HatId = hatId;

            using var writer = Game.StartRpc(NetId, RpcCalls.SetHatStr);
            Rpc39SetHatStr.Serialize(writer, hatId, PlayerInfo.GetNextRpcSequenceId(RpcCalls.SetHatStr));
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask SetPetAsync(string petId)
        {
            PlayerInfo.CurrentOutfit.PetId = petId;

            using var writer = Game.StartRpc(NetId, RpcCalls.SetPetStr);
            Rpc41SetPetStr.Serialize(writer, petId, PlayerInfo.GetNextRpcSequenceId(RpcCalls.SetPetStr));
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask SetSkinAsync(string skinId)
        {
            PlayerInfo.CurrentOutfit.SkinId = skinId;

            using var writer = Game.StartRpc(NetId, RpcCalls.SetSkinStr);
            Rpc40SetSkinStr.Serialize(writer, skinId, PlayerInfo.GetNextRpcSequenceId(RpcCalls.SetSkinStr));
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask SetVisorAsync(string visorId)
        {
            PlayerInfo.CurrentOutfit.VisorId = visorId;

            using var writer = Game.StartRpc(NetId, RpcCalls.SetVisorStr);
            Rpc42SetVisorStr.Serialize(writer, visorId, PlayerInfo.GetNextRpcSequenceId(RpcCalls.SetVisorStr));
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask SetNamePlateAsync(string nameplateId)
        {
            PlayerInfo.CurrentOutfit.NamePlateId = nameplateId;

            using var writer = Game.StartRpc(NetId, RpcCalls.SetNamePlateStr);
            Rpc43SetNamePlateStr.Serialize(writer, nameplateId, PlayerInfo.GetNextRpcSequenceId(RpcCalls.SetNamePlateStr));
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask SendChatAsync(string text)
        {
            using var writer = Game.StartRpc(NetId, RpcCalls.SendChat);
            writer.Write(text);
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask SendChatToPlayerAsync(string text, IInnerPlayerControl? player = null)
        {
            if (player == null)
            {
                player = this;
            }

            using var writer = Game.StartRpc(NetId, RpcCalls.SendChat);
            writer.Write(text);
            await Game.FinishRpcAsync(writer, player.OwnerId);
        }

        public async ValueTask MurderPlayerAsync(IInnerPlayerControl target, MurderResultFlags result)
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

            if (!result.IsFailed())
            {
                ((InnerPlayerControl)target).Die(DeathReason.Kill);
            }

            using var writer = Game.StartRpc(NetId, RpcCalls.MurderPlayer);
            Rpc12MurderPlayer.Serialize(writer, target, result);
            await Game.FinishRpcAsync(writer);

            await _eventManager.CallAsync(new PlayerMurderEvent(Game, Game.GetClientPlayer(OwnerId)!, this, target, result));
        }

        public async ValueTask MurderPlayerAsync(IInnerPlayerControl target)
        {
            await MurderPlayerAsync(target, MurderResultFlags.Succeeded);
        }

        public async ValueTask ProtectPlayerAsync(IInnerPlayerControl target)
        {
            if (target.PlayerInfo.RoleType == RoleTypes.GuardianAngel)
            {
                throw new ImpostorProtocolException("Tried to protect another Guardian Angel");
            }

            ((InnerPlayerControl)target).Protect(this);

            using var writer = Game.StartRpc(NetId, RpcCalls.ProtectPlayer);
            Rpc45ProtectPlayer.Serialize(writer, target, PlayerInfo.CurrentOutfit.Color);
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask ExileAsync()
        {
            if (PlayerInfo.IsDead)
            {
                throw new ImpostorProtocolException("Tried to exile a player, but target was not alive.");
            }

            // Update player.
            Die(DeathReason.Exile);

            // Send RPC.
            using var writer = Game.StartRpc(NetId, RpcCalls.Exiled);
            Rpc04Exiled.Serialize(writer);
            await Game.FinishRpcAsync(writer);

            // Notify plugins.
            await _eventManager.CallAsync(new PlayerExileEvent(Game, Game.GetClientPlayer(OwnerId)!, this));
        }

        public async ValueTask StartVanishAsync()
        {
            using var writer = Game.StartRpc(NetId, RpcCalls.StartVanish);
            Rpc63StartVanish.Serialize(writer);
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask StartAppearAsync(bool shouldAnimate)
        {
            using var writer = Game.StartRpc(NetId, RpcCalls.StartAppear);
            Rpc65StartAppear.Serialize(writer, shouldAnimate);
            await Game.FinishRpcAsync(writer);
        }
    }
}
