# Server configuration

Some information about all the possible configurations. Click [here](https://github.com/Impostor/Impostor/blob/master/src/Impostor.Server/config-full.json) to see all the possible config options.

## Options

### Required Server Configuration

| Key            | Default     | Description                                                                                                                                                                                                                                                                                                                                                                                            |
| -------------- | ----------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| __PublicIp__   | `127.0.0.1` | This needs to the public IPv4 address of the server which you give to others to connect. You can find your IPv4 address [on this website](http://whatismyip.host/). Unless you are only planning to use Impostor privately, on your local network, you should change this to your public ip. It is also possible to use hostnames instead of IPv4 addresses, which will be resolved to IPv4 addresses. |
| __PublicPort__ | `22023`     | The public port of the server which you give to others to connect. (__This is the external port you configure on your router when port forwarding.__) Usually `22023`.                                                                                                                                                                                                                                 |
| __ListenIp__   | `0.0.0.0`   | The network interface to listen on. If you do not know what to put here, use `0.0.0.0`. Since 1.2.2 it is also possible to use hostnames instead of IPv4 addresses, these must resolve to a valid IPv4 address.                                                                                                                                                                                        |
| __ListenPort__ | `22023`     | The listen port of the server, usually `22023`.                                                                                                                                                                                                                                                                                                                                                        |

### AntiCheat

Impostor has an Anticheat that makes it possible to kick cheaters from games automatically. Note that the anticheat is tuned on the vanilla version of the game, so client-side modifications could trigger the Anticheat if you're playing with them.

| Key               | Default | Value                                                                                                                                                                                                               |
| ----------------- | ------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| __Enabled__       | `true`  | Whether the anticheat should be enabled.                                                                                                                                                                            |
| __BanIpFromGame__ | `true`  | When anticheat is enabled and a player is caught hacking, they will be kicked from the server. If this value is set to `true`, the player will be banned instead and will not be able to rejoin that specific game. |

### AnnouncementsServer

The Announcement Server is used to show update announcements to clients. Using the Plugin API you can create a plugin that serves custom announcements.

| Key            | Default   | Value                                               |
| -------------- | --------- | --------------------------------------------------- |
| __Enabled__    | `false`   | Whether the announcements server should be enabled. |
| __ListenIp__   | `0.0.0.0` | See Required Server Configuration.                  |
| __ListenPort__ | `22025`   | See Required Server Configuration.                  |

### AuthServer

The Authentication Server is used since Among Us version 2021.3.31 and is used by clients to identify themselves. If you're playing with modded Among Us clients, you can enable this server to skip the 5 second wait to join.

| Key             | Default                | Value                                                |
| --------------- | ---------------------- | ---------------------------------------------------- |
| __Enabled__     | `false`                | Whether the authentication server should be enabled. |
| __ListenIp__    | `0.0.0.0`              | See Required Server Configuration.                   |
| __ListenPort__  | `22025`                | See Required Server Configuration.                   |
| __Certificate__ | `dtls/certificate.pem` | The public certificate used to authenticate with.    |
| __PrivateKey__  | `dtls/key.pem`         | The corresponding private key.                       |

On Linux you can generate a certificate by running `openssl req -x509 -newkey rsa:2048 -keyout dtls/key.pem -out dtls/certificate.pem -days 3650 -nodes`. It does not need to be signed.

### Debug

The Debug configuration is used to enable the game recorder. This is mostly useful when developing Impostor.

| Key                     | Default | Value                                        |
| ----------------------- | ------- | -------------------------------------------- |
| __GameRecorderEnabled__ | `false` | Enables the Game Recorder.                   |
| __GameRecorderPath__    | _empty_ | Path where the recorded games will be saved. |

### ServerRedirector

In a multi-node setup these values need to be specified. Note that most people do not need to run a Multi-node setup.

| Key                    | Default | Value                                                                                                                                             |
| ---------------------- | ------- | ------------------------------------------------------------------------------------------------------------------------------------------------- |
| __Enabled__            | `false` | Whether the server runs in multi-node setup. If this is `false`, all other options in this section do not have any effect.                        |
| __Master__             | `false` | Whether the current server is a master. A master is responsible for redirecting clients to nodes                                                  |
| __Locator__            |         | Fill in either `Redis` or `UdpMasterEndpoint` to choose which method to use for locating other nodes. This must be the same across all servers.   |
| __>Redis__             |         | Format `127.0.0.1.6379`, you can also use a password like so: `127.0.0.1.6379,password=value`.                                                    |
| __>UdpMasterEndpoint__ |         | On the master, this value acts as a listen ip and port. On a node, this should be the public ip and port of the master. Format `127.0.0.1:32320`. |
| __Nodes__              |         | An array containing public ips and ports of nodes. Only needs to be set on the master. See above for an example.                                  |

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
