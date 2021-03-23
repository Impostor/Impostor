using Impostor.Api;
using Impostor.Api.Events;
using Impostor.Api.Games;
using Impostor.Api.Games.Managers;

namespace Impostor.Server.Events
{
    public class BeforeGameCreatedEvent : IBeforeGameCreatedEvent
    {
        private readonly IGameManager _gameManager;
        private GameCode? _gameCode;

        public BeforeGameCreatedEvent(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

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
    }
}
