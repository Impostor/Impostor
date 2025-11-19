# Writing a plugin

Impostor has support for plugins. This document will help you to setup a development environment for writing a plugin.

- [1. Install .NET Core SDK](#1-install-net-core-sdk)
- [2. Create a C# project](#2-create-a-c-project)
- [3. Add the Impostor.Api library](#3-add-the-impostorapi-library)
  - [Quick](#quick)
  - [Visual Studio](#visual-studio)
- [4. The plugin class](#4-the-plugin-class)
- [5. Adding an event listener](#5-adding-an-event-listener)
- [6. Registering the event listener](#6-registering-the-event-listener)
- [7. Build and run your plugin](#7-build-and-run-your-plugin)
- [8. Extra](#8-extra)
  - [Event listeners](#event-listeners)
  - [Dependency injection](#dependency-injection)
  - [Server configuration](#server-configuration)
  - [Using other libraries](#using-other-libraries)
  - [Impostor versions](#impostor-versions)
- [9. Missing/invalid data or want more functions?](#9-missinginvalid-data-or-want-more-functions)

## 1. Install .NET SDK

Download and install the latest .NET SDK.

https://dotnet.microsoft.com/download

## 2. Create a C# project

The first step is creating a new C# project, it must be a **Class Library (.NET Standard)**. The target framework can be any of those compatible with .NET 8, but we recommend sticking with **.NET 8.0**.

For more information about compatibility, see https://docs.microsoft.com/en-us/dotnet/standard/net-standard.

When the project has been created, you should have `Class.cs` and `Project.csproj` files. Your `Project.csproj` should look something like this.

```xml
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <TargetFramework>net8.0</TargetFramework>
    </PropertyGroup>
</Project>
```

## 3. Add the Impostor.Api library

You only have to follow the instructions of one below. 

### Quick

Install the `Impostor.Api` NuGet package.  
Make sure to get a prerelease if you are writing a plugin for a dev release of the server.

### Visual Studio

1. Right click your project.
2. Click `Manage NuGet Packages`.
3. Click `Browse`.
4. Search for `Impostor.Api`.
5. Click the `Impostor.Api` result and press install on the right side.

### Dotnet CLI

1. Open your project folder in command prompt / bash.
2. Run `dotnet add package Impostor.Api`.

## 4. The plugin class

Now the `Impostor.Api` is installed, you need to create a class for your plugin. A plugin **must** contain exactly one. See the code below for an example.

```csharp
using System.Threading.Tasks;
using Impostor.Api.Plugins;
using Microsoft.Extensions.Logging;

namespace Impostor.Plugins.Example
{
    /// <summary>
    ///     The metadata information of your plugin, this is required.
    ///     Note: The multi-parameter constructor is obsolete. Use the single string ID parameter.
    /// </summary>
    [ImpostorPlugin("gg.impostor.example")]
    public class ExamplePlugin : PluginBase // This is also required ": PluginBase".
    {
        /// <summary>
        ///     A logger that works seamlessly with the server.
        /// </summary>
        private readonly ILogger<ExamplePlugin> _logger;

        /// <summary>
        ///     The constructor of the plugin. Parameters are automatically injected
        ///     by the server's dependency injection system.
        /// </summary>
        /// <param name="logger">
        ///     A logger to write messages in the console.
        /// </param>
        public ExamplePlugin(ILogger<ExamplePlugin> logger)
        {
            _logger = logger;
        }

        /// <summary>
        ///     This is called when your plugin is enabled by the server.
        /// </summary>
        /// <returns></returns>
        public override ValueTask EnableAsync()
        {
            _logger.LogInformation("Example is being enabled.");
            return default;
        }

        /// <summary>
        ///     This is called when your plugin is disabled by the server.
        ///     Most likely because it is shutting down, this is the place to clean up any managed resources.
        /// </summary>
        /// <returns></returns>
        public override ValueTask DisableAsync()
        {
            _logger.LogInformation("Example is being disabled.");
            return default;
        }
    }
}
```

## 5. Adding an event listener

Currently you should have a plugin that loads and does nothing. In order to get some actual functionality, you need to add an event listener.

Create a new class called `GameEventListener`. Example code:

```csharp
using Impostor.Api.Events;
using Impostor.Api.Events.Player;
using Microsoft.Extensions.Logging;

namespace Impostor.Plugins.Example.Handlers
{
    /// <summary>
    ///     A class that listens for two events.
    ///     It may be more but this is just an example.
    ///
    ///     Make sure your class implements <see cref="IEventListener"/>.
    /// </summary>
    public class GameEventListener : IEventListener
    {
        private readonly ILogger<ExamplePlugin> _logger;

        public GameEventListener(ILogger<ExamplePlugin> logger)
        {
            _logger = logger;
        }

        /// <summary>
        ///     An example event listener.
        /// </summary>
        /// <param name="e">
        ///     The event you want to listen for.
        /// </param>
        [EventListener]
        public void OnGameStarted(IGameStartedEvent e)
        {
            _logger.LogInformation("Game is starting.");

            // This prints out for all players if they are impostor or crewmate.
            foreach (var player in e.Game.Players)
            {
                // Note: Use null-forgiving operator (!) as Character is guaranteed to exist when game starts
                var info = player.Character!.PlayerInfo;
                var isImpostor = info.IsImpostor;
                if (isImpostor)
                {
                    _logger.LogInformation("- {PlayerName} is an impostor.", info.PlayerName);
                }
                else
                {
                    _logger.LogInformation("- {PlayerName} is a crewmate.", info.PlayerName);
                }
            }
        }

        [EventListener]
        public void OnGameEnded(IGameEndedEvent e)
        {
            _logger.LogInformation("Game has ended.");
        }

        [EventListener]
        public void OnPlayerChat(IPlayerChatEvent e)
        {
            _logger.LogInformation("{PlayerName} said {Message}", e.PlayerControl.PlayerInfo.PlayerName, e.Message);
        }
    }
}
```

## 6. Registering the event listener

The last step to get your plugin working is to register the event listener with the dependency injection system. The **modern and recommended approach** is to use `IPluginStartup`:

Create a new class called `ExamplePluginStartup.cs`:

```csharp
using Impostor.Api.Events;
using Impostor.Api.Plugins;
using Impostor.Plugins.Example.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Impostor.Plugins.Example
{
    public class ExamplePluginStartup : IPluginStartup
    {
        public void ConfigureHost(IHostBuilder host)
        {
            // Optional: Configure host settings if needed
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Register your event listeners as singletons
            services.AddSingleton<IEventListener, GameEventListener>();
        }
    }
}
```

**Alternative (Legacy) Approach:**

If you prefer manual registration (not recommended), you can register listeners in the plugin's `EnableAsync` method:

```csharp
using System;
using System.Threading.Tasks;
using Impostor.Api.Events.Managers;
using Impostor.Api.Plugins;
using Impostor.Plugins.Example.Handlers;
using Microsoft.Extensions.Logging;

namespace Impostor.Plugins.Example
{
    [ImpostorPlugin("gg.impostor.example")]
    public class ExamplePlugin : PluginBase
    {
        private readonly ILogger<ExamplePlugin> _logger;
        private readonly IEventManager _eventManager;
        private IDisposable _unregister;

        public ExamplePlugin(ILogger<ExamplePlugin> logger, IEventManager eventManager)
        {
            _logger = logger;
            _eventManager = eventManager;
        }

        public override ValueTask EnableAsync()
        {
            _logger.LogInformation("Example is being enabled.");
            _unregister = _eventManager.RegisterListener(new GameEventListener(_logger));
            return default;
        }

        public override ValueTask DisableAsync()
        {
            _logger.LogInformation("Example is being disabled.");
            _unregister?.Dispose();
            return default;
        }
    }
}
```

**Recommendation:** Use the `IPluginStartup` approach as it integrates better with dependency injection and allows automatic cleanup.

## 7. Build and run your plugin

Now your plugin is ready to be tested.

1. Right click your project and press `Build`.
2. Right click your project and press `Open Folder in File Explorer`.
3. Go to `bin/Debug/net8.0/`.
4. In this directory, you should find your plugin named `Project.dll`.
5. Copy the `Project.dll` to the `plugins` directory in your Impostor server directory.
6. (Re)start your Impostor server.
7. Open Among Us, create a game and send a chat message. In the console you should see your plugin being loaded and the messages from the example.

## 8. Extra

Some extra information that might be useful for those developing plugins.

### Event listeners

- You can have multiple event listeners on the same event.
- An event listener can be given a priority `[EventListener(EventPriority.High)]` or `[EventListener(EventPriority.Low)]`. Default is `EventPriority.Normal`. Higher priority listeners execute first.
- Event handlers can return `void`, `ValueTask`, or `async ValueTask` depending on whether you need async operations.
- It is not recommended to block for a long time inside `EventListener` because the events are called from inside the packet handlers. Blocking too long causes the client to time out. You should create a new `Task` for operations that will take a lot of time.

**Event Cancellation:**

Some events implement `IEventCancelable` and can be cancelled to prevent the action:

```csharp
[EventListener]
public void OnPlayerChat(IPlayerChatEvent e)
{
    // Cancel chat messages containing bad words
    if (e.Message.Contains("badword"))
    {
        e.IsCancelled = true;
        _logger.LogWarning("Blocked inappropriate message from {Player}", e.PlayerControl.PlayerInfo.PlayerName);
    }
}
```

**Async Event Handlers:**

```csharp
[EventListener]
public async ValueTask OnPlayerSpawned(IPlayerSpawnedEvent e)
{
    // Wait before modifying player
    await Task.Delay(TimeSpan.FromSeconds(3));

    // Modify player color
    await e.PlayerControl.SetColorAsync(ColorType.Pink);
}
```

**Available Event Types:**

- **Game Events:** `IGameCreationEvent`, `IGameCreatedEvent`, `IGameStartingEvent`, `IGameStartedEvent`, `IGameEndedEvent`, `IGameDestroyedEvent`, `IGameHostChangedEvent`, `IGameOptionsChangedEvent`
- **Player Events:** `IGamePlayerJoinedEvent`, `IGamePlayerLeftEvent`, `IPlayerSpawnedEvent`, `IPlayerDestroyedEvent`, `IPlayerChatEvent`, `IPlayerStartMeetingEvent`, `IPlayerEnterVentEvent`, `IPlayerExitVentEvent`, `IPlayerVentEvent`, `IPlayerCompletedTaskEvent`, `IPlayerVotedEvent`
- **Meeting Events:** `IMeetingStartedEvent`, `IMeetingEndedEvent`
- **Client Events:** `IClientConnectedEvent`, `IClientDisconnectedEvent`

### Player manipulation and cosmetics

You can modify player properties using the `IPlayerControl` interface. **Important:** Cosmetics now use string identifiers instead of numeric enums.

**Changing player cosmetics:**

```csharp
[EventListener]
public async ValueTask OnPlayerChat(IPlayerChatEvent e)
{
    if (e.Message == "makemepretty")
    {
        await e.PlayerControl.SetColorAsync(ColorType.Pink);
        await e.PlayerControl.SetHatAsync("hat_pk05_Cheese");
        await e.PlayerControl.SetSkinAsync("skin_Police");
        await e.PlayerControl.SetPetAsync("pet_alien1");
        await e.PlayerControl.SetNameAsync("Pretty Player");
    }
}
```

**Common cosmetic string IDs:**
- Hats: `"hat_pk05_Cheese"`, `"hat_pk01_Antenna"`, `"hat_baseball"`, etc.
- Skins: `"skin_Police"`, `"skin_Astronaut"`, `"skin_Mechanic"`, etc.
- Pets: `"pet_alien1"`, `"pet_Bedcrab"`, `"pet_Hamster"`, etc.

**Other player operations:**

```csharp
// Teleport player
await playerControl.NetworkTransform.SnapToAsync(new Vector2(x, y));

// Send chat message as player
await playerControl.SendChatAsync("Hello from plugin!");

// Complete all tasks
foreach (var task in playerControl.PlayerInfo.Tasks)
{
    await task.CompleteAsync();
}

// Murder a player (if impostor)
await playerControl.MurderPlayerAsync(targetPlayer);
```

### Game manipulation

Access and modify game settings:

```csharp
[EventListener]
public async ValueTask OnPlayerChat(IPlayerChatEvent e)
{
    if (e.Message == "speed")
    {
        // Modify game options
        e.Game.Options.NumImpostors = 2;

        if (e.Game.Options is NormalGameOptions normalOptions)
        {
            normalOptions.KillCooldown = 10f;
            normalOptions.PlayerSpeedMod = 2.5f;
            normalOptions.CrewLightMod = 1.5f;
        }

        // Sync changes to all clients
        await e.Game.SyncSettingsAsync();
    }
}
```

### Dependency injection

- The main plugin class is constructed by the `IServiceProvider` of the server and can inject everything the server uses. A few examples are:
  - `ILogger<T>` - Structured logging
  - `IEventManager` - Event system
  - `IClientManager` - Access connected clients
  - `IGameManager` - Create and manage games
  - `IOptions<ServerConfig>` - Server configuration
  - `IOptions<AntiCheatConfig>` - Anti-cheat settings
- You can add your own classes and `EventListener` implementation to the `IServiceProvider` by creating a new class and implementing `IPluginStartup`. Make sure to register them as a singleton `services.AddSingleton<IEventListener, GameEventListener>();`.

**Creating games programmatically:**

```csharp
using Impostor.Api.Games.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.GameOptions;

public class ExamplePlugin : PluginBase
{
    private readonly IGameManager _gameManager;

    public ExamplePlugin(ILogger<ExamplePlugin> logger, IGameManager gameManager)
    {
        _logger = logger;
        _gameManager = gameManager;
    }

    public override async ValueTask EnableAsync()
    {
        // Create a game with default options
        var game = await _gameManager.CreateAsync(
            new NormalGameOptions(),
            GameFilterOptions.CreateDefault()
        );

        if (game != null)
        {
            game.DisplayName = "My Plugin Game";
            await game.SetPrivacyAsync(false); // true = private, false = public
            _logger.LogInformation("Created game {GameCode}", game.Code.Code);
        }

        return default;
    }
}
```

### Server configuration

Constantly copying the plugin dll to your server directory can be pretty annoying. Luckily we have a solution for that. In your Impostor server open the `config.json` and add the `PluginLoader` from the example below, replace the path with the build destination of your plugin.

```json
{
  "Server": {
    "PublicIp": "127.0.0.1",
    "PublicPort": 22023,
    "ListenIp": "0.0.0.0",
    "ListenPort": 22023
  },
  "PluginLoader": {
    "Paths": [
      "D:\\Projects\\Impostor\\src\\Impostor.Plugins.Example\\bin\\Debug\\net8.0"
    ],
    "LibraryPaths": []
  }
}
```

### Using other libraries

Sometimes you need to use libraries that the original Impostor server does not provide. The dll files of these libraries must be placed in the `libraries` folder next to the server executable. You could also provide them by modifying the `PluginLoader.LibraryPaths` option in the `config.json`, similarly to the `PluginLoader.Paths` option.

### Impostor versions

It is important to use the correct versions when working with `Impostor.Api` prereleases and the `Impostor` dev builds to reduce the chances of mismatching assemblies. 

**Example** 

The prerelease `Impostor.Api` package `1.2.0-ci.54` belongs to build `54` on AppVeyor, which can be found here https://ci.appveyor.com/project/Impostor/Impostor/build/54. Notice the `54` on the end of the url.

## 9. Missing/invalid data or want more functions?

The `Impostor.Api` is currently in beta. There are a lot of things still missing and we would like to hear from you what you need to develop a plugin.

Create an issue:

- [Suggest a function](https://github.com/Impostor/Impostor/issues/new?template=3--api-suggestion.md)
- [Data is invalid](https://github.com/Impostor/Impostor/issues/new?template=4--api-invalid.md)
- [Data is unavailable](https://github.com/Impostor/Impostor/issues/new?template=5--api-missing.md)
- [Other](https://github.com/Impostor/Impostor/issues/new?template=6--api-other.md)
