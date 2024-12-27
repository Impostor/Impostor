# Troubleshooting

If you're reading this, something went wrong.
Don't worry though, as this is the most thorough guide to help you!

## `./Impostor.Server: line 1: ELF: not found` (plus other errors)

No idea where you got that system. But we clearly do **NOT** support it.

## `cannot execute binary file: Exec format error`

Please check that you have downloaded the right version of Impostor, as we mantain two CPU architectures (x64 and ARM).
Unless you are running Impostor on a computer like the Raspberry Pi, you most likely want to use the x64 version.

## `./Impostor.Server: Permission denied`

This is an error related to Linux file permissions.
Some files do not hold their executable bit (the permission that allows them to run) during a download.
You can solve this by doing: `chmod +x Impostor.Server`

## Disconnected with `Your client is too new, please update your Impostor to play`

This happens when your game is too new for your copy of Impostor to support.

If you're using an Impostor release, check if there is a newer release available that supports your version. If this doesn't exist, you can try the [latest build from CI](https://nightly.link/Impostor/Impostor/workflows/ci/master).

If even that build doesn't work, you can try looking through the Pull Requests to see if there is a pending pull request that aims to update it to the next version, but note these may be unstable and may contain malicious code, as they haven't been vetted by the Impostor development team yet.

## Disconnected with `Please update your game to play on this server`

This happens when you're using a version of Impostor that is too new for your game version. You can download an older version from [the GitHub release page](https://github.com/Impostor/Impostor/releases). We however don't support older versions of Impostor with new API's and bugfixes so we recommend to update your game instead.

## Disconnected with `You are using an older version of the game`

You are using a version of Impostor that is not designed for the version of the game you're playing. The game does not really check who is outdated and blames it on the user. Newer versions (v1.5.0+) do warn correctly and send the correct message.

Look at which version of the game you're playing, which you can see in the top left corner of the main menu, then download an Impostor version for that game. Every [release on the release page](https://github.com/Impostor/Impostor/releases) shows which version of the game it is compatible with. If your game version is newer than the latest release, check if the [latest build from CI works](https://nightly.link/Impostor/Impostor/workflows/ci/master).

## Disconnected with `You disconnected from the server. Reliable Packet 1 ...`

Please double-check that you have followed the [Server Configuration](Server-configuration.md) correctly.
**NOTE: Your public ip does not start with `10`, `127` or `192`**
Also check if the port Impostor (ListenPort) is listening on is correctly port-forwarded for UDP (or TCP/UDP).

## `Could not load file or assembly...`

Please check that you only have **working** plugins in the `plugins` folder.
This error can be caused by non-plugin files or plugins that are not working correctly.

## Disconnected with `You disconnected from the server. If this happens often, check your network strength. This may also be a server issue`

Usually this means that Among Us couldn't connect to Impostor's HTTP server.

Open the address of your server in a web browser, it should show you a small page confirming that Impostor is available.

## My question is not yet answered and I'm still having problems!

That's unfortunate. Join the [Impostor Discord](https://discord.gg/Mk3w6Tb), ask your question there and we'll try to help you out. Note that we're not always available, so it may take some time to get an answer. To make answering your question easier, please add the following details:

- Which version of Impostor are you using?
- Which version or Among Us are you using?
- Which operating system are you running Impostor on?
- If you have console logs, attaching these will also help.
