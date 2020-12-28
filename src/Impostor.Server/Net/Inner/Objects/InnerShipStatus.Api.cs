using System;
using System.Linq;
using System.Threading.Tasks;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Inner.Objects;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerShipStatus : IInnerShipStatus
    {
        public async ValueTask Sabotage(SystemTypes systemType)
        {
            switch (systemType)
            {
                case SystemTypes.Reactor:

                    if ((byte)_game.Options.Map > 1)
                    {
                        _logger.LogWarning("{0}: {1} is not a valid sabotage on map {2}", nameof(InnerShipStatus),
                            systemType, (byte)_game.Options.Map);
                        return;
                    }

                    break;

                case SystemTypes.Electrical:
                    break;

                case SystemTypes.LifeSupp:

                    if ((byte)_game.Options.Map > 1)
                    {
                        _logger.LogWarning("{0}: {1} is not a valid sabotage on map {2}", nameof(InnerShipStatus),
                            systemType, (byte)_game.Options.Map);
                        return;
                    }

                    break;

                case SystemTypes.Comms:
                    break;

                case SystemTypes.Laboratory:

                    if ((byte)_game.Options.Map != 2)
                    {
                        _logger.LogWarning("{0}: {1} is not a valid sabotage on map {2}", nameof(InnerShipStatus),
                            systemType, (byte)_game.Options.Map);
                        return;
                    }

                    break;

                default:
                    _logger.LogWarning("{0}: {1} is not a valid sabotage", nameof(InnerShipStatus), systemType);
                    return;
            }

            // Is this ok? Sabotages can only be triggered by impostors.
            // The anticheat will need to be modified in the future to ignore packets sent/modified by plugins
            var netId = _game.Players.First(p => p.Character.PlayerInfo.IsImpostor).Character.NetId;
            Console.WriteLine(netId);

            using var writer = _game.StartRpc(NetId, RpcCalls.RepairSystem);
            writer.Write((byte) SystemTypes.Sabotage);
            writer.WritePacked(netId);
            writer.Write((byte) systemType);
            await _game.FinishRpcAsync(writer);
        }
    }
}