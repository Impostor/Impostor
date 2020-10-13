The solution file contains two main projects which are the Impostor Client and the Impostor Server and are built using different .net versions being .NET Framework (netfx) 4.52 for the client and .net 5 for the server.
For now, .net 5 is still not officially released (it's in release candidate) so in order to build using Visual Studio, you should have Visual Studio Preview installed.
This documentation will go over building both the [Server](#building-the-server) and the [Client](#building-the-client) and their requirements.

### Cloning the repo
Since Impostor uses git submodule to reference [Hazel](https://github.com/AeonLucid/Impostor/tree/master/submodules) you should include the ```--recurse-submodules``` option to ```git clone``` command. ie:
```bash
git clone --recurse-submodules https://github.com/AeonLucid/Impostor.git
```
if you're using https.

## Building the server
### Dependencies
* [.net 5 SDK](https://dotnet.microsoft.com/download/dotnet/5.0)
* [Visual Studio Preview](https://visualstudio.microsoft.com/vs/preview/) (optional, only if you want the full IDE experience)

### Build using the CLI
```bash
cd Impostor/src/Impostor.Server/
dotnet build
```
To setup the configs and run it, please look at [Running the server](https://github.com/AeonLucid/Impostor/docs/Running-the-server)

## Building the client
### Dependencies
* [.NET Framework 4.5.2 Developer Pack](https://dotnet.microsoft.com/download/dotnet-framework/thank-you/net452-developer-pack-offline-installer) (aka SDK)

### Build using the CLI
```bash
cd Impostor/src/Impostor.Client/
dotnet build
```