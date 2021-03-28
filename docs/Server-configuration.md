# Server configuration

Some information about all the possible configurations. Click [here](https://github.com/AeonLucid/Impostor/blob/master/src/Impostor.Server/config.full.json) to see all the possible config options.

## Options

### Required Server Configuration

| Key | Default | Description |
|-|-|-|
| **PublicIp** | `127.0.0.1` | This needs to the public IPv4 address of the server which you give to others to connect. You can find your IPv4 address [on this website](http://whatismyip.host/). Unless you are only planning to use Impostor privately, on your local network, you should change this to your public ip. It is also possible to use hostnames instead of IPv4 addresses, which will be resolved to IPv4 addresses. |
| **PublicPort** | `22023` | The public port of the server which you give to others to connect. (**This is the external port you configure on your router when port forwarding.**) Usually `22023`. |
| **ListenIp** | `0.0.0.0` | The network interface to listen on. If you do not know what to put here, use `0.0.0.0`. Since 1.2.2 it is also possible to use hostnames instead of IPv4 addresses, these must resolve to a valid IPv4 address. |
| **ListenPort** | `22023` | The listen port of the server, usually `22023`. |

### AntiCheat

| Key | Default | Value |
|-|-|-|
| **Enabled** | `true` | Whether the anticheat should be enabled. |
| **BanIpFromGame** | `true` | When a player is caught hacking, they will be kicked from the server. If this value is set to `true`, the player will be banned instead and will not be able to rejoin that specific game. **(Setting this to false does not disable the anti-cheat!)** |

### ServerRedirector
In a multi-node setup these need to be specified.
| Key | Default | Value |
|-|-|-|
| **Enabled** | `false` | Whether the server runs in multi-node setup. If this is `false`, all other options in this section do not have any effect. |
| **Master** | `false` | Whether the current server is a master. A master is responsible for redirecting clients to nodes |
| **Locator** | | Fill in either `Redis` or `UdpMasterEndpoint` to choose which method to use for locating other nodes. This must be the same across all servers. |
| **>Redis** | | Format `127.0.0.1.6379`, you can also use a password like so: `127.0.0.1.6379,password=value`. |
| **>UdpMasterEndpoint** | | On the master, this value acts as a listen ip and port. On a node, this should be the public ip and port of the master. Format `127.0.0.1:32320`. |
| **Nodes** | | An array containing public ips and ports of nodes. Only needs to be set on the master. See above for an example. |

## Config providers

### File

The simplest option to configure is by using the `config.json` file next to the server executable. For all possible options see the [config-full.json](https://github.com/Impostor/Impostor/blob/dev/src/Impostor.Server/config-full.json) file.

### Command line arguments

TODO

```
Server:PublicIp=127.0.0.1
Server:PublicPort=22023
Server:ListenIp=0.0.0.0
Server:ListenPort=22023
AntiCheat:Enabled=true
AntiCheat:BanIpFromGame=true
ServerRedirector:Enabled=false
ServerRedirector:Master=true
ServerRedirector:Locator:Redis=127.0.0.1.6379
ServerRedirector:Locator:UdpMasterEndpoint=127.0.0.1:32320
ServerRedirector:Nodes:0:Ip=127.0.0.1
ServerRedirector:Nodes:0:Port=22024
ServerRedirector:Nodes:1:Ip=127.0.0.1
ServerRedirector:Nodes:1:Port=22025
```

### Environment variables

TODO

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
