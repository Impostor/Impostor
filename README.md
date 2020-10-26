# Impostor

[![Discord](https://img.shields.io/badge/Discord-chat-blue?style=flat-square)](https://discord.gg/Mk3w6Tb)
[![AppVeyor](https://img.shields.io/appveyor/build/Impostor/Impostor/dev?style=flat-square)](https://ci.appveyor.com/project/Impostor/Impostor/branch/dev)

Impostor is one of the first **Among Us** private servers, written in C#. 

We support steam, itch, android and iOS. The latest version supported is `2020.9.22`, the `dev` build currently supports `2020.10.22`.

| Impostor version | Among Us version | Experimental | Download |
|-|-|-|-|
| 1.1.0 | 2020.09.07 - 2020.09.22 | No | [![Download](https://img.shields.io/badge/Download-v1.1.0-blue?style=flat-square)](https://github.com/Impostor/Impostor/releases/tag/v1.1.0) |
| 1.2.2 | 2020.09.22 - 2020.10.22 | Yes | [![Download](https://img.shields.io/badge/Download-v1.2.2-blue?style=flat-square)](https://ci.appveyor.com/project/Impostor/Impostor/branch/dev/artifacts) |

There are no special features at this moment, the goal is aiming to be as close as possible to the real server, for now. In a later stage, making modifications to game logic by modifying `GameData` packets can be looked at.

## Features

- All Among Us features are implemented. It is a full replacement for the official server.
- Plugin support.
- Server-sided anticheat.

## Installation

### Client

If you just want to play on a server hosted by someone else, you need to follow these instructions.

1. Find the [latest release](https://github.com/AeonLucid/Impostor/releases/latest).
2. Download `Impostor-Client-win-x64.zip`.
3. Extract the zip.
4. Run `Impostor.Client.exe`.
5. Follow the instructions inside the application.

![Client](docs/images/client.jpg)

### Server

See the [docs](docs/Running-the-server.md) for instructions on how to set it up.

## Contributing

See [CONTRIBUTING](CONTRIBUTING.md).

## License

This software is distributed under the **GNU GPLv3** License.

## Credits

- [willardf/Hazel-Networking](https://github.com/willardf/Hazel-Networking)
