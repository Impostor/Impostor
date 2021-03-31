using System;
using System.Diagnostics.CodeAnalysis;
using Impostor.Api.Net;

namespace Impostor.Api.Games
{
    public readonly struct GameJoinResult
    {
        private GameJoinResult(GameJoinError error, string? message = null, IClientPlayer? player = null)
        {
            Error = error;
            Message = message;
            Player = player;
        }

        public GameJoinError Error { get; }

        public bool IsSuccess => Error == GameJoinError.None;

        public bool IsCustomError => Error == GameJoinError.Custom;

        [MemberNotNullWhen(true, nameof(IsCustomError))]
        public string? Message { get; }

        [MemberNotNullWhen(true, nameof(IsSuccess))]
        public IClientPlayer? Player { get; }

        public static GameJoinResult CreateCustomError(string message)
        {
            return new GameJoinResult(GameJoinError.Custom, message);
        }

        public static GameJoinResult CreateSuccess(IClientPlayer player)
        {
            return new GameJoinResult(GameJoinError.None, player: player);
        }

        public static GameJoinResult FromError(GameJoinError error)
        {
            if (error == GameJoinError.Custom)
            {
                throw new InvalidOperationException($"Custom errors should provide a message, use {nameof(CreateCustomError)} instead.");
            }

            return new GameJoinResult(error);
        }
    }
}
