using Impostor.Api;
using Impostor.Api.Events;
using Impostor.Api.Games;
using Impostor.Api.Games.Managers;
using Impostor.Api.Net;

namespace Impostor.Server.Events
{
    public class GameCreationEvent : IGameCreationEvent
    {
        private readonly IGameManager _gameManager;
        private GameCode? _gameCode;

        public GameCreationEvent(IGameManager gameManager, IClient? client)
        {
            _gameManager = gameManager;
            Client = client;
        }

        public IClient? Client { get; }

        public GameCode? GameCode
        {
            get => _gameCode;
            set
            {
                if (value.HasValue)
                {
                    if (value.Value.IsInvalid)
                    {
                        throw new ImpostorException("GameCode is invalid.");
                    }

                    if (_gameManager.Find(value.Value) != null)
                    {
                        throw new ImpostorException($"GameCode [{value.Value.Code}] is already used.");
                    }
                }

                _gameCode = value;
            }
        }

        public bool IsCancelled { get; set; }
    }
}
