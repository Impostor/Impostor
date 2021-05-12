# Running the server

There are currently two modes to run the Impostor server in. The first way is the simplest one and is the one you should probably use. The other way will distribute players across other servers and is a more advanced configuration.

## Single server

### Without docker
1. Install the **.NET 5.0 runtime**.
    - [Windows x64](https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-5.0.0-windows-x64-installer)
    - [Linux x64](https://docs.microsoft.com/en-us/dotnet/core/install/linux)
    - [macOS x64](https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-5.0.0-macos-x64-installer)
2. Find the [latest master release](https://ci.appveyor.com/project/Impostor/Impostor/branch/master/artifacts).
3. Download either the Windows or the Linux version.
4. Extract the zip.
5. Modify `config.json` to your liking. Documentation can be found [here](Server-configuration.md) *(this step is mandatory if you want to expose this server to other devices)*
6. Run `Impostor.Server.exe` (Windows) / `Impostor.Server` (Linux)

### Using docker

[![Docker Image](https://img.shields.io/docker/v/aeonlucid/impostor?sort=semver)](https://hub.docker.com/repository/docker/aeonlucid/impostor)
[![Docker Image](https://img.shields.io/docker/v/aeonlucid/impostor/nightly)](https://hub.docker.com/repository/docker/aeonlucid/impostor)

```
docker run -p 22023:22023/udp aeonlucid/impostor:nightly
```

### Using docker-compose
```
version: '3.4'

services:
  impostor:
    image: aeonlucid/impostor:nightly
    container_name: impostor
    ports:
      - 22023:22023/udp
    volumes:
      - /path/to/local/config.json:/app/config.json # For easy editing of the config
      - /path/to/local/plugins:/app/plugins         # Only needed if using plugins
      - /path/to/local/libraries:/app/libraries     # Only needed if using external libraries
```

## Multiple servers

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
      "UdpMasterEndpoint": "127.0.0.1:22024"
    },
    "Nodes": [
      {
        "Ip": "127.0.0.1",
        "Port": 22025
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
    "PublicPort": 22025,
    "ListenIp": "0.0.0.0",
    "ListenPort": 22025
  },
  "ServerRedirector": {
    "Enabled": true,
    "Master": false,
    "Locator": {
      "Redis": "",
      "UdpMasterEndpoint": "127.0.0.1:22024"
    }
  }
}
```
