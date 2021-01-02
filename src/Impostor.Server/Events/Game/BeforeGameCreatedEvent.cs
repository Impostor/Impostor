using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Impostor.Api.Events;
using Impostor.Api.Innersloth;
using Impostor.Server.Net.Redirector;
using Impostor.Server.Net.State;

namespace Impostor.Server.Events
{
    public class BeforeGameCreatedEvent : IBeforeGameCreatedEvent
    {
        private readonly ICollection<int> _gameKeys;

        public BeforeGameCreatedEvent(ICollection<int> gameKeys)
        {
            _gameKeys = gameKeys;
        }

        public string? GameCode { get; private set; }

        public bool TryToSetCode(string gameCode)
        {
            if (gameCode == null)
            {
                throw new ArgumentException("GameCode cannot be null.");
            }

            if (gameCode.Length != 6)
            {
                throw new ArgumentException("GameCode must be 6 caracters longs.");
            }

            if (!Regex.IsMatch(gameCode.ToUpper(), @"^[A-Z]+$"))
            {
                throw new ArgumentException("GameCode must contains only letters.");
            }

            if (_gameKeys.Contains(GameCodeParser.GameNameToInt(gameCode)))
            {
                return false;
            }

            GameCode = gameCode;
            return true;
        }
    }
}
