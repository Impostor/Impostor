# Server configuration

Some information about all the possible configurations. Click [here](https://github.com/AeonLucid/Impostor/blob/master/src/Impostor.Server/config.full.json) to see all the possible config options.

## Config providers

### File

The simplest option to configure is by using the `config.json` file next to the server executable.

### Command line arguments

TODO

```
Server:PublicIp=127.0.0.1
Server:PublicPort=22023
Server:ListenIp=0.0.0.0
Server:listenPort=22023
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
IMPOSTOR_Server__listenPort=22023
IMPOSTOR_ServerRedirector__Enabled=false
IMPOSTOR_ServerRedirector__Master=true
IMPOSTOR_ServerRedirector__Locator__Redis=127.0.0.1.6379
IMPOSTOR_ServerRedirector__Locator__UdpMasterEndpoint=127.0.0.1:32320
IMPOSTOR_ServerRedirector__Nodes__0__Ip=127.0.0.1
IMPOSTOR_ServerRedirector__Nodes__0__Port=22024
IMPOSTOR_ServerRedirector__Nodes__1__Ip=127.0.0.1
IMPOSTOR_ServerRedirector__Nodes__1__Port=22025
```

## Options

TODO - Write something about every option.