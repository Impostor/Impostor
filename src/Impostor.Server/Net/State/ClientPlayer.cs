using System;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner;
using Impostor.Api.Unity;
using Impostor.Server.Net.Inner.Objects;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.State
{
    internal partial class ClientPlayer : IClientPlayer
    {
        private readonly ILogger<ClientPlayer> _logger;
        private readonly Timer _spawnTimeout;

        public ClientPlayer(ILogger<ClientPlayer> logger, ClientBase client, Game game)
        {
            _logger = logger;
            _spawnTimeout = new Timer(RunSpawnTimeout!, null, -1, -1);

            Game = game;
            Client = client;
            Limbo = LimboStates.PreSpawn;
        }

        public ClientBase Client { get; }

        public Game Game { get; }

        /// <inheritdoc />
        public LimboStates Limbo { get; set; }

        public InnerPlayerControl? Character { get; internal set; }

        public bool IsHost => Game?.Host == this;

        public string? Scene { get; internal set; }

        public RuntimePlatform? Platform { get; internal set; }

        public void InitializeSpawnTimeout()
        {
            _spawnTimeout.Change(Constants.SpawnTimeout, -1);
        }

        public void DisableSpawnTimeout()
        {
            _spawnTimeout.Change(-1, -1);
        }

        /// <inheritdoc />
        public bool IsOwner(IInnerNetObject netObject)
        {
            return Client.Id == netObject.OwnerId;
        }

        /// <inheritdoc />
        public ValueTask KickAsync()
        {
            return Game.HandleKickPlayer(Client.Id, false);
        }

        /// <inheritdoc />
        public ValueTask BanAsync()
        {
            return Game.HandleKickPlayer(Client.Id, true);
        }

        private async void RunSpawnTimeout(object state)
        {
            try
            {
                if (Character == null)
                {
                    _logger.LogInformation("{0} - Player {1} spawn timed out, kicking.", Game.Code, Client.Id);

                    await KickAsync();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception caught while kicking player for spawn timeout.");
            }
            finally
            {
                await _spawnTimeout.DisposeAsync();
            }
        }
    }
}
