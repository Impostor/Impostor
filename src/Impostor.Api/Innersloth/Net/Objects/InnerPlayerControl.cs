using System;
using Impostor.Api.Games;
using Impostor.Api.Innersloth.Data;
using Impostor.Api.Innersloth.Net.Objects.Components;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Impostor.Api.Innersloth.Net.Objects
{
    public class InnerPlayerControl : InnerNetObject
    {
        private readonly ILogger<InnerPlayerControl> _logger;
        private readonly IGame _game;

        public InnerPlayerControl(ILogger<InnerPlayerControl> logger, IServiceProvider serviceProvider, IGame game)
        {
            _logger = logger;
            _game = game;

            Components.Add(this);
            Components.Add(ActivatorUtilities.CreateInstance<InnerPlayerPhysics>(serviceProvider));
            Components.Add(ActivatorUtilities.CreateInstance<InnerCustomNetworkTransform>(serviceProvider));

            PlayerId = byte.MaxValue;
        }

        public bool IsNew { get; private set; }

        public byte PlayerId { get; private set; }

        public InnerGameData.PlayerInfo PlayerInfo { get; internal set; }

        private void Die(DeathReason reason)
        {
            PlayerInfo.IsDead = true;
            PlayerInfo.LastDeathReason = reason;
        }

        public override void HandleRpc(IClientPlayer sender, IClientPlayer target, RpcCalls call, IMessageReader reader)
        {
            switch (call)
            {
                case RpcCalls.PlayAnimation:
                {
                    var animation = reader.ReadByte();
                    break;
                }

                case RpcCalls.CompleteTask:
                {
                    var index = reader.ReadPackedUInt32();
                    break;
                }

                case RpcCalls.SyncSettings:
                {
                    _game.Options.Deserialize(reader.ReadBytesAndSize());
                    Console.WriteLine(_game.Options.PlayerSpeedMod);
                    break;
                }

                // Set Impostors.
                case RpcCalls.SetInfected:
                {
                    var length = reader.ReadPackedInt32();

                    for (var i = 0; i < length; i++)
                    {
                        var playerId = reader.ReadByte();
                        var player = _game.GameNet.GameData.GetPlayerById(playerId);

                        player.IsImpostor = true;
                    }

                    break;
                }

                // Player was voted out.
                case RpcCalls.Exiled:
                {
                    Console.WriteLine(PlayerInfo.PlayerName + " was voted out.");
                    break;
                }

                // Validates the player name at the host.
                case RpcCalls.CheckName:
                {
                    if (!target.IsHost)
                    {

                    }
                    var name = reader.ReadString();
                    break;
                }

                case RpcCalls.SetName:
                {
                    PlayerInfo.PlayerName = reader.ReadString();
                    break;
                }

                default:
                    _logger.LogWarning("InnerPlayerControl: Unknown rpc call {0}", call);
                    break;
            }
        }

        public override bool Serialize(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
        }

        public override void Deserialize(IClientPlayer sender, IMessageReader reader, bool initialState)
        {
            if (initialState)
            {
                IsNew = reader.ReadBoolean();
            }

            PlayerId = reader.ReadByte();
        }
    }
}