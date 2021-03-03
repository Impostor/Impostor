# Building from source

The solution contains two main projects, the Impostor server and patcher. The server is built using [.NET 5](https://dotnet.microsoft.com/download/dotnet/5.0) and the winforms patcher is with [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472).

## Cloning Impostor

You need to clone Impostor using git.

```bash
git clone https://github.com/AeonLucid/Impostor.git
```

## Building the server

### Dependencies
- [.NET 5 SDK](https://dotnet.microsoft.com/download/dotnet/5.0)
- [Rider](https://www.jetbrains.com/rider/) or [Visual Studio](https://visualstudio.microsoft.com/vs/) (Optional, only if you want the full IDE experience)

### Build using the CLI

```bash
cd src/Impostor.Server/
dotnet build
```
To setup the server, please look at [Running the server](Running-the-server.md).

## Building the winforms patcher

### Dependencies
* [.NET Framework 4.7.2 Developer Pack](https://dotnet.microsoft.com/download/dotnet-framework/thank-you/net472-developer-pack-offline-installer) or [Mono](https://www.mono-project.com/download/)

### Build using the CLI
```bash
cd src/Impostor.Patcher/Impostor.Patcher.WinForms
dotnet build
```
