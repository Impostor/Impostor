# Impostor

[![Discord](https://img.shields.io/badge/Discord-chat-blue?style=flat-square)](https://discord.gg/Mk3w6Tb)
[![AppVeyor](https://img.shields.io/appveyor/build/Impostor/Impostor/dev?style=flat-square)](https://ci.appveyor.com/project/Impostor/Impostor/branch/dev)

Impostor is one of the first **Among Us** private servers, written in C#. 

We support steam, itch, android and iOS. The latest version supported is `2020.9.22`, the `dev` build currently supports `2020.10.22`.

| Impostor version | Among Us version | Experimental | Download |
|-|-|-|-|
| 1.1.0 | 2020.09.07 - 2020.09.22 | No | [![Download](https://img.shields.io/badge/Download-v1.1.0-blue?style=flat-square)](https://github.com/Impostor/Impostor/releases/tag/v1.1.0) |
| 1.2.2 | 2020.09.22 - 2020.10.22 | Yes | [![Download](https://img.shields.io/badge/Download-v1.2.2-blue?style=flat-square)](https://ci.appveyor.com/project/Impostor/Impostor/branch/dev/artifacts) |

## Features

- All Among Us features are implemented. It is a full replacement for the official server.
- Plugin support.
- Server-sided anticheat.

## Installation

### Client

If you just want to play on a server hosted by someone else, you need to follow these instructions.

#### Windows

1. Find the [latest release](https://github.com/AeonLucid/Impostor/releases/latest).
2. Download `Impostor-Client-win-x64.zip`.
3. Extract the zip.
4. Run `Impostor.Client.exe`.
5. Follow the instructions inside the application.

![Client](docs/images/client.jpg)

If you do not wish to execute any program. Follow the instructions in [this website](https://impostor.github.io/Impostor)

#### Android

##### Android 10 and less.
1. Go to [this website](https://impostor.github.io/Impostor) **(IN YOUR MOBILE DEVICE)**
2. Follow its instructions.

##### Android 11.
1. Connect your phone to a computer. Go to [this website](https://impostor.github.io/Impostor) in your computer to generate a file named `regioninfo.dat`.
2. Instead of following the website instructions please go to `/sdcard/Android/data/com.innersloth.spacemafia` from the computer's file explorer and paste the file there.

#### IOS

Needs to be jailbroken, we don't provide instructions on this.

### Server

See the [docs](docs/Running-the-server.md) for instructions on how to set it up.

## Troubleshooting

See [TROUBLESHOOTING](docs/TROUBLESHOOTING.md) to solve issues with both the client and the server

## Contributing

See [CONTRIBUTING](CONTRIBUTING.md).

## License

This software is distributed under the **GNU GPLv3** License.

## Credits

- [willardf/Hazel-Networking](https://github.com/willardf/Hazel-Networking)
