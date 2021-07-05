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

## 1. Install .NET Core SDK

Download and install the latest .NET Core SDK.

https://dotnet.microsoft.com/download

## 2. Create a C# project

The first step is creating a new C# project, it must be a **Class Library (.NET Standard)**. The target framework can be any of those compatible with .NET 5, which includes:

- .NET Standard 2.0
- .NET Standard 2.1
- .NET 5

For more information about compatibility, see https://docs.microsoft.com/en-us/dotnet/standard/net-standard.

> At the moment of writing this document, I recommend you to use **.NET Standard 2.1**. This should give you enough functionality. If not, upgrade to .NET 5.

When the project has been created, you should have `Class.cs` and `Project.csproj` files. Your `Project.csproj` should look something like this.

```xml
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <TargetFramework>netstandard2.1</TargetFramework>
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
using Impostor.Api.Events.Managers;
using Impostor.Api.Plugins;
using Microsoft.Extensions.Logging;

namespace Impostor.Plugins.Example
{
    /// <summary>
    ///     The metadata information of your plugin, this is required.
    /// </summary>
    [ImpostorPlugin(
        package: "gg.impostor.example",
        name: "Example",
        author: "AeonLucid",
        version: "1.0.0")]
    public class ExamplePlugin : PluginBase // This is also required ": PluginBase".
    {
        /// <summary>
        ///     A logger that works seamlessly with the server.
        /// </summary>
        private readonly ILogger<ExamplePlugin> _logger;

        /// <summary>
        ///     The constructor of the plugin. There are a few parameters you can add here and they
        ///     will be injected automatically by the server, two examples are used here.
        ///
        ///     They are not necessary but very recommended.
        /// </summary>
        /// <param name="logger">
        ///     A logger to write messages in the console.
        /// </param>
        /// <param name="eventManager">
        ///     An event manager to register event listeners.
        ///     Useful if you want your plugin to interact with the game.
        /// </param>
        public ExamplePlugin(ILogger<ExamplePlugin> logger, IEventManager eventManager)
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
            _logger.LogInformation($"Game is starting.");

            // This prints out for all players if they are impostor or crewmate.
            foreach (var player in e.Game.Players)
            {
                var info = player.Character.PlayerInfo;
                var isImpostor = info.IsImpostor;
                if (isImpostor)
                {
                    _logger.LogInformation($"- {info.PlayerName} is an impostor.");
                }
                else
                {
                    _logger.LogInformation($"- {info.PlayerName} is a crewmate.");
                }
            }
        }

        [EventListener]
        public void OnGameEnded(IGameEndedEvent e)
        {
            _logger.LogInformation($"Game has ended.");
        }

        [EventListener]
        public void OnPlayerChat(IPlayerChatEvent e)
        {
            _logger.LogInformation($"{e.PlayerControl.PlayerInfo.PlayerName} said {e.Message}");
        }
    }
}
```

## 6. Registering the event listener

The last step to get your plugin working is to register the event listener, so the server knows about it. Go back to your plugin class and modify it as below.

```csharp
using System;
using System.Threading.Tasks;
using Impostor.Api.Events.Managers;
using Impostor.Api.Plugins;
using Impostor.Plugins.Example.Handlers;
using Microsoft.Extensions.Logging;

namespace Impostor.Plugins.Example
{
    [ImpostorPlugin(
        package: "gg.impostor.example",
        name: "Example",
        author: "AeonLucid",
        version: "1.0.0")]
    public class ExamplePlugin : PluginBase
    {
        private readonly ILogger<ExamplePlugin> _logger;
        // Add the line below!
        private readonly IEventManager _eventManager;
        // Add the line below!
        private IDisposable _unregister;

        public ExamplePlugin(ILogger<ExamplePlugin> logger, IEventManager eventManager)
        {
            _logger = logger;
            // Add the line below!
            _eventManager = eventManager;
        }

        public override ValueTask EnableAsync()
        {
            _logger.LogInformation("Example is being enabled.");
            // Add the line below!
            _unregister = _eventManager.RegisterListener(new GameEventListener(_logger));
            return default;
        }

        public override ValueTask DisableAsync()
        {
            _logger.LogInformation("Example is being disabled.");
            // Add the line below!
            _unregister.Dispose();
            return default;
        }
    }
}

```

## 7. Build and run your plugin

Now your plugin is ready to be tested.

1. Right click your project and press `Build`.
2. Right click your project and press `Open Folder in File Explorer`.
3. Go to `bin/Debug/netstandard2.1/`.
4. In this directory, you should find your plugin named `Project.dll`.
5. Copy the `Project.dll` to the `plugins` directory in your Impostor server directory.
6. (Re)start your Impostor server.
7. Open Among Us, create a game and send a chat message. In the console you should see your plugin being loaded and the messages from the example.

## 8. Extra

Some extra information that might be useful for those developing plugins.

### Event listeners

- You can have multiple event listener on the same event.
- An event listener can be given a priority `[EventListener(EventPriority.Normal)]` and is called in order.
- It is not recommended to block for a long time inside `EventListener` because the events are called from inside the packet handlers. Blocking too long causes the client to time out. You should create a new `Task` for operations that will take a lot of time. 

### Dependency injection

- The main plugin class is constructed by the `IServiceProvider` of the server and can inject everything the server uses. A few examples are:
  - `ILogger<T>`
  - `IEventManager`
  - `IClientManager`
  - `IOptions<ServerConfig>`
  - `IOptions<ServerRedirectorConfig>`
- You can add your own classes and `EventListener` implementation to the `IServiceProvider` by creating a new class and implementing `IPluginStartup`. Make sure to register them as a singleton `services.AddSingleton<IEventListener, GameEventListener>();`.

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
      "D:\\Projects\\Impostor\\src\\Impostor.Plugins.Example\\bin\\Debug\\netstandard2.1"
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