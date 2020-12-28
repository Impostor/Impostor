using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Messages;
using Impostor.Api.Net.Messages.S2C;
using Impostor.Hazel;
using Impostor.Server.Events.Meeting;
using Impostor.Server.Events.Player;
using Impostor.Server.Net.Inner;
using Impostor.Server.Net.Inner.Objects;
using Impostor.Server.Net.Inner.Objects.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.State
{
    internal partial class Game
    {
        private const int FakeClientId = int.MaxValue - 1;

        /// <summary>
        ///     Used for global object, spawned by the host.
        /// </summary>
        private const int InvalidClient = -2;

        /// <summary>
        ///     Used internally to set the OwnerId to the current ClientId.
        ///     i.e: <code>ownerId = ownerId == -3 ? this.ClientId : ownerId;</code>
        /// </summary>
        private const int CurrentClient = -3;

        private static readonly Type[] SpawnableObjects =
        {
            typeof(InnerShipStatus), // ShipStatus
            typeof(InnerMeetingHud),
            typeof(InnerLobbyBehaviour),
            typeof(InnerGameData),
            typeof(InnerPlayerControl),
            typeof(InnerShipStatus), // HeadQuarters
            typeof(InnerShipStatus), // PlanetMap
            typeof(InnerShipStatus), // AprilShipStatus
        };

        private readonly List<InnerNetObject> _allObjects = new List<InnerNetObject>();
        private readonly Dictionary<uint, InnerNetObject> _allObjectsFast = new Dictionary<uint, InnerNetObject>();

        private int _gamedataInitialized;
        private bool _gamedataFakeReceived;

        private async ValueTask OnSpawnAsync(InnerNetObject netObj)
        {
            switch (netObj)
            {
                case InnerLobbyBehaviour lobby:
                {
                    GameNet.LobbyBehaviour = lobby;
                    break;
                }

                case InnerGameData data:
                {
                    GameNet.GameData = data;
                    break;
                }

                case InnerVoteBanSystem voteBan:
                {
                    GameNet.VoteBan = voteBan;
                    break;
                }

                case InnerShipStatus shipStatus:
                {
                    GameNet.ShipStatus = shipStatus;
                    break;
                }

                case InnerPlayerControl control:
                {
                    // Hook up InnerPlayerControl <-> IClientPlayer.
                    if (!TryGetPlayer(control.OwnerId, out var player))
                    {
                        throw new ImpostorException("Failed to find player that spawned the InnerPlayerControl");
                    }

                    player.Character = control;
                    player.DisableSpawnTimeout();

                    // Hook up InnerPlayerControl <-> InnerPlayerControl.PlayerInfo.
                    control.PlayerInfo = GameNet.GameData.GetPlayerById(control.PlayerId)!;

                    if (control.PlayerInfo == null)
                    {
                        GameNet.GameData.AddPlayer(control);
                    }

                    if (control.PlayerInfo != null)
                    {
                        control.PlayerInfo!.Controller = control;
                    }

                    await _eventManager.CallAsync(new PlayerSpawnedEvent(this, player, control));

                    break;
                }

                case InnerMeetingHud meetingHud:
                {
                    await _eventManager.CallAsync(new MeetingStartedEvent(this, meetingHud));
                    break;
                }
            }
        }

        private async ValueTask OnDestroyAsync(InnerNetObject netObj)
        {
            switch (netObj)
            {
                case InnerLobbyBehaviour:
                {
                    GameNet.LobbyBehaviour = null;
                    break;
                }

                case InnerGameData:
                {
                    GameNet.GameData = null;
                    break;
                }

                case InnerVoteBanSystem:
                {
                    GameNet.VoteBan = null;
                    break;
                }

                case InnerShipStatus:
                {
                    GameNet.ShipStatus = null;
                    break;
                }

                case InnerPlayerControl control:
                {
                    // Remove InnerPlayerControl <-> IClientPlayer.
                    if (TryGetPlayer(control.OwnerId, out var player))
                    {
                        player.Character = null;
                    }

                    await _eventManager.CallAsync(new PlayerDestroyedEvent(this, player, control));

                    break;
                }
            }
        }

        private async ValueTask InitGameDataAsync(ClientPlayer player)
        {
            if (Interlocked.Exchange(ref _gamedataInitialized, 1) != 0)
            {
                return;
            }

            /*
             * The Among Us client on 20.9.22i spawns some components on the host side and
             * only spawns these on other clients when someone else connects. This means that we can't
             * parse data until someone connects because we don't know which component belongs to the NetId.
             *
             * We solve this by spawning a fake player and removing the player when the spawn GameData
             * is received in HandleGameDataAsync.
             */
            using (var message = MessageWriter.Get(MessageType.Reliable))
            {
                // Spawn a fake player.
                Message01JoinGameS2C.SerializeJoin(message, false, Code, FakeClientId, HostId);

                message.StartMessage(MessageFlags.GameData);
                message.Write(Code);
                message.StartMessage(GameDataTag.SceneChangeFlag);
                message.WritePacked(FakeClientId);
                message.Write("OnlineGame");
                message.EndMessage();
                message.EndMessage();

                await player.Client.Connection.SendAsync(message);
            }
        }

        public async ValueTask<bool> HandleGameDataAsync(IMessageReader parent, ClientPlayer sender, bool toPlayer)
        {
            // Find target player.
            ClientPlayer target = null;

            if (toPlayer)
            {
                var targetId = parent.ReadPackedInt32();
                if (targetId == FakeClientId && !_gamedataFakeReceived && sender.IsHost)
                {
                    _gamedataFakeReceived = true;

                    // Remove the fake client, we received the data.
                    using (var message = MessageWriter.Get(MessageType.Reliable))
                    {
                        WriteRemovePlayerMessage(message, false, FakeClientId, (byte)DisconnectReason.ExitGame);

                        await sender.Client.Connection.SendAsync(message);
                    }
                }
                else if (!TryGetPlayer(targetId, out target))
                {
                    _logger.LogWarning("Player {0} tried to send GameData to unknown player {1}.", sender.Client.Id, targetId);
                    return false;
                }

                _logger.LogTrace("Received GameData for target {0}.", targetId);
            }

            // Parse GameData messages.
            while (parent.Position < parent.Length)
            {
                using var reader = parent.ReadMessage();

                switch (reader.Tag)
                {
                    case GameDataTag.DataFlag:
                    {
                        var netId = reader.ReadPackedUInt32();
                        if (_allObjectsFast.TryGetValue(netId, out var obj))
                        {
                            await obj.DeserializeAsync(sender, target, reader, false);
                        }
                        else
                        {
                            _logger.LogWarning("Received DataFlag for unregistered NetId {0}.", netId);
                        }

                        break;
                    }

                    case GameDataTag.RpcFlag:
                    {
                        var netId = reader.ReadPackedUInt32();
                        if (_allObjectsFast.TryGetValue(netId, out var obj))
                        {
                            await obj.HandleRpc(sender, target, (RpcCalls) reader.ReadByte(), reader);
                        }
                        else
                        {
                            _logger.LogWarning("Received RpcFlag for unregistered NetId {0}.", netId);
                        }

                        break;
                    }

                    case GameDataTag.SpawnFlag:
                    {
                        // Only the host is allowed to despawn objects.
                        if (!sender.IsHost)
                        {
                            throw new ImpostorCheatException("Tried to send SpawnFlag as non-host.");
                        }

                        var objectId = reader.ReadPackedUInt32();
                        if (objectId < SpawnableObjects.Length)
                        {
                            var innerNetObject = (InnerNetObject) ActivatorUtilities.CreateInstance(_serviceProvider, SpawnableObjects[objectId], this);
                            var ownerClientId = reader.ReadPackedInt32();

                            // Prevent fake client from being broadcasted.
                            // TODO: Remove message from stream properly.
                            if (ownerClientId == FakeClientId)
                            {
                                return false;
                            }

                            innerNetObject.SpawnFlags = (SpawnFlags) reader.ReadByte();

                            var components = innerNetObject.GetComponentsInChildren<InnerNetObject>();
                            var componentsCount = reader.ReadPackedInt32();

                            if (componentsCount != components.Count)
                            {
                                _logger.LogError(
                                    "Children didn't match for spawnable {0}, name {1} ({2} != {3})",
                                    objectId,
                                    innerNetObject.GetType().Name,
                                    componentsCount,
                                    components.Count);
                                continue;
                            }

                            _logger.LogDebug(
                                "Spawning {0} components, SpawnFlags {1}",
                                innerNetObject.GetType().Name,
                                innerNetObject.SpawnFlags);

                            for (var i = 0; i < componentsCount; i++)
                            {
                                var obj = components[i];

                                obj.NetId = reader.ReadPackedUInt32();
                                obj.OwnerId = ownerClientId;

                                _logger.LogDebug(
                                    "- {0}, NetId {1}, OwnerId {2}",
                                    obj.GetType().Name,
                                    obj.NetId,
                                    obj.OwnerId);

                                if (!AddNetObject(obj))
                                {
                                    _logger.LogTrace("Failed to AddNetObject, it already exists.");

                                    obj.NetId = uint.MaxValue;
                                    break;
                                }

                                using var readerSub = reader.ReadMessage();
                                if (readerSub.Length > 0)
                                {
                                    await obj.DeserializeAsync(sender, target, readerSub, true);
                                }

                                await OnSpawnAsync(obj);
                            }

                            continue;
                        }

                        _logger.LogError("Couldn't find spawnable object {0}.", objectId);
                        break;
                    }

                    // Only the host is allowed to despawn objects.
                    case GameDataTag.DespawnFlag:
                    {
                        var netId = reader.ReadPackedUInt32();
                        if (_allObjectsFast.TryGetValue(netId, out var obj))
                        {
                            if (sender.Client.Id != obj.OwnerId && !sender.IsHost)
                            {
                                _logger.LogWarning(
                                    "Player {0} ({1}) tried to send DespawnFlag for {2} but was denied.",
                                    sender.Client.Name,
                                    sender.Client.Id,
                                    netId);
                                return false;
                            }

                            RemoveNetObject(obj);
                            await OnDestroyAsync(obj);
                            _logger.LogDebug("Destroyed InnerNetObject {0} ({1}), OwnerId {2}", obj.GetType().Name, netId, obj.OwnerId);
                        }
                        else
                        {
                            _logger.LogDebug(
                                "Player {0} ({1}) sent DespawnFlag for unregistered NetId {2}.",
                                sender.Client.Name,
                                sender.Client.Id,
                                netId);
                        }

                        break;
                    }

                    case GameDataTag.SceneChangeFlag:
                    {
                        // Sender is only allowed to change his own scene.
                        var clientId = reader.ReadPackedInt32();
                        if (clientId != sender.Client.Id)
                        {
                            _logger.LogWarning(
                                "Player {0} ({1}) tried to send SceneChangeFlag for another player.",
                                sender.Client.Name,
                                sender.Client.Id);
                            return false;
                        }

                        sender.Scene = reader.ReadString();

                        _logger.LogTrace("> Scene {0} to {1}", clientId, sender.Scene);
                        break;
                    }

                    case GameDataTag.ReadyFlag:
                    {
                        var clientId = reader.ReadPackedInt32();
                        _logger.LogTrace("> IsReady {0}", clientId);
                        break;
                    }

                    default:
                    {
                        _logger.LogTrace("Bad GameData tag {0}", reader.Tag);
                        break;
                    }
                }

                if (sender.Client.Player == null)
                {
                    // Disconnect handler was probably invoked, cancel the rest.
                    return false;
                }
            }

            return true;
        }

        private bool AddNetObject(InnerNetObject obj)
        {
            if (_allObjectsFast.ContainsKey(obj.NetId))
            {
                return false;
            }

            _allObjects.Add(obj);
            _allObjectsFast.Add(obj.NetId, obj);
            return true;
        }

        private void RemoveNetObject(InnerNetObject obj)
        {
            var index = _allObjects.IndexOf(obj);
            if (index > -1)
            {
                _allObjects.RemoveAt(index);
            }

            _allObjectsFast.Remove(obj.NetId);

            obj.NetId = uint.MaxValue;
        }

        public T FindObjectByNetId<T>(uint netId)
            where T : InnerNetObject
        {
            if (_allObjectsFast.TryGetValue(netId, out var obj))
            {
                return (T) obj;
            }

            return null;
        }
    }
}
