# Impostor

Impostor is one of the first **Among Us** private servers, written in C#.

The latest version supported is `2020.9.22`, both desktop and mobile.

There are no special features at this moment, the goal is aiming to be as close as possible to the real server, for now. In a later stage, making modifications to game logic by modifying `GameData` packets can be looked at.

[Discord server for Impostor](https://discord.gg/Mk3w6Tb)

## Features

All Among Us features are implemented. It is a full replacement for the official server.

- [x] Create game
- [x] Join game
- [x] Start game
- [x] Game listing
- [x] V1 GameCodes (AAAA)
- [x] V2 GameCodes (AAAAAA)
- [x] Kick player
- [x] Ban player
- [x] Server redirection
- [ ] Multiple master servers

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

If you want to contribute to Impostor, make sure you work on and target the `dev` branch. That is where the latest changes will be. 

## License

This software is distributed under the **GNU GPLv3** License.

## Credits

- [willardf/Hazel-Networking](https://github.com/willardf/Hazel-Networking)
