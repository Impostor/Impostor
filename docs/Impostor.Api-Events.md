# Impostor.Api Events Documentation
Work in progress

## Table of Contents

- [Player Events](#player-events)
- [Game Events](#game-events)
- [Meeting Events](#meeting-events)
- [General Event Interfaces](#general-event-interfaces)

---

## Player Events

### IPlayerChatEvent

Triggered when a player sends a chat message.

- **Properties**:
  - `Message`: The content of the chat message.

### IPlayerCompletedTaskEvent

Triggered when a player completes a task.

- **Properties**:
  - `Task`: Information about the task that was completed.

### IPlayerDestroyedEvent

Triggered when a player is destroyed.

### IPlayerEnterVentEvent

Triggered when a player enters a vent.

- **Properties**:
  - `Vent`: The vent that the player entered.

### IPlayerEvent

Base interface for player events.

- **Properties**:
  - `ClientPlayer`: The client player instance.
  - `PlayerControl`: Control instance for the player.

### IPlayerExileEvent

Triggered when a player is exiled.

### IPlayerExitVentEvent

Triggered when a player exits a vent.

- **Properties**:
  - `Vent`: The vent that the player exited.

### IPlayerMovementEvent

Triggered when a player moves.

### IPlayerMurderEvent

Triggered when a player murders another player.

- **Properties**:
  - `Victim`: The player that was murdered.

### IPlayerSetStartCounterEvent

Triggered when a player sets the start counter.

- **Properties**:
  - `SecondsLeft`: The number of seconds left on the start counter.

### IPlayerSpawnedEvent

Triggered when a player spawns.

### IPlayerStartMeetingEvent

Triggered when a player starts a meeting.

- **Properties**:
  - `Body`: The body that was reported, if any.

### IPlayerVentEvent

Triggered when a player uses a vent.

- **Properties**:
  - `NewVent`: The vent that the player used.

### IPlayerVotedEvent

Triggered when a player casts a vote.

- **Properties**:
  - `VotedFor`: The player that was voted for.
  - `VoteType`: The type of vote (e.g., skipped, dead, player).

---

## Game Events

### IGameAlterEvent

Triggered when a game's visibility (public/private) is altered.

### IGameCreatedEvent

Triggered when a new game is created.

### IGameCreationEvent

Triggered during the creation of a game.

- **Properties**:
  - `Client`: The client instance.
  - `GameCode`: The game code.

### IGameDestroyedEvent

Triggered when a game is destroyed.

### IGameEndedEvent

Triggered when a game ends.

- **Properties**:
  - `Reason`: The reason for the game's end.

### IGameEvent

Base interface for game events.

- **Properties**:
  - `Game`: The game instance.

### IGameHostChangedEvent

Triggered when the host of a game changes.

- **Properties**:
  - `PreviousHost`: The previous host.
  - `NewHost`: The new host.

### IGamePlayerJoinedEvent

Triggered when a player joins a game.

### IGamePlayerJoiningEvent

Triggered when a player is in the process of joining a game.

- **Properties**:
  - `Player`: The player instance.
  - `JoinResult`: The result of the join attempt.

### IGamePlayerLeftEvent

Triggered when a player leaves a game.

- **Properties**:
  - `IsBanned`: Indicates if the player was banned.

### IGameStartedEvent

Triggered when a game starts.

### IGameStartingEvent

Triggered when a game is about to start.

---

## Meeting Events

### IMeetingEndedEvent

Triggered when a meeting ends.

- **Properties**:
  - `Exiled`: The player that was exiled.
  - `IsTie`: Indicates if the vote was a tie.

### IMeetingEvent

Base interface for meeting events.

- **Properties**:
  - `MeetingHud`: The meeting HUD instance.

### IMeetingStartedEvent

Triggered when a meeting starts.

---

## General Event Interfaces

### IEvent

Base interface for all events.

### IEventCancelable

Interface for events that can be canceled.

- **Properties**:
  - `IsCancelled`: Indicates whether the event is canceled.

### IEventListener

Base interface for event listeners.

### IManualEventListener

Interface for manual event listeners.

- **Properties**:
  - `Priority`: The priority of the event listener.

- **Methods**:
  - `CanExecute<T>()`: Determines if the event can be executed.
  - `Execute(IEvent @event)`: Executes the event.

### IEventManager

Interface for the event manager.

- **Methods**:
  - `RegisterListener<TListener>(TListener listener)`: Registers an event listener.
  - `IsRegistered<TEvent>()`: Checks if an event is registered.
  - `CallAsync<TEvent>(TEvent @event)`: Calls an asynchronous event.

### MultiDisposable

A utility class that allows multiple `IDisposable` objects to be disposed of together.
