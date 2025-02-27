using System.Diagnostics.CodeAnalysis;
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

        IInnerPlayerInfo? IInnerPlayerControl.PlayerInfo => PlayerInfo;

        public async ValueTask SetNameAsync(string name)
        {
            if (PlayerInfo == null)
            {
                throw new ImpostorProtocolException("Cannot set name, PlayerInfo is null");
            }

            PlayerInfo.CurrentOutfit.PlayerName = name;

            using var writer = Game.StartRpc(NetId, RpcCalls.SetName);
            Rpc06SetName.Serialize(writer, PlayerInfo.NetId, name);
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask SetColorAsync(ColorType color)
        {
            if (PlayerInfo == null)
            {
                throw new ImpostorProtocolException("Cannot set color, PlayerInfo is null");
            }

            PlayerInfo.CurrentOutfit.Color = color;

            using var writer = Game.StartRpc(NetId, RpcCalls.SetColor);
            Rpc08SetColor.Serialize(writer, PlayerInfo.NetId, color);
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask SetHatAsync(string hatId)
        {
            if (PlayerInfo == null)
            {
                throw new ImpostorProtocolException("Cannot set hat, PlayerInfo is null");
            }

            PlayerInfo.CurrentOutfit.HatId = hatId;

            using var writer = Game.StartRpc(NetId, RpcCalls.SetHatStr);
            Rpc39SetHatStr.Serialize(writer, hatId, PlayerInfo.GetNextRpcSequenceId(RpcCalls.SetHatStr));
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask SetPetAsync(string petId)
        {
            if (PlayerInfo == null)
            {
                throw new ImpostorProtocolException("Cannot set pet, PlayerInfo is null");
            }

            PlayerInfo.CurrentOutfit.PetId = petId;

            using var writer = Game.StartRpc(NetId, RpcCalls.SetPetStr);
            Rpc41SetPetStr.Serialize(writer, petId, PlayerInfo.GetNextRpcSequenceId(RpcCalls.SetPetStr));
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask SetSkinAsync(string skinId)
        {
            if (PlayerInfo == null)
            {
                throw new ImpostorProtocolException("Cannot set skin, PlayerInfo is null");
            }

            PlayerInfo.CurrentOutfit.SkinId = skinId;

            using var writer = Game.StartRpc(NetId, RpcCalls.SetSkinStr);
            Rpc40SetSkinStr.Serialize(writer, skinId, PlayerInfo.GetNextRpcSequenceId(RpcCalls.SetSkinStr));
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask SetVisorAsync(string visorId)
        {
            if (PlayerInfo == null)
            {
                throw new ImpostorProtocolException("Cannot set visor, PlayerInfo is null");
            }

            PlayerInfo.CurrentOutfit.VisorId = visorId;

            using var writer = Game.StartRpc(NetId, RpcCalls.SetVisorStr);
            Rpc42SetVisorStr.Serialize(writer, visorId, PlayerInfo.GetNextRpcSequenceId(RpcCalls.SetVisorStr));
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask SetNamePlateAsync(string nameplateId)
        {
            if (PlayerInfo == null)
            {
                throw new ImpostorProtocolException("Cannot set nameplate, PlayerInfo is null");
            }

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

        private bool ValidateMurderPlayer(IInnerPlayerControl target, MurderResultFlags result, [NotNullWhen(false)] out string? invalidReason)
        {
            if (PlayerInfo == null)
            {
                invalidReason = "Tried to murder a player, but the murderer didn't have a playerinfo";
            }
            else if (!PlayerInfo.IsImpostor)
            {
                invalidReason = "Tried to murder a player, but murderer was not an impostor.";
            }
            else if (PlayerInfo.IsDead)
            {
                invalidReason = "Tried to murder a player, but murderer was not alive.";
            }
            else if (target.PlayerInfo == null)
            {
                invalidReason = "Tried to murder a player, but the murderer didn't have a playerinfo";
            }
            else if (target.PlayerInfo.IsImpostor)
            {
                invalidReason = "Tried to murder a player, but target is an impostor";
            }
            else if (target.PlayerInfo.IsDead)
            {
                invalidReason = "Tried to murder a player, but target was not alive.";
            }
            else
            {
                invalidReason = null;
                return true;
            }

            return false;
        }

        public async ValueTask MurderPlayerAsync(IInnerPlayerControl target, MurderResultFlags result)
        {
            if (!ValidateMurderPlayer(target, result, out var reason))
            {
                throw new ImpostorProtocolException(reason);
            }

            await ForceMurderPlayerAsync(target, result);
        }

        public async ValueTask ForceMurderPlayerAsync(IInnerPlayerControl target, MurderResultFlags result)
        {
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
            if (target.PlayerInfo == null)
            {
                throw new ImpostorProtocolException("Tried to exile a player, but target didn't have a playerinfo");
            }
            else if (target.PlayerInfo.IsDead)
            {
                throw new ImpostorProtocolException("Tried to protect a player that is dead");
            }

            await ForceProtectPlayerAsync(target);
        }

        public async ValueTask ForceProtectPlayerAsync(IInnerPlayerControl target)
        {
            ((InnerPlayerControl)target).Protect(this);

            using var writer = Game.StartRpc(NetId, RpcCalls.ProtectPlayer);
            Rpc45ProtectPlayer.Serialize(writer, target, PlayerInfo?.CurrentOutfit.Color ?? ColorType.Red);
            await Game.FinishRpcAsync(writer);
        }

        public async ValueTask ExileAsync()
        {
            if (PlayerInfo == null)
            {
                throw new ImpostorProtocolException("Tried to exile a player, but target didn't have a playerinfo");
            }
            else if (PlayerInfo.IsDead)
            {
                throw new ImpostorProtocolException("Tried to exile a player, but target was not alive.");
            }

            await ForceExileAsync();
        }

        public async ValueTask ForceExileAsync()
        {
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
