# Building from source

## Cloning Impostor

You need to clone Impostor using git.

```bash
git clone https://github.com/Impostor/Impostor.git
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
