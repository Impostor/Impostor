using System;
using System.Threading.Tasks;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.Data;
using Impostor.Api.Net.Messages;
using Impostor.Api.Net.Messages.C2S;
using Impostor.Api.Net.Messages.S2C;
using Impostor.Hazel;
using Impostor.Server.Data;
using Impostor.Server.Net.Hazel;
using Impostor.Server.Net.Manager;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net
{
    internal class Client : ClientBase
    {
        private readonly ILogger<Client> _logger;
        private readonly ClientManager _clientManager;
        private readonly GameManager _gameManager;

        public Client(ILogger<Client> logger, ClientManager clientManager, GameManager gameManager, string name, HazelConnection connection)
            : base(name, connection)
        {
            _logger = logger;
            _clientManager = clientManager;
            _gameManager = gameManager;
        }

        public override async ValueTask HandleMessageAsync(IMessageReader reader, MessageType messageType)
        {
            var flag = reader.Tag;

            _logger.LogTrace("[{0}] Server got {1}.", Id, flag);

            switch (flag)
            {
                case MessageFlags.HostGame:
                {
                    // Read game settings.
                    var gameInfo = Message00HostGameC2S.Deserialize(reader);

                    // Create game.
                    var game = await _gameManager.CreateAsync(gameInfo);

                    // Code in the packet below will be used in JoinGame.
                    using (var writer = MessageWriter.Get(MessageType.Reliable))
                    {
                        Message00HostGameS2C.Serialize(writer, game.Code);
                        await Connection.SendAsync(writer);
                    }

                    break;
                }

                case MessageFlags.JoinGame:
                {
                    Message01JoinGameC2S.Deserialize(
                        reader,
                        out var gameCode,
                        out _);

                    var game = _gameManager.Find(gameCode);
                    if (game == null)
                    {
                        await SendDisconnectReason(DisconnectReason.GameMissing);
                        return;
                    }

                    var result = await game.AddClientAsync(this);

                    switch (result.Error)
                    {
                        case GameJoinError.None:
                            break;
                        case GameJoinError.InvalidClient:
                            await SendDisconnectReason(DisconnectReason.Custom, "Client is in an invalid state.");
                            break;
                        case GameJoinError.Banned:
                            await SendDisconnectReason(DisconnectReason.Banned);
                            break;
                        case GameJoinError.GameFull:
                            await SendDisconnectReason(DisconnectReason.GameFull);
                            break;
                        case GameJoinError.InvalidLimbo:
                            await SendDisconnectReason(DisconnectReason.Custom, "Invalid limbo state while joining.");
                            break;
                        case GameJoinError.GameStarted:
                            await SendDisconnectReason(DisconnectReason.GameStarted);
                            break;
                        case GameJoinError.GameDestroyed:
                            await SendDisconnectReason(DisconnectReason.Custom, DisconnectMessages.Destroyed);
                            break;
                        case GameJoinError.Custom:
                            await SendDisconnectReason(DisconnectReason.Custom, result.Message);
                            break;
                        default:
                            await SendDisconnectReason(DisconnectReason.Custom, "Unknown error.");
                            break;
                    }

                    break;
                }

                case MessageFlags.StartGame:
                {
                    if (!IsPacketAllowed(reader, true))
                    {
                        return;
                    }

                    await Player.Game.HandleStartGame(reader);
                    break;
                }

                // No idea how this flag is triggered.
                case MessageFlags.RemoveGame:
                    break;

                case MessageFlags.RemovePlayer:
                {
                    if (!IsPacketAllowed(reader, true))
                    {
                        return;
                    }

                    Message04RemovePlayerC2S.Deserialize(
                        reader,
                        out var playerId,
                        out var reason);

                    await Player.Game.HandleRemovePlayer(playerId, (DisconnectReason)reason);
                    break;
                }

                case MessageFlags.GameData:
                case MessageFlags.GameDataTo:
                {
                    if (!IsPacketAllowed(reader, false))
                    {
                        return;
                    }

                    var toPlayer = flag == MessageFlags.GameDataTo;

                    // Handle packet.
                    var readerCopy = reader.Slice(reader.Position);

                    // TODO: Return value, either a bool (to cancel) or a writer (to cancel (null) or modify/overwrite).
                    await Player.Game.HandleGameData(readerCopy, Player, toPlayer);

                    // Broadcast packet to all other players.
                    using (var writer = MessageWriter.Get(messageType))
                    {
                        if (toPlayer)
                        {
                            var target = reader.ReadPackedInt32();
                            reader.CopyTo(writer);
                            await Player.Game.SendToAsync(writer, target);
                        }
                        else
                        {
                            reader.CopyTo(writer);
                            await Player.Game.SendToAllExceptAsync(writer, Id);
                        }
                    }

                    break;
                }

                case MessageFlags.EndGame:
                {
                    if (!IsPacketAllowed(reader, true))
                    {
                        return;
                    }

                    await Player.Game.HandleEndGame(reader);
                    break;
                }

                case MessageFlags.AlterGame:
                {
                    if (!IsPacketAllowed(reader, true))
                    {
                        return;
                    }

                    Message10AlterGameC2S.Deserialize(
                        reader,
                        out var gameTag,
                        out var value);

                    if (gameTag != AlterGameTags.ChangePrivacy)
                    {
                        return;
                    }

                    await Player.Game.HandleAlterGame(reader, Player, value);
                    break;
                }

                case MessageFlags.KickPlayer:
                {
                    if (!IsPacketAllowed(reader, true))
                    {
                        return;
                    }

                    Message11KickPlayerC2S.Deserialize(
                        reader,
                        out var playerId,
                        out var isBan);

                    await Player.Game.HandleKickPlayer(playerId, isBan);
                    break;
                }

                case MessageFlags.GetGameListV2:
                {
                    Message16GetGameListC2S.Deserialize(reader, out var options);
                    await OnRequestGameList(options);
                    break;
                }

                default:
                    _logger.LogWarning("Server received unknown flag {0}.", flag);
                    break;
            }

#if DEBUG
            if (flag != MessageFlags.GameData &&
                flag != MessageFlags.GameDataTo &&
                flag != MessageFlags.EndGame &&
                reader.Position < reader.Length)
            {
                _logger.LogWarning(
                    "Server did not consume all bytes from {0} ({1} < {2}).",
                    flag,
                    reader.Position,
                    reader.Length);
            }
#endif
        }

        public override async ValueTask HandleDisconnectAsync(string reason)
        {
            try
            {
                if (Player != null)
                {
                    await Player.Game.HandleRemovePlayer(Id, DisconnectReason.ExitGame);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception caught in client disconnection.");
            }

            _logger.LogInformation("Client disconnecting, reason: {0}.", reason);
            _clientManager.Remove(this);
        }

        private bool IsPacketAllowed(IMessageReader message, bool hostOnly)
        {
            if (Player == null)
            {
                return false;
            }

            var game = Player.Game;

            // GameCode must match code of the current game assigned to the player.
            if (message.ReadInt32() != game.Code)
            {
                return false;
            }

            // Some packets should only be sent by the host of the game.
            if (hostOnly)
            {
                if (game.HostId == Id)
                {
                    return true;
                }

                _logger.LogWarning("[{0}] Client sent packet only allowed by the host ({1}).", Id, game.HostId);
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Triggered when the connected client requests the game listing.
        /// </summary>
        /// <param name="options">
        ///     All options given.
        ///     At this moment, the client can only specify the map, impostor count and chat language.
        /// </param>
        private ValueTask OnRequestGameList(GameOptionsData options)
        {
            using var message = MessageWriter.Get(MessageType.Reliable);

            var games = _gameManager.FindListings((MapFlags)options.MapId, options.NumImpostors, options.Keywords);

            var skeldGameCount = _gameManager.GetGameCount(MapFlags.Skeld);
            var miraHqGameCount = _gameManager.GetGameCount(MapFlags.MiraHQ);
            var polusGameCount = _gameManager.GetGameCount(MapFlags.Polus);

            Message16GetGameListS2C.Serialize(message, skeldGameCount, miraHqGameCount, polusGameCount, games);

            return Connection.SendAsync(message);
        }

        private ValueTask SendDisconnectReason(DisconnectReason reason, string message = null)
        {
            if (Connection == null)
            {
                return default;
            }

            using var packet = MessageWriter.Get(MessageType.Reliable);
            Message01JoinGameS2C.SerializeError(packet, false, reason, message);
            return Connection.SendAsync(packet);
        }
    }
}