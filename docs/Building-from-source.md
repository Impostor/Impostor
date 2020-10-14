# Building from source

The solution contains two main projects, the Impostor client and server. The client is built using [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472) and the server with [.NET 5](https://dotnet.microsoft.com/download/dotnet/5.0).

Currently .NET 5 is not yet officially released, so in order to build using Visual Studio, you should have Visual Studio 2019 **Preview** installed.
This documentation will go over building both the [Server](#building-the-server) and the [Client](#building-the-client) and their requirements.

## Cloning Impostor

You need to clone Impostor with all submodules.

```bash
git clone --recursive https://github.com/AeonLucid/Impostor.git
```

If you already have cloned Impostor but have errors related to Hazel, run the following.

```bash
git submodule update --init
```

## Building the server

### Dependencies
- [.NET 5 SDK](https://dotnet.microsoft.com/download/dotnet/5.0)
- [Visual Studio Preview](https://visualstudio.microsoft.com/vs/preview/) (Optional, only if you want the full IDE experience)

### Build using the CLI

```bash
cd src/Impostor.Server/
dotnet build
```
To setup the server, please look at [Running the server](Running-the-server).

## Building the client

### Dependencies
* [.NET Framework 4.7.2 Developer Pack](https://dotnet.microsoft.com/download/dotnet-framework/thank-you/net472-developer-pack-offline-installer)

### Build using the CLI
```bash
cd src/Impostor.Client/Impostor.Client.WinForms
dotnet build
```