/// <summary>
/// Impostor.Api Events Documentation
/// </summary>
namespace Impostor.Api
{
    /// <summary>
    /// Contains all player-related events.
    /// </summary>
    public interface IPlayerEvents
    {
        /// <summary>
        /// Triggered when a player sends a chat message.
        /// </summary>
        /// <remarks>
        /// Properties: Message - The content of the chat message.
        /// </remarks>
        event Action<IPlayerChatEvent> PlayerChat;

        /// <summary>
        /// Triggered when a player completes a task.
        /// </summary>
        /// <remarks>
        /// Properties: Task - Information about the task that was completed.
        /// </remarks>
        event Action<IPlayerCompletedTaskEvent> PlayerCompletedTask;

        /// <summary>
        /// Triggered when a player is destroyed.
        /// </summary>
        event Action<IPlayerDestroyedEvent> PlayerDestroyed;

        /// <summary>
        /// Triggered when a player enters a vent.
        /// </summary>
        /// <remarks>
        /// Properties: Vent - The vent that the player entered.
        /// </remarks>
        event Action<IPlayerEnterVentEvent> PlayerEnterVent;

        /// <summary>
        /// Triggered when a player is exiled.
        /// </summary>
        event Action<IPlayerExileEvent> PlayerExile;

        /// <summary>
        /// Triggered when a player exits a vent.
        /// </summary>
        /// <remarks>
        /// Properties: Vent - The vent that the player exited.
        /// </remarks>
        event Action<IPlayerExitVentEvent> PlayerExitVent;

        /// <summary>
        /// Triggered when a player moves.
        /// </summary>
        event Action<IPlayerMovementEvent> PlayerMovement;

        /// <summary>
        /// Triggered when a player murders another player.
        /// </summary>
        /// <remarks>
        /// Properties: Victim - The player that was murdered.
        /// </remarks>
        event Action<IPlayerMurderEvent> PlayerMurder;

        /// <summary>
        /// Triggered when a player sets the start counter.
        /// </summary>
        /// <remarks>
        /// Properties: SecondsLeft - The number of seconds left on the start counter.
        /// </remarks>
        event Action<IPlayerSetStartCounterEvent> PlayerSetStartCounter;

        /// <summary>
        /// Triggered when a player spawns.
        /// </summary>
        event Action<IPlayerSpawnedEvent> PlayerSpawned;

        /// <summary>
        /// Triggered when a player starts a meeting.
        /// </summary>
        /// <remarks>
        /// Properties: Body - The body that was reported, if any.
        /// </remarks>
        event Action<IPlayerStartMeetingEvent> PlayerStartMeeting;

        /// <summary>
        /// Triggered when a player uses a vent.
        /// </summary>
        /// <remarks>
        /// Properties: NewVent - The vent that the player used.
        /// </remarks>
        event Action<IPlayerVentEvent> PlayerVent;

        /// <summary>
        /// Triggered when a player casts a vote.
        /// </summary>
        /// <remarks>
        /// Properties: VotedFor - The player that was voted for, VoteType - The type of vote (e.g., skipped, dead, player).
        /// </remarks>
        event Action<IPlayerVotedEvent> PlayerVoted;
    }
/// <summary>
    /// Contains all game-related events.
    /// </summary>
    public interface IGameEvents
    {
        /// <summary>
        /// Triggered when a game's visibility (public/private) is altered.
        /// </summary>
        event Action<IGameAlterEvent> GameAlter;

        /// <summary>
        /// Triggered when a new game is created.
        /// </summary>
        event Action<IGameCreatedEvent> GameCreated;

        /// <summary>
        /// Triggered during the creation of a game.
        /// </summary>
        /// <remarks>
        /// Properties: Client - The client instance, GameCode - The game code.
        /// </remarks>
        event Action<IGameCreationEvent> GameCreation;

        /// <summary>
        /// Triggered when a game is destroyed.
        /// </summary>
        event Action<IGameDestroyedEvent> GameDestroyed;

        /// <summary>
        /// Triggered when a game ends.
        /// </summary>
        /// <remarks>
        /// Properties: Reason - The reason for the game's end.
        /// </remarks>
        event Action<IGameEndedEvent> GameEnded;

        /// <summary>
        /// Triggered when the host of a game changes.
        /// </summary>
        /// <remarks>
        /// Properties: PreviousHost - The previous host, NewHost - The new host.
        /// </remarks>
        event Action<IGameHostChangedEvent> GameHostChanged;

        /// <summary>
        /// Triggered when a player joins a game.
        /// </summary>
        event Action<IGamePlayerJoinedEvent> GamePlayerJoined;

        /// <summary>
        /// Triggered when a player is in the process of joining a game.
        /// </summary>
        /// <remarks>
        /// Properties: Player - The player instance, JoinResult - The result of the join attempt.
        /// </remarks>
        event Action<IGamePlayerJoiningEvent> GamePlayerJoining;

        /// <summary>
        /// Triggered when a player leaves a game.
        /// </summary>
        /// <remarks>
        /// Properties: IsBanned - Indicates if the player was banned.
        /// </remarks>
        event Action<IGamePlayerLeftEvent> GamePlayerLeft;

        /// <summary>
        /// Triggered when a game starts.
        /// </summary>
        event Action<IGameStartedEvent> GameStarted;

        /// <summary>
        /// Triggered when a game is about to start.
        /// </summary>
        event Action<IGameStartingEvent> GameStarting;
    }

    /// <summary>
    /// Contains all meeting-related events.
    /// </summary>
    public interface IMeetingEvents
    {
        /// <summary>
        /// Triggered when a meeting ends.
        /// </summary>
        /// <remarks>
        /// Properties: Exiled - The player that was exiled, IsTie - Indicates if the vote was a tie.
        /// </remarks>
        event Action<IMeetingEndedEvent> MeetingEnded;

        /// <summary>
        /// Triggered when a meeting starts.
        /// </summary>
        event Action<IMeetingStartedEvent> MeetingStarted;
    }

    /// <summary>
    /// General interfaces for events.
    /// </summary>
    public interface IEvent
    {
        // Base interface for all events.
    }

    /// <summary>
    /// Interface for events that can be canceled.
    /// </summary>
    /// <remarks>
    /// Properties: IsCancelled - Indicates whether the event is canceled.
    /// </remarks>
    public interface IEventCancelable : IEvent
    {
        // Definitions
    }

    /// <summary>
    /// Base interface for event listeners.
    /// </summary>
    public interface IEventListener
    {
        // Definitions
    }

    /// <summary>
    /// Interface for manual event listeners.
    /// </summary>
    /// <remarks>
    /// Properties: Priority - The priority of the event listener.
    /// Methods: CanExecute&lt;T&gt;() - Determines if the event can be executed, Execute(IEvent @event) - Executes the event.
    /// </remarks>
    public interface IManualEventListener : IEventListener
    {
        // Definitions
    }

    /// <summary>
    /// Interface for the event manager.
    /// </summary>
    /// <remarks>
    /// Methods: RegisterListener&lt;TListener&gt;(TListener listener) - Registers an event listener, IsRegistered&lt;TEvent&gt;() - Checks if an event is registered, CallAsync&lt;TEvent&gt;(TEvent @event) - Calls an asynchronous event.
    /// </remarks>
    public interface IEventManager
    {
        // Definitions
    }
}
