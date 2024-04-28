# Building from source

The solution contains the Impostor server and its dependencies, like Hazel and the plugin API. The server is built using [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0).

## Cloning Impostor

You need to clone Impostor using git:

```bash
git clone https://github.com/Impostor/Impostor.git
```

## Building the server

### Dependencies

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Rider](https://www.jetbrains.com/rider/) or [Visual Studio](https://visualstudio.microsoft.com/vs/) (Optional, only if you want the full IDE experience)

### Build using the CLI

```bash
cd src/Impostor.Server/
dotnet build
# Or if you want a single file, ready for production: (change linux-x64 to win-x64 if you use Windows)
dotnet publish -c Release -r linux-x64 -p:PublishSingleFile=true
```

To setup the server, please look at [Running the server](Running-the-server.md).
