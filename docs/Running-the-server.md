# Running the server

There are currently two install methods for Impostor: You can install it normally or inside of a Docker container. If you do not have a particular preference, we recommend the normal installation over the Docker container method.

## General remarks for both install methods

This section applies to both the normal installation as well as the Docker (Compose) installation

To connect to the server, you need to configure and install a region file on https://impostor.github.io/Impostor/

Among Us connects to the server using two network services: the (TCP) HTTP service points Among Us to the UDP service, then the UDP service hosts the actual game traffic. Because of this, Impostor uses port 22023 using **both** the TCP and UDP protocols.

As the phone version of Among Us requires this HTTP connection to be secure, we recommend using a HTTP reverse proxy to terminate SSL on the HTTP service. Setup instructions for this are in the [Http Server documentation](Http-server.md).

Depending on your host you may also need to port forward Impostor to the internet or pass Impostor traffic by your firewall. Port 22023 UDP needs to be accessible for everyone that wants to play on the server, then you also need to portforward your HTTP reverse proxy or port 22023 TCP if you don't use a reverse proxy. As port forwarding changes per host or router configuration, port forwarding is not covered by this guide.

## Normal installation

1. Install [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0). We recommend either the ASP.NET Core Runtime or the SDK. The SDK is necessary in case you want to develop Impostor or Impostor plugins.
2. Download the [latest release](https://github.com/Impostor/Impostor/releases) or the [latest CI build](https://nightly.link/Impostor/Impostor/workflows/ci/master). Note that Impostor is built for multiple CPU-architectures and operating systems, you most likely want the x64 version, unless you are running on a Raspberry Pi or another device/VPS with an Arm processor.
3. Extract the zip.
4. Modify `config.json` to your liking. Documentation can be found [here](Server-configuration.md). You need to at least change `PublicIp` to the address people will connect to your server to.
5. Run `Impostor.Server` (Linux/macOS) or `Impostor.Server.exe` (Windows)
6. (OPTIONAL - Linux) Configure a systemd definition file and enable the service to start on boot, see [systemd configuration](Server-configuration.md#systemd)
7. (OPTIONAL) Set up a reverse proxy to support HTTPS, so mobile phones can connect, see [reverse proxy configuration](Http-server.md)

## Using Docker

[![Docker Image](https://img.shields.io/docker/v/aeonlucid/impostor?sort=semver)](https://hub.docker.com/r/aeonlucid/impostor)
[![Docker Image](https://img.shields.io/docker/v/aeonlucid/impostor/nightly)](https://hub.docker.com/r/aeonlucid/impostor)

Docker is a program that allows you to run programs like Impostor in a container.

After installing Docker, you can just start a Docker container with `docker run`:

```
docker run -p 22023:22023/tcp -p 22023:22023/udp -e IMPOSTOR_Server__PublicIp=your.public.ip.here aeonlucid/impostor:nightly
```

Please replace `your.public.ip.here` with the public IP address of your server. This is the address Among Us will try to reach your server at.

To configure the docker container, either use environment variables or mount config.json in your container.

## Using Docker Compose

Docker Compose allows you to start a Docker container using a prefined configuration. This is an example configuration you can continue on:

```
version: '3.4'

services:
  impostor:
    image: aeonlucid/impostor:nightly
    container_name: impostor
    ports:
      - 22023:22023/tcp # Add "127.0.0.1:" if you're using a reverse proxy to terminate HTTPS
      - 22023:22023/udp
    environment: # Either configure Impostor using environment variables or mount a copy of config.json
      - IMPOSTOR_Server__PublicIp=your.public.ip.here
    volumes:
      - /path/to/local/config.json:/app/config.json # For easy editing of the config
      - /path/to/local/plugins:/app/plugins         # Only needed if using plugins
      - /path/to/local/libraries:/app/libraries     # Only needed if using external libraries
```
