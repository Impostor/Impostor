using System;
using System.Threading.Tasks;
using Hazel;
using Impostor.Server.Data;
using Impostor.Server.Games;
using Impostor.Server.Games.Managers;
using Impostor.Server.Hazel.Messages;
using Impostor.Server.Net.Manager;
using Impostor.Server.Net.Messages;
using Impostor.Shared.Innersloth;
using Impostor.Shared.Innersloth.Data;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Impostor.Server.Net
{
    internal class Client : ClientBase
    {
        private readonly ILogger<Client> _logger;
        private readonly IClientManager _clientManager;
        private readonly IGameManager _gameManager;

        public Client(ILogger<Client> logger, IClientManager clientManager, IGameManager gameManager, string name, IConnection connection)
            : base(name, connection)
        {
            _logger = logger;
            _clientManager = clientManager;
            _gameManager = gameManager;
        }

        public override async ValueTask HandleMessageAsync(IMessage message)
        {
            var reader = message.CreateReader();

            var flag = reader.Tag;

            _logger.LogTrace("[{0}] Server got {1}.", Id, flag);

            switch (flag)
            {
                case MessageFlags.HostGame:
                {
                    // Read game settings.
                    var gameInfo = Message00HostGame.Deserialize(reader);

                    // Create game.
                    var game = await _gameManager.CreateAsync(gameInfo);

                    // Code in the packet below will be used in JoinGame.
                    using var writer = Connection.CreateMessage(MessageType.Reliable);
                    Message00HostGame.Serialize(writer, game.Code);

                    await writer.SendAsync();

                    break;
                }

                case MessageFlags.JoinGame:
                {
                    Message01JoinGame.Deserialize(
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

                    Message04RemovePlayer.Deserialize(
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

                    // Broadcast packet to all other players.
                    using var writer = Player.Game.CreateMessage(message.Type);

                    if (flag == MessageFlags.GameDataTo)
                    {
                        var target = reader.ReadPackedInt32();
                        reader.CopyTo(writer);
                        await writer.SendToAsync(target);
                    }
                    else
                    {
                        reader.CopyTo(writer);
                        await writer.SendToAllExceptAsync(Id);
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

                    Message10AlterGame.Deserialize(
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

                    Message11KickPlayer.Deserialize(
                        reader,
                        out var playerId,
                        out var isBan);

                    await Player.Game.HandleKickPlayer(playerId, isBan);
                    break;
                }

                case MessageFlags.GetGameListV2:
                {
                    Message16GetGameListV2.Deserialize(reader, out var options);
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
        private async ValueTask OnRequestGameList(GameOptionsData options)
        {
            using var message = Connection.CreateMessage(MessageType.Reliable);
            var games = _gameManager.FindListings((MapFlags)options.MapId, options.NumImpostors, options.Keywords);

            var skeldGameCount = _gameManager.GetGameCount(MapFlags.Skeld);
            var miraHqGameCount = _gameManager.GetGameCount(MapFlags.MiraHQ);
            var polusGameCount = _gameManager.GetGameCount(MapFlags.Polus);

            Message16GetGameListV2.Serialize(message, skeldGameCount, miraHqGameCount, polusGameCount, games);

            await message.SendAsync();
        }

        private async ValueTask SendDisconnectReason(DisconnectReason reason, string message = null)
        {
            if (Connection == null)
            {
                return;
            }

            using var packet = Connection.CreateMessage(MessageType.Reliable);
            Message01JoinGame.SerializeError(packet, false, reason, message);
            await packet.SendAsync();
        }
    }
}