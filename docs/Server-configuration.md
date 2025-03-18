# Server configuration

## Options

### Required Server Configuration

| Key            | Default     | Description                                                                                                                                                                                                                                                                                                                                                                                            |
| -------------- | ----------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **PublicIp**   | `127.0.0.1` | This needs to the public IPv4 address of the server which you give to others to connect. You can find your IPv4 address [on this website](http://whatismyip.host/). Unless you are only planning to use Impostor privately, on your local network, you should change this to your public ip. It is also possible to use hostnames instead of IPv4 addresses, which will be resolved to IPv4 addresses. |
| **PublicPort** | `22023`     | The public port of the server which you give to others to connect. (**This is the external port you configure on your router when port forwarding.**) Usually `22023`.                                                                                                                                                                                                                                 |
| **ListenIp**   | `0.0.0.0`   | The network interface to listen on. If you do not know what to put here, use `0.0.0.0`. Since 1.2.2 it is also possible to use hostnames instead of IPv4 addresses, these must resolve to a valid IPv4 address.                                                                                                                                                                                        |
| **ListenPort** | `22023`     | The listen port of the server, usually `22023`. For port forwarding purposes: this is an UDP port                                                                                                                                                                                                                                                                                                                                                       |

### HttpServer

Impostor has an Http Server that is used by recent versions of Among Us to connect to. See [the Http Server page](Http-server.md) for more details on how to set this up.

| Key            | Default   | Description                                                                                                                                                                |
|----------------|-----------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Enabled**    | `true`    | Whether the http server should be enabled.                                                                                                                                 |
| **ListenIp**   | `0.0.0.0` | The network interface to listen on. Use `127.0.0.1` if you use a reverse proxy or just run locally. Use `0.0.0.0` if you are directly exposing this server to the internet |
| **ListenPort** | `22023`   | The listen port of this server. For port forwarding purposes, this is an TCP port.                                                                                         |

### AntiCheat

Impostor has an Anticheat that makes it possible to kick cheaters from games automatically. Note that the anticheat is tuned on the vanilla version of the game, so client-side modifications could trigger the Anticheat if you're playing with them.

| Key                           | Default   | Value                                                                                                                                                                                                               |
|-------------------------------|-----------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Enabled**                   | `true`    | Whether the anticheat should be enabled.                                                                                                                                                                            |
| **BanIpFromGame**             | `true`    | When anticheat is enabled and a player is caught hacking, they will be kicked from the server. If this value is set to `true`, the player will be banned instead and will not be able to rejoin that specific game. |
| **AllowCheatingHosts**        | `"Never"` | Configure whether hosts are allowed to cheat. "Never" forbids it, "Always" allows it. "IfRequested" allows hosts to cheat if they connect with the DisableServerAuthorityFlag set.                                  |
| **EnableGameFlowChecks**      | `true`    | Enable checks that check if certain actions are done in the appropriate order or at the appropriate moment in the game. This includes changing cosmetics while in game or murdering too fast.                       |
| **EnableInvalidObjectChecks** | `true`    | Enables checks that check if network objects are spawned properly. Disabling this option also implies disabling EnableRoleChecks.                                                                                   |
| **EnableMustBeHostChecks**    | `true`    | Enables checks that check if players are the host before they can do actions that require them to be host of the game. This includes starting the game and spawning objects.                                        |
| **EnableColorLimitChecks**    | `true`    | Enables checks that checks if players request colors that are already in use.                                                                                                                                       |
| **EnableNameLimitChecks**     | `true`    | Enables checks that checks if player names have a length that is possible to set using the user interface.                                                                                                          |
| **EnableOwnershipChecks**     | `true`    | Enables checks that check if players are allowed to perform a certain action on themself or another player.                                                                                                         |
| **EnableRoleChecks**          | `true`    | Enables checks that check if players have the correct role when performing certain role abilities like venting or murdering.                                                                                        |
| **EnableTargetChecks**        | `true`    | Enables checks that check if certain packets to everyone that should only have been sent to certain players or vice versa. This includes sending votes or network objects.                                          |
| **ForbidProtocolExtensions**  | `true`    | If disabled allows players to send network packets that go beyond the network packets sent by the vanilla game. This is necessary for most mods that need all players to install it.                                |

### Compatibility

Impostor has some compatibility options which allow some extra flexibility but may not work properly. Enabling any of these options is not recommended. When contacting support, please mention which of these options are enabled.

| Key                         | Default | Value                                                                                                                                                                                                                                                            |
|-----------------------------|---------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **AllowFutureGameVersions** | `false` | Warning: Setting this option to `true` is unsupported and may cause issues when large updates to Among Us are released, but can be useful when a small patch is released. Allows future versions of Among Us to join your server.                                |
| **AllowHostAuthority**      | `false` | Certain Among Us mods allow disabling some server-authoritative features, which also changes some code paths in the client. These code paths have not undergone as much testing and contain bugs, which can't be fixed from the Impostor side. Use with caution. |
| **AllowVersionMixing**      | `false` | Allows players using different game versions to play in one lobby that have not been marked by the Impostor developers as compatible.                                                                                                                            |

### Debug

The Debug configuration is used to enable the game recorder. This is mostly useful when developing Impostor.

| Key                     | Default | Value                                        |
| ----------------------- | ------- | -------------------------------------------- |
| **GameRecorderEnabled** | `false` | Enables the Game Recorder.                   |
| **GameRecorderPath**    | _empty_ | Path where the recorded games will be saved. |

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

The `config.json` file contains all the configurable options.

### Environment variables

If you're unable to edit the `config.json` file, you can instead set Environment variables to configure Impostor. The general pattern for each variable is `IMPOSTOR_SectionName__VariableName`. For example, to disable the anticheat, you can set the environment variable `IMPOSTOR_AntiCheat__Enabled=false`. Here are some more examples:

```
IMPOSTOR_Server__PublicIp=127.0.0.1
IMPOSTOR_Server__PublicPort=22023
IMPOSTOR_Server__ListenIp=0.0.0.0
IMPOSTOR_Server__ListenPort=22023
IMPOSTOR_AntiCheat__Enabled=true
IMPOSTOR_AntiCheat__BanIpFromGame=true
```

# systemd

Linux users will likely want to create a systemd service definition to manage running the Impostor application.  Benefits to using a systemd service include the following:

- The server process can automatically restart if the process dies
- Impostor can be set to start on boot (handy if you do system updates regularly)
- You can use systemd targets to ensure the process doesn't start until the system dependencies are met

Using your favorite text editor, create the following service file:

```
sudoedit /etc/systemd/system/impostor.service
```

Modify the following template to your needs and populate the service file.

```
[Unit]
Description=Impostor private Among Us server - https://github.com/Impostor/Impostor

[Service]
# Directory where Impostor is installed
WorkingDirectory=/opt/impostor
# Path to Impostor binary
ExecStart=/opt/impostor/Impostor.Server
Restart=always
# Restart service after 10 seconds if the dotnet service crashes
RestartSec=10
KillSignal=SIGTERM
SyslogIdentifier=impostor

# User and group that will run the Impostor server process -- ensure that user is allowed to execute the binary
User=impostor
Group=impostor
TimeoutStopSec=10

[Install]
# Wait until most system services have started before starting Impostor
After=multi-user.target
WantedBy=multi-user.target
```

Reload all systemd unit files to source in the new file you've created.

```
sudo systemctl daemon-reload
```

Start the service. 

```
sudo systemctl start impostor.service
```

Check the log for the service and verify it has started correctly -- Hint: Use the Q key to quit out of journalctl.

```
sudo journalctl -u impostor.service
```

If everything has started correctly, ensure the service is set to start on boot.

```
sudo systemctl enable impostor.service
```
