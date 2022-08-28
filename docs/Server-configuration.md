# Server configuration

Some information about all the possible configurations. Click [here](https://github.com/Impostor/Impostor/blob/master/src/Impostor.Server/config-full.json) to see all the possible config options.

## Options

### Required Server Configuration

| Key            | Default     | Description                                                                                                                                                                                                                                                                                                                                                                                            |
| -------------- | ----------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **PublicIp**   | `127.0.0.1` | This needs to the public IPv4 address of the server which you give to others to connect. You can find your IPv4 address [on this website](http://whatismyip.host/). Unless you are only planning to use Impostor privately, on your local network, you should change this to your public ip. It is also possible to use hostnames instead of IPv4 addresses, which will be resolved to IPv4 addresses. |
| **PublicPort** | `22023`     | The public port of the server which you give to others to connect. (**This is the external port you configure on your router when port forwarding.**) Usually `22023`.                                                                                                                                                                                                                                 |
| **ListenIp**   | `0.0.0.0`   | The network interface to listen on. If you do not know what to put here, use `0.0.0.0`. Since 1.2.2 it is also possible to use hostnames instead of IPv4 addresses, these must resolve to a valid IPv4 address.                                                                                                                                                                                        |
| **ListenPort** | `22023`     | The listen port of the server, usually `22023`.                                                                                                                                                                                                                                                                                                                                                        |

### AntiCheat

Impostor has an Anticheat that makes it possible to kick cheaters from games automatically. Note that the anticheat is tuned on the vanilla version of the game, so client-side modifications could trigger the Anticheat if you're playing with them.

| Key               | Default | Value                                                                                                                                                                                                               |
| ----------------- | ------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Enabled**       | `true`  | Whether the anticheat should be enabled.                                                                                                                                                                            |
| **BanIpFromGame** | `true`  | When anticheat is enabled and a player is caught hacking, they will be kicked from the server. If this value is set to `true`, the player will be banned instead and will not be able to rejoin that specific game. |

### Compatibility

Impostor has two compatibility options which allow some extra flexibility but may not work properly. Enabling either of these options is not recommended.

| Key                      | Default | Value                                                                                                                                                                          |
| ------------------------ | ------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **AllowNewGameVersions** | `false` | Warning: Setting this option to `true` is unsupported and may cause issues when large updates to Among Us are released. Allow future versions of Among Us to join your server. |
| **AllowVersionMixing**   | `false` | Allow players using different game versions to play in one lobby.                                                                                                              |

### Debug

The Debug configuration is used to enable the game recorder. This is mostly useful when developing Impostor.

| Key                     | Default | Value                                        |
| ----------------------- | ------- | -------------------------------------------- |
| **GameRecorderEnabled** | `false` | Enables the Game Recorder.                   |
| **GameRecorderPath**    | _empty_ | Path where the recorded games will be saved. |

### ServerRedirector

In a multi-node setup these values need to be specified. Note that most people do not need to run a Multi-node setup.

| Key                    | Default | Value                                                                                                                                             |
| ---------------------- | ------- | ------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Enabled**            | `false` | Whether the server runs in multi-node setup. If this is `false`, all other options in this section do not have any effect.                        |
| **Master**             | `false` | Whether the current server is a master. A master is responsible for redirecting clients to nodes                                                  |
| **Locator**            |         | Fill in either `Redis` or `UdpMasterEndpoint` to choose which method to use for locating other nodes. This must be the same across all servers.   |
| **>Redis**             |         | Format `127.0.0.1.6379`, you can also use a password like so: `127.0.0.1.6379,password=value`.                                                    |
| **>UdpMasterEndpoint** |         | On the master, this value acts as a listen ip and port. On a node, this should be the public ip and port of the master. Format `127.0.0.1:32320`. |
| **Nodes**              |         | An array containing public ips and ports of nodes. Only needs to be set on the master. See above for an example.                                  |

### Serilog (Logging)

Impostor's log framework, Serilog, can be configured in the config file. You can change its default log level and you can add additional sinks.

| Key              | Default       | Value                                                                                                                                                                                                                          |
| ---------------- | ------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **MinimumLevel** | `Information` | Minimum log level for a message to be printed. If a log entry is as severe or more severe than this level, it will be printed. From most severe to least severe: `Fatal`,`Error`, `Warning`, `Information`, `Debug`, `Verbose` |
| **Using**        | `[]`          | List of additional Serilog Sinks assemblies to load.                                                                                                                                                                           |
| **WriteTo**      | `[]`          | Additional logging sinks. See the Serilog documentation or the example in this section. Serilog                                                                                                                                |

For more information, check the [Serilog.Settings.Configuration](https://github.com/serilog/serilog-settings-configuration) documentation.

For example, to add logging to a file, you should add the following snippet to your configuration:

```json
"Serilog": {
  "Using": [
    "Serilog.Sinks.File"
  ],
  "WriteTo": [
    {
      "Name": "File",
      "Args": {
        "path": "logs/log.txt",
        "rollingInterval": "Day"
      }
    }
  ]
}
```

Next to that, you also need to copy over Serilog.Sinks.File.dll from [NuGet](https://www.nuget.org/packages/Serilog.Sinks.File/). See the [Serilog.Sinks.File documentation](https://github.com/serilog/serilog-sinks-file#json-appsettingsjson-configuration) for a list of parameters that can be configured.

Other Serilog sinks are also supported, but are out of scope for this documentation.

## Config providers

### File

The simplest option to configure is by using the `config.json` file next to the server executable. For all possible options see the [config-full.json](https://github.com/Impostor/Impostor/blob/dev/src/Impostor.Server/config-full.json) file.

### Environment variables

If you're unable to edit the config.json file, you can instead set Environment variables to configure Impostor. The general pattern for each variable is `IMPOSTOR_SectionName__VariableName`. For example, to disable the anticheat, you can set the environment variable `IMPOSTOR_AntiCheat__Enabled=false`. Here are some more examples:

```
IMPOSTOR_Server__PublicIp=127.0.0.1
IMPOSTOR_Server__PublicPort=22023
IMPOSTOR_Server__ListenIp=0.0.0.0
IMPOSTOR_Server__ListenPort=22023
IMPOSTOR_AntiCheat__Enabled=true
IMPOSTOR_AntiCheat__BanIpFromGame=true
IMPOSTOR_ServerRedirector__Enabled=false
IMPOSTOR_ServerRedirector__Master=true
IMPOSTOR_ServerRedirector__Locator__Redis=127.0.0.1.6379
IMPOSTOR_ServerRedirector__Locator__UdpMasterEndpoint=127.0.0.1:32320
IMPOSTOR_ServerRedirector__Nodes__0__Ip=127.0.0.1
IMPOSTOR_ServerRedirector__Nodes__0__Port=22024
IMPOSTOR_ServerRedirector__Nodes__1__Ip=127.0.0.1
IMPOSTOR_ServerRedirector__Nodes__1__Port=22025
```
