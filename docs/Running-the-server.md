# Running the server

There are currently two modes to run the Impostor server. The first way, Single Server, is the simplest one, can handle up to a few hundred simultaneous players, and is the one you should probably use. Multi-Server mode will distribute players across other servers and is intended for advanced users.

## Single server

### Without Docker

1. Install the [.NET 6.0 runtime](https://dotnet.microsoft.com/download/dotnet/6.0). Installing the SDK also works, but is not necessary unless you plan on developing Impostor or Impostor plugins. If you're asked to pick between the normal, desktop or server runtime, the normal runtime is enough, but some plugins may require the ASP.NET Core Runtime as well.
2. Find the [latest release](https://github.com/Impostor/Impostor/releases) or the [latest CI build](https://nightly.link/Impostor/Impostor/workflows/ci/master).
3. Download the version for your OS (linux/win/osx). Impostor is built for multiple CPU-architectures, you most likely want the x64 version, unless you are running on a Raspberry Pi or another device with an ARM processor.
4. Extract the zip.
5. Modify `config.json` to your liking. Documentation can be found [here](Server-configuration.md) _(this step is mandatory if you want to expose this server to other devices)_
6. Run `Impostor.Server` (Linux/macOS) or `Impostor.Server.exe` (Windows)

Depending on your host you may also need to port forward Impostor to the internet. By default Impostor uses port **22023** and the **UDP** protocol. As port forwarding changes per host or router configuration, port forwarding is not covered by this guide.

To connect to the server, you need to configure a region file on https://impostor.github.io/Impostor/

### Using Docker

[![Docker Image](https://img.shields.io/docker/v/aeonlucid/impostor?sort=semver)](https://hub.docker.com/r/aeonlucid/impostor)
[![Docker Image](https://img.shields.io/docker/v/aeonlucid/impostor/nightly)](https://hub.docker.com/r/aeonlucid/impostor)

After installing Docker, you can just start a Docker container with `docker run`:

```
docker run -p 22023:22023/udp aeonlucid/impostor:nightly
```

### Using Docker Compose

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

Follow the steps from the single server on two or more servers. You should only need to set this up if you have a very large server and can no longer fit everyone on one server: if this is your first time installing Impostor, use a single server.

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
