# Impostor

Impostor is one of the first **Among Us** private servers, written in C#.

The latest version supported is `2020.9.9`, both desktop and mobile.

There are no special features at this moment, the goal is aiming to be as close as possible to the real server, for now. In a later stage, making modifications to game logic by modifying `GameData` can be looked at.

[Discord server for Impostor](https://discord.gg/Mk3w6Tb)

## Features

Almost all standard features are implemented at this moment, except for public games. It is already possible to host and play private games.

- [x] Create game
- [x] Join game
- [x] Start game
- [ ] Find game
- [x] V1 GameCodes (AAAA)
- [x] V2 GameCodes (AAAAAA)
- [x] Kick player
- [x] Ban player
- [ ] Server redirection
- [ ] Server reselect

## Installation

If you just want to play, follow the client instructions.

When hosting a server, make sure port **22023** is open and forwarded for **UDP**. It is not possible to use a different port because Among Us seems to have hardcoded 22023, even though the functionality for setting a port exists.

### Client

If you just want to play on a server hosted by someone else, you need to follow these instructions.

1. Find the [latest release](https://github.com/AeonLucid/Impostor/releases/latest).
2. Download `Impostor-Client-win-x64.zip`.
3. Extract the zip.
4. Run `Impostor.Client.exe`.
5. Follow the instructions inside the application.

![Client](docs/images/client.jpg)

### Server (Docker)

[![Docker Image](https://img.shields.io/docker/v/aeonlucid/impostor?sort=semver)](https://hub.docker.com/repository/docker/aeonlucid/impostor)
[![Docker Image](https://img.shields.io/docker/v/aeonlucid/impostor/edge)](https://hub.docker.com/repository/docker/aeonlucid/impostor)

```
docker run -p 22023:22023/udp aeonlucid/impostor
```

### Server (Windows / Linux)

1. Find the [latest release](https://github.com/AeonLucid/Impostor/releases/latest).
2. Download either the Windows or the Linux version.
3. Extract the zip.
4. Run `Impostor.Server.exe` (Windows) / `Impostor.Server` (Linux)

## License

This software is distributed under the **GNU GPLv3** License.

## Credits

- [willardf/Hazel-Networking](https://github.com/willardf/Hazel-Networking)
