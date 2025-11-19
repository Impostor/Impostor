# CLAUDE.md - Impostor Codebase Guide for AI Assistants

This document provides a comprehensive guide to the Impostor codebase for AI assistants working on development tasks.

## Table of Contents

- [Project Overview](#project-overview)
- [Codebase Structure](#codebase-structure)
- [Architecture & Design Patterns](#architecture--design-patterns)
- [Development Setup](#development-setup)
- [Key Technologies](#key-technologies)
- [Development Workflow](#development-workflow)
- [Code Conventions](#code-conventions)
- [Common Tasks](#common-tasks)
- [Testing](#testing)
- [Important Files Reference](#important-files-reference)
- [Plugin Development](#plugin-development)

---

## Project Overview

**Impostor** is an open-source Among Us private server written in C#. It provides:

- Full replacement for the official Among Us server
- Plugin system for extensibility
- Server-sided anti-cheat
- HTTP/REST API for game management
- Support for all Among Us maps and game features

**Current Version:** 1.10.5
**Target Framework:** .NET 8.0
**Language:** C# 11
**License:** GNU GPLv3

---

## Codebase Structure

The repository is organized as follows:

```
/
├── src/                          # Source code
│   ├── Impostor.Api/            # Core plugin API (NuGet package)
│   ├── Impostor.Server/         # Main server implementation
│   ├── Impostor.Api.Innersloth.Generator/  # Protocol code generator
│   ├── Impostor.Client/         # Client library
│   ├── Impostor.Client.App/     # Example client application
│   ├── Impostor.Plugins.Example/    # Example plugin
│   ├── Impostor.Plugins.Debugger/   # Built-in debugging plugin
│   ├── Impostor.Tools.Proxy/    # Network packet interceptor
│   ├── Impostor.Tools.ServerReplay/ # Session replay tool
│   ├── Impostor.Tests/          # Unit tests (xUnit)
│   └── Impostor.Benchmarks/     # Performance benchmarks
├── docs/                         # Documentation
├── .github/workflows/           # CI/CD workflows
├── build.cake                   # Cake build script
├── Dockerfile                   # Docker build configuration
└── CONTRIBUTING.md              # Contribution guidelines
```

### Project Purposes

| Project | Purpose | Type |
|---------|---------|------|
| **Impostor.Api** | Public API for plugin development, event system, game interfaces | NuGet Library |
| **Impostor.Server** | Main Among Us server implementation, networking, game logic | Executable |
| **Impostor.Api.Innersloth.Generator** | Roslyn source generator for Among Us protocol structures | Source Generator |
| **Impostor.Plugins.Example** | Reference implementation of a plugin | Plugin |
| **Impostor.Plugins.Debugger** | Built-in debugging tools with web UI | Plugin |
| **Impostor.Tools.Proxy** | Network traffic analysis tool | Tool |
| **Impostor.Tools.ServerReplay** | Game session replay for testing | Tool |
| **Impostor.Tests** | Unit and integration tests | Test Project |
| **Impostor.Benchmarks** | Performance benchmarks | Benchmark Project |

---

## Architecture & Design Patterns

### Overall Architecture

```
Client Connection (Among Us Game)
    ↓
Hazel Network Layer (UDP/TCP)
    ↓
ClientManager / MatchmakerService
    ↓
GameManager + Game Instances
    ↓
InnerNetObjects (GameData, PlayerControl, MeetingHud, ShipStatus)
    ↓
EventManager → Plugins
    ↓
HTTP/REST API (optional)
```

### Key Design Patterns

#### 1. Dependency Injection (DI)
- Uses `Microsoft.Extensions.DependencyInjection`
- All services registered in `Program.cs`
- Plugins can register services via `IPluginStartup`
- Constructor injection throughout codebase

#### 2. Event-Driven Architecture
- `IEventManager` handles all event publishing/subscription
- Plugins register `IEventListener` implementations
- `[EventListener]` attribute marks handler methods
- Events can be canceled via `IEventCancelable`
- Event priority support for execution order

**Example Event Flow:**
```
Player sends chat message
→ Client.HandleMessageAsync
→ InnerPlayerControl.HandleRpc
→ EventManager.CallAsync(GamePlayerChatEvent)
→ Plugin event listeners receive event
→ Response sent to all players
```

#### 3. Plugin System
- Dynamic assembly loading via `AssemblyLoadContext`
- Plugins implement `IPlugin` interface
- `IPluginStartup` for DI registration
- `IPluginHttpStartup` for HTTP middleware
- Isolated plugin contexts for unloading

#### 4. Message Processing Pipeline
- `MessageReader`/`MessageWriter` for binary serialization
- Message types: Root, RPC, GameData, Custom
- Routing based on message type and destination
- Object pooling for performance

#### 5. State Management
- `InnerNetObject` hierarchy for game state
- Serialize/Deserialize pattern for network sync
- Owner-based access control
- Spawn/Despawn lifecycle management

---

## Development Setup

### Prerequisites

- .NET 8.0 SDK or later
- C# 11 compatible IDE (Visual Studio 2022, Rider, VS Code)
- Git for version control

### Building from Source

```bash
# Clone the repository
git clone https://github.com/Impostor/Impostor.git
cd Impostor

# Restore dependencies
dotnet restore ./src/Impostor.sln

# Build the solution
dotnet build ./src/Impostor.sln

# Run tests
dotnet test ./src/Impostor.Tests/Impostor.Tests.csproj

# Build using Cake (recommended for releases)
dotnet tool restore
dotnet cake --target=Build
```

### Running the Server

```bash
# Development run
cd src/Impostor.Server
dotnet run

# Or with Cake
dotnet cake --target=Build
cd build/Impostor-Server_<version>_<platform>
./Impostor.Server  # or Impostor.Server.exe on Windows
```

### Build Targets (Cake)

```bash
dotnet cake --target=Clean      # Clean build directory
dotnet cake --target=Restore    # Restore NuGet packages
dotnet cake --target=Build      # Build and publish all platforms
dotnet cake --target=Test       # Run tests
dotnet cake --target=Replay     # Run server replay tool
```

### Supported Platforms

- `win-x64` - Windows 64-bit
- `linux-x64` - Linux 64-bit
- `linux-arm` - Linux ARM 32-bit
- `linux-arm64` - Linux ARM 64-bit
- `osx-x64` - macOS 64-bit

---

## Key Technologies

### Core Framework
- **.NET 8.0** - Runtime and base class libraries
- **C# 11** - Modern language features (records, pattern matching, etc.)
- **Microsoft.Extensions.Hosting** - Generic host builder pattern
- **Microsoft.Extensions.DependencyInjection** - IoC container
- **Microsoft.Extensions.Configuration** - Configuration management

### Networking
- **Impostor.Hazel v1.0.0** - Custom fork of Hazel networking library
  - UDP and TCP support
  - Reliable/unreliable message delivery
  - Connection pooling
  - Custom disconnect handling

### HTTP/Web
- **ASP.NET Core** - HTTP server and REST API
- **Kestrel** - High-performance web server
- **MVC Controllers** - REST endpoint implementation

### Logging
- **Serilog** - Structured logging
- **Serilog.Sinks.Console** - Console output
- **Microsoft.Extensions.Logging** - Logging abstractions

### Code Quality
- **StyleCop.Analyzers** - Code style enforcement
- **Nullable Reference Types** - Null safety
- **.editorconfig** - Cross-IDE consistency

### Testing & Benchmarking
- **xUnit** - Unit testing framework
- **BenchmarkDotNet** - Performance benchmarking

### Build & Deployment
- **Cake** - Build automation
- **Docker** - Containerization
- **GitHub Actions** - CI/CD

---

## Development Workflow

### Branch Strategy

- **`main`** - Stable releases only (do NOT commit here)
- **`dev`** - Primary development branch (target for PRs)
- **Feature branches** - Named descriptively (e.g., `feature/new-anticheat`)

### Making Changes

1. **Create an issue** describing the feature/bug
2. **Comment on the issue** to avoid duplicate work
3. **Create a feature branch** from `dev`
4. **Make your changes** following code conventions
5. **Test thoroughly** - ensure tests pass
6. **Commit with meaningful messages**
7. **Create PR targeting `dev` branch**
8. **Respond to review feedback**

### Commit Message Guidelines

- Use present tense: "Add feature" not "Added feature"
- Be descriptive but concise
- Reference issue numbers: "Fix #123: Resolve connection timeout"
- One feature/fix per commit (or squash before PR)

**Good Examples:**
```
Add role-based anti-cheat validation
Fix race condition in game cleanup
Update protocol for Among Us v2025.1.0
Refactor message handler to reduce allocations
```

**Bad Examples:**
```
Fix bug
WIP
stuff
debugging code
```

### CI/CD Pipeline

GitHub Actions automatically:
1. Runs on push/PR to `dev` or `main`
2. Restores NuGet packages (with caching)
3. Builds all projects
4. Runs unit tests
5. Publishes artifacts for all platforms
6. On tag: Creates GitHub release and publishes NuGet package

---

## Code Conventions

### StyleCop Rules

StyleCop is **enforced at build time**. Code must compile without warnings.

Key rules:
- Using directives: Preserve order (as specified in `.editorconfig`)
- Braces: Required for all blocks
- Naming: PascalCase for public members, camelCase for private fields with `_` prefix
- Documentation: Required for public APIs
- File organization: One class per file (generally)
- Newline at end of file: Required

### Nullable Reference Types

Enabled across all projects. Always handle nullability correctly:

```csharp
// Good
public void ProcessPlayer(IClientPlayer? player)
{
    if (player == null)
    {
        return;
    }

    // player is non-null here
}

// Bad - will cause warnings
public void ProcessPlayer(IClientPlayer player)
{
    // Might be null but not checked
}
```

### Async/Await Patterns

```csharp
// Good - ValueTask for hot paths
public ValueTask EnableAsync()
{
    // Implementation
}

// Good - proper async/await
public async Task<Game> CreateGameAsync(GameCode code)
{
    var game = await _gameFactory.CreateAsync(code);
    return game;
}

// Bad - blocking on async code
public void DoWork()
{
    SomeAsyncMethod().Wait(); // Don't do this
}
```

### Dependency Injection

```csharp
// Good - constructor injection
public class MyService
{
    private readonly ILogger<MyService> _logger;
    private readonly IGameManager _gameManager;

    public MyService(ILogger<MyService> logger, IGameManager gameManager)
    {
        _logger = logger;
        _gameManager = gameManager;
    }
}

// Bad - service locator pattern
public class MyService
{
    public void DoWork()
    {
        var service = ServiceProvider.GetService<IGameManager>(); // Avoid
    }
}
```

### Logging

```csharp
// Good - structured logging with Serilog
_logger.LogInformation("Player {PlayerName} joined game {GameCode}",
    player.Name, game.Code);

// Good - log levels
_logger.LogTrace("Detailed diagnostic info");
_logger.LogDebug("Debug information");
_logger.LogInformation("Important events");
_logger.LogWarning("Warning conditions");
_logger.LogError(ex, "Error occurred processing {MessageType}", messageType);

// Bad - string concatenation
_logger.LogInformation("Player " + player.Name + " joined"); // Don't do this
```

---

## Common Tasks

### Adding a New Event

1. **Create event interface** in `Impostor.Api/Events/`
```csharp
public interface IGameCustomEvent : IGameEvent
{
    string CustomData { get; }
}
```

2. **Create event implementation** in `Impostor.Server/Events/`
```csharp
public class GameCustomEvent : IGameCustomEvent
{
    public IGame Game { get; }
    public string CustomData { get; }

    public GameCustomEvent(IGame game, string customData)
    {
        Game = game;
        CustomData = customData;
    }
}
```

3. **Fire the event** where appropriate
```csharp
await _eventManager.CallAsync(new GameCustomEvent(game, data));
```

### Adding a New RPC Handler

RPCs are handled in `InnerNetObject` implementations (e.g., `InnerPlayerControl`):

```csharp
public override async ValueTask<bool> HandleRpcAsync(ClientPlayer sender,
    ClientPlayer? target, RpcCalls call, IMessageReader reader)
{
    switch (call)
    {
        case RpcCalls.CustomRpc:
        {
            // Read data
            var data = reader.ReadString();

            // Validate
            if (!sender.IsOwner(this))
            {
                return false; // Reject unauthorized RPC
            }

            // Process and broadcast
            await ProcessCustomRpc(data);
            return true;
        }

        default:
            return await base.HandleRpcAsync(sender, target, call, reader);
    }
}
```

### Adding Anti-Cheat Validation

Add checks in appropriate handlers with proper logging:

```csharp
// Example: Validate player position in InnerPlayerControl
private bool ValidatePosition(Vector2 position)
{
    if (!IsValidMapPosition(position))
    {
        _logger.LogWarning(
            "Player {PlayerName} sent invalid position {Position}",
            PlayerInfo.PlayerName,
            position);
        return false;
    }
    return true;
}
```

Configuration in `config.json`:
```json
{
  "AntiCheat": {
    "Enabled": true,
    "BanIpFromGame": true,
    "AllowCheatingHosts": "Never",
    "EnableGameFlowChecks": true,
    "EnableMustBeHostChecks": true,
    "EnableInvalidObjectChecks": true,
    "EnableColorLimitChecks": true,
    "EnableNameLimitChecks": true,
    "EnableOwnershipChecks": true,
    "EnableRoleChecks": true,
    "EnableTargetChecks": true,
    "ForbidProtocolExtensions": true
  }
}
```

### Updating for New Among Us Version

1. **Update protocol definitions** in `Impostor.Api/Innersloth/`
2. **Update JSON data files** in `Impostor.Api.Innersloth.Generator/Innersloth/Data/`
3. **Regenerate code** (automatic on build via source generator)
4. **Update `GameVersion`** constants
5. **Test all game features** thoroughly
6. **Update compatibility manager** if protocol changed

### Adding Configuration Options

1. **Add to config class** in `Impostor.Api/Config/`
```csharp
public class MyFeatureConfig
{
    public bool Enabled { get; set; } = true;
    public int MaxValue { get; set; } = 100;
}
```

2. **Register in ServerConfig** or create new section
3. **Use in code** via dependency injection
```csharp
public MyService(IOptions<MyFeatureConfig> config)
{
    _config = config.Value;
}
```

4. **Document in** `docs/Server-configuration.md`

---

## Testing

### Running Tests

```bash
# Run all tests
dotnet test ./src/Impostor.Tests/Impostor.Tests.csproj

# Run with coverage
dotnet test ./src/Impostor.Tests/Impostor.Tests.csproj --collect:"XPlat Code Coverage"

# Run specific test
dotnet test --filter "FullyQualifiedName~GameCodeTests"
```

### Writing Tests

Use xUnit framework:

```csharp
public class GameCodeTests
{
    [Fact]
    public void GameCode_Should_Parse_Correctly()
    {
        // Arrange
        const int code = 123456;

        // Act
        var gameCode = GameCode.From(code);

        // Assert
        Assert.Equal(code, gameCode.Value);
    }

    [Theory]
    [InlineData("ABCDEF")]
    [InlineData("QWERTY")]
    public void GameCode_Should_Parse_From_String(string codeString)
    {
        // Arrange & Act
        var gameCode = GameCode.From(codeString);

        // Assert
        Assert.NotNull(gameCode);
    }
}
```

### Integration Testing

Use `Impostor.Tools.ServerReplay` for session replay testing:

```bash
dotnet cake --target=Replay
```

---

## Important Files Reference

### Core Server Files

| File Path | Purpose | Key Responsibilities |
|-----------|---------|---------------------|
| `Impostor.Server/Program.cs` | Application entry point | Host setup, DI configuration, plugin loading |
| `Impostor.Server/Net/Manager/ClientManager.cs` | Client lifecycle | Connection registration, disconnection handling |
| `Impostor.Server/Net/Manager/GameManager.cs` | Game lifecycle | Game creation, lookup, cleanup |
| `Impostor.Server/Net/Inner/InnerPlayerControl.cs` | Player state & RPCs | Movement, chat, voting, killing, etc. |
| `Impostor.Server/Net/Inner/InnerGameData.cs` | Game data container | Player list, game state synchronization |
| `Impostor.Server/Net/Inner/InnerMeetingHud.cs` | Meeting management | Voting, discussion timer, results |
| `Impostor.Server/Net/State/Game.cs` | Game instance | Game loop, player management, state |
| `Impostor.Server/Events/EventManager.cs` | Event system | Event registration, firing, priority handling |
| `Impostor.Server/MatchmakerService.cs` | Server startup | Service initialization, shutdown |

### API Files

| File Path | Purpose |
|-----------|---------|
| `Impostor.Api/Events/*.cs` | Event interfaces for plugins |
| `Impostor.Api/Innersloth/*.cs` | Among Us protocol data structures |
| `Impostor.Api/Net/*.cs` | Networking abstractions and interfaces |
| `Impostor.Api/Plugins/*.cs` | Plugin system interfaces |
| `Impostor.Api/Config/*.cs` | Configuration models |

### Configuration Files

| File Path | Purpose |
|-----------|---------|
| `src/Directory.Build.props` | Shared MSBuild properties (version, framework) |
| `src/.editorconfig` | Code style settings |
| `src/stylecop.json` | StyleCop configuration |
| `src/ProjectRules.ruleset` | Code analysis ruleset |
| `build.cake` | Cake build script |

### Documentation

| File Path | Purpose |
|-----------|---------|
| `docs/Writing-a-plugin.md` | Plugin development guide |
| `docs/Server-configuration.md` | Server config reference |
| `docs/Running-the-server.md` | Server setup instructions |
| `docs/TROUBLESHOOTING.md` | Common issues and solutions |

---

## Plugin Development

### Plugin Structure

Minimum plugin implementation:

```csharp
using Impostor.Api.Plugins;
using Microsoft.Extensions.Logging;

namespace MyPlugin
{
    [ImpostorPlugin("com.example.myplugin")]
    public class MyPlugin : PluginBase
    {
        private readonly ILogger<MyPlugin> _logger;

        public MyPlugin(ILogger<MyPlugin> logger)
        {
            _logger = logger;
        }

        public override ValueTask EnableAsync()
        {
            _logger.LogInformation("My Plugin enabled!");
            return default;
        }

        public override ValueTask DisableAsync()
        {
            _logger.LogInformation("My Plugin disabled!");
            return default;
        }
    }
}
```

**Note:** The `ImpostorPlugin` attribute takes a single string parameter (the plugin ID). The multi-parameter constructor with name, author, and version is obsolete.

### Plugin with Event Listeners

```csharp
using Impostor.Api.Events;
using Impostor.Api.Events.Player;
using Microsoft.Extensions.DependencyInjection;

// Startup class for DI registration
public class MyPluginStartup : IPluginStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IEventListener, MyEventListener>();
    }

    public void ConfigureHost(IHostBuilder host)
    {
        // Optional: Configure host
    }
}

// Event listener implementation
public class MyEventListener : IEventListener
{
    private readonly ILogger<MyEventListener> _logger;

    public MyEventListener(ILogger<MyEventListener> logger)
    {
        _logger = logger;
    }

    [EventListener]
    public void OnPlayerSpawned(IPlayerSpawnedEvent e)
    {
        _logger.LogInformation(
            "Player {PlayerName} spawned in game {GameCode}",
            e.PlayerControl.PlayerInfo.PlayerName,
            e.Game.Code);
    }

    [EventListener(EventPriority.High)]
    public void OnPlayerChat(IPlayerChatEvent e)
    {
        // High priority - runs before normal priority listeners
        if (e.Message.Contains("badword"))
        {
            e.IsCancelled = true; // Cancel the event
        }
    }
}
```

### Plugin Project Setup

1. Create new .NET 8.0 Class Library
2. Add NuGet reference to `Impostor.Api`
3. Build plugin DLL
4. Copy to server's `plugins/` directory
5. Restart server

### Actual Example Plugin

The repository includes a comprehensive example plugin at `src/Impostor.Plugins.Example/` that demonstrates:

- Plugin initialization with game creation in `EnableAsync()`
- Multiple event listeners organized by category:
  - `GameEventListener.cs` - Game lifecycle events
  - `PlayerEventListener.cs` - Player actions (spawn, chat, voting, tasks, venting)
  - `MeetingEventListener.cs` - Meeting/voting events
  - `ClientEventListener.cs` - Client connection events
- Using dependency injection to access server services (IGameManager, ILogger)
- Modifying player properties (color, hat, skin, pet)
- Interacting with game settings
- Handling cosmetics using string identifiers (e.g., `"hat_pk05_Cheese"`, `"skin_Police"`)

**Key takeaways from the example:**
- Event listeners are separate classes registered in `ExamplePluginStartup`
- Multiple event handlers can be in one listener class
- Event handlers can be `void`, `ValueTask`, or `async ValueTask`
- Player cosmetics use string IDs, not numeric enums (updated from older versions)

For complete guide, see `docs/Writing-a-plugin.md`.

---

## Additional Resources

### Official Documentation
- Main README: `/README.md`
- Contributing Guide: `/CONTRIBUTING.md`
- Server Setup: `/docs/Running-the-server.md`
- Plugin Development: `/docs/Writing-a-plugin.md`
- Server Configuration: `/docs/Server-configuration.md`
- Troubleshooting: `/docs/TROUBLESHOOTING.md`

### External Links
- Discord: https://discord.gg/Mk3w6Tb
- GitHub: https://github.com/Impostor/Impostor
- Among Us Protocol: Among Us uses a custom binary protocol over UDP/TCP

### Key Concepts to Understand

1. **Game Codes** - 6-character codes (e.g., "ABCDEF") used to identify games
2. **InnerNetObjects** - Networked game objects synchronized between clients
3. **RPC Calls** - Remote Procedure Calls for player actions
4. **Hazel Protocol** - Custom UDP/TCP networking protocol
5. **Host Authority** - Whether server or host client controls game state
6. **Anti-Cheat** - Server-side validation of player actions

---

## Quick Reference Commands

```bash
# Build everything
dotnet cake --target=Build

# Run tests
dotnet cake --target=Test

# Quick server build for development
dotnet build src/Impostor.Server/Impostor.Server.csproj

# Run server directly
dotnet run --project src/Impostor.Server/Impostor.Server.csproj

# Build specific platform
dotnet publish -c Release -r linux-x64 src/Impostor.Server/Impostor.Server.csproj

# Run benchmarks
dotnet run -c Release --project src/Impostor.Benchmarks/Impostor.Benchmarks.csproj

# Clean build artifacts
dotnet cake --target=Clean
```

---

## Summary for AI Assistants

When working on Impostor:

1. **Always target `dev` branch** for pull requests, never `main`
2. **Follow StyleCop rules** - code must compile without warnings
3. **Write tests** for new features and bug fixes
4. **Use dependency injection** - don't instantiate services directly
5. **Handle nullability properly** - nullable reference types are enabled
6. **Log with structured logging** - use Serilog's format
7. **Document public APIs** - XML documentation comments required
8. **Consider backward compatibility** - especially for plugin API
9. **Test with actual Among Us client** when possible
10. **Ask questions on Discord** if uncertain about implementation approach

**Key Architecture Points:**
- Event-driven plugin system
- Dependency injection throughout
- Message-based networking via Hazel
- InnerNetObject hierarchy for game state
- Anti-cheat validation on server side

This is a well-architected, modern C# codebase following best practices. Code quality and maintainability are prioritized.
