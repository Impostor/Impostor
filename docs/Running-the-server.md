# Running the server

There are currently two modes to run the Impostor server in. The first way is the simplest one and is the one you should probably use. The other way will distribute players across other servers and is a more advanced configuration.

## Single server

### Without docker
1. Find the [latest release](https://github.com/AeonLucid/Impostor/releases/latest).
2. Download either the Windows or the Linux version.
3. Extract the zip.
4. Modify `config.json` to your liking.
5. Run `Impostor.Server.exe` (Windows) / `Impostor.Server` (Linux)

### Using docker

[![Docker Image](https://img.shields.io/docker/v/aeonlucid/impostor?sort=semver)](https://hub.docker.com/repository/docker/aeonlucid/impostor)
[![Docker Image](https://img.shields.io/docker/v/aeonlucid/impostor/edge)](https://hub.docker.com/repository/docker/aeonlucid/impostor)

```
docker run -p 22023:22023/udp aeonlucid/impostor
```

## Multiple servers

**This is only in the `dev` branch at this moment so you will need to build it yourself.**

Follow the steps from the single server on two or more servers.

### Master server

The master server will accept client connections and redirect them to the other servers listed in the configuration. It will not host any games itself.

Example configuration:

```json
{
  "Server": {
    "PublicIp": "127.0.0.1",
    "PublicPort": 22023,
    "ListenIp": "0.0.0.0",
    "ListenPort": 22023
  },
  "ServerRedirector": {
    "Enabled": true,
    "Master": true,
    "Locator": {
      "Redis": "",
      "UdpMasterEndpoint": "127.0.0.1:22023"
    },
    "Nodes": [
      {
        "Ip": "127.0.0.1",
        "Port": 22024
      }
    ]
  }
}
```

### Node servers

The node server should have `ServerRedirector` enabled too, but `Master` **must be disabled**. Nodes do not need to be aware of each other.

Example configuration:

```json
{
  "Server": {
    "PublicIp": "127.0.0.1",
    "PublicPort": 22024,
    "ListenIp": "0.0.0.0",
    "ListenPort": 22024
  },
  "ServerRedirector": {
    "Enabled": false,
    "Master": true,
    "Locator": {
      "Redis": "",
      "UdpMasterEndpoint": "127.0.0.1:22023"
    }
  }
}
```