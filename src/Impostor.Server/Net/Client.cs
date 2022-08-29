using System;
using System.Linq;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Config;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Custom;
using Impostor.Api.Net.Messages;
using Impostor.Api.Net.Messages.C2S;
using Impostor.Api.Net.Messages.S2C;
using Impostor.Hazel;
using Impostor.Server.Net.Manager;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Net
{
    internal class Client : ClientBase
    {
        private readonly ILogger<Client> _logger;
        private readonly AntiCheatConfig _antiCheatConfig;
        private readonly ClientManager _clientManager;
        private readonly GameManager _gameManager;
        private readonly ICustomMessageManager<ICustomRootMessage> _customMessageManager;

        public Client(ILogger<Client> logger, IOptions<AntiCheatConfig> antiCheatOptions, ClientManager clientManager, GameManager gameManager, ICustomMessageManager<ICustomRootMessage> customMessageManager, string name, int gameVersion, Language language, QuickChatModes chatMode, PlatformSpecificData platformSpecificData, IHazelConnection connection)
            : base(name, gameVersion, language, chatMode, platformSpecificData, connection)
        {
            _logger = logger;
            _antiCheatConfig = antiCheatOptions.Value;
            _clientManager = clientManager;
            _gameManager = gameManager;
            _customMessageManager = customMessageManager;
        }

        public override async ValueTask<bool> ReportCheatAsync(CheatContext context, string message)
        {
            _logger.LogWarning("Client {Name} ({Id}) was caught cheating: [{Context}] {Message}", Name, Id, context.Name, message);

            if (!_antiCheatConfig.Enabled)
            {
                return false;
            }

            if (_antiCheatConfig.BanIpFromGame)
            {
                Player?.Game.BanIp(Connection.EndPoint.Address);
            }

            await DisconnectAsync(DisconnectReason.Hacking, context.Name + ": " + message);

            return true;
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
                    var game = await _gameManager.CreateAsync(this, gameInfo);

                    if (game == null)
                    {
                        await DisconnectAsync(DisconnectReason.GameMissing);
                        return;
                    }

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
                    Message01JoinGameC2S.Deserialize(reader, out var gameCode);

                    var game = _gameManager.Find(gameCode);
                    if (game == null)
                    {
                        await DisconnectAsync(DisconnectReason.GameMissing);
                        return;
                    }

                    var result = await game.AddClientAsync(this);

                    switch (result.Error)
                    {
                        case GameJoinError.None:
                            break;
                        case GameJoinError.InvalidClient:
                            await DisconnectAsync(DisconnectReason.Custom, "Client is in an invalid state.");
                            break;
                        case GameJoinError.Banned:
                            await DisconnectAsync(DisconnectReason.Banned);
                            break;
                        case GameJoinError.GameFull:
                            await DisconnectAsync(DisconnectReason.GameFull);
                            break;
                        case GameJoinError.InvalidLimbo:
                            await DisconnectAsync(DisconnectReason.Custom, "Invalid limbo state while joining.");
                            break;
                        case GameJoinError.GameStarted:
                            await DisconnectAsync(DisconnectReason.GameStarted);
                            break;
                        case GameJoinError.GameDestroyed:
                            await DisconnectAsync(DisconnectReason.Custom, DisconnectMessages.Destroyed);
                            break;
                        case GameJoinError.ClientOutdated:
                            await DisconnectAsync(DisconnectReason.Custom, DisconnectMessages.ClientOutdated);
                            break;
                        case GameJoinError.ClientTooNew:
                            await DisconnectAsync(DisconnectReason.Custom, DisconnectMessages.ClientTooNew);
                            break;
                        case GameJoinError.Custom:
                            await DisconnectAsync(DisconnectReason.Custom, result.Message);
                            break;
                        default:
                            await DisconnectAsync(DisconnectReason.Custom, "Unknown error.");
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

                    await Player!.Game.HandleStartGame(reader);
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

                    await Player!.Game.HandleRemovePlayer(playerId, (DisconnectReason)reason);
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

                    var position = reader.Position;
                    var verified = await Player!.Game.HandleGameDataAsync(reader, Player, toPlayer);
                    reader.Seek(position);

                    if (verified && Player != null)
                    {
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
                    }

                    break;
                }

                case MessageFlags.EndGame:
                {
                    if (!IsPacketAllowed(reader, true))
                    {
                        return;
                    }

                    Message08EndGameC2S.Deserialize(
                        reader,
                        out var gameOverReason);

                    await Player!.Game.HandleEndGame(reader, gameOverReason);
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

                    await Player!.Game.HandleAlterGame(reader, Player, value);
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

                    await Player!.Game.HandleKickPlayer(playerId, isBan);
                    break;
                }

                case MessageFlags.GetGameListV2:
                {
                    Message16GetGameListC2S.Deserialize(reader, out var options, out _);
                    await OnRequestGameListAsync(options);
                    break;
                }

                case MessageFlags.SetActivePodType:
                {
                    Message21SetActivePodType.Deserialize(reader, out _);
                    break;
                }

                case MessageFlags.QueryPlatformIds:
                {
                    Message22QueryPlatformIdsC2S.Deserialize(reader, out var gameCode);
                    await OnQueryPlatformIds(gameCode);
                    break;
                }

                default:
                    if (_customMessageManager.TryGet(flag, out var customRootMessage))
                    {
                        await customRootMessage.HandleMessageAsync(this, reader, messageType);
                        break;
                    }

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

            _logger.LogInformation("Client {0} disconnecting, reason: {1}", Id, reason);
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
        private ValueTask OnRequestGameListAsync(GameOptionsData options)
        {
            using var message = MessageWriter.Get(MessageType.Reliable);

            var games = _gameManager.FindListings((MapFlags)options.Map, options.NumImpostors, options.Keywords, this.GameVersion);

            Message16GetGameListS2C.Serialize(message, games);

            return Connection.SendAsync(message);
        }

        /// <summary>
        ///     Triggered when the connected client requests the PlatformSpecificData.
        /// </summary>
        /// <param name="code">
        ///     The GameCode of the game whose platform id's are checked.
        /// </param>
        private ValueTask OnQueryPlatformIds(GameCode code)
        {
            using var message = MessageWriter.Get(MessageType.Reliable);

            var playerSpecificData = _gameManager.Find(code)?.Players.Select(p => p.Client.PlatformSpecificData) ?? Enumerable.Empty<PlatformSpecificData>();

            Message22QueryPlatformIdsS2C.Serialize(message, code, playerSpecificData);

            return Connection.SendAsync(message);
        }
    }
}
