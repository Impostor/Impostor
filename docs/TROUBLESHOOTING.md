# Troubleshooting
If you're reading this, something went wrong.
Don't worry though, as this is the most thorough guide to help you!

## `./Impostor.Server: line 1: ELF: not found` (plus other errors)
No idea where you got that system. But we clearly do **NOT** support it.

## `cannot execute binary file: Exec format error`
Please check that you have downloaded the right version of Impostor, as we mantain two CPU architectures (x64 and ARM).  
Unless you are running Impostor on a SBC (Single-Board Computer), like the Raspberry Pi, you most likely want to use the x64 version.

## `./Impostor.Server: Permission denied`
This is an error related to Linux file permissions.  
You can solve this by doing: `chmod +x Impostor.Server`
_(Note: only run `chmod` when you trust the software. Linux blocks downloaded executables by default for a reason)_

## `You are using an older version of the game`
You are using an older version of Impostor. The game does not really check who is outdated and blames it on the user.  
Make sure you got the latest working version of Impostor (probably in AppVeyor, not Github).

## `You disconnected from the server. Reliable Packet 1 ...`
Please double-check that you have followed the [Server Configuration](Server-configuration.md) correctly.  
**NOTE: Your public ip does not start with `127` nor `192`**  
Also check if the port Impostor is listening on is *opened* in your firewall and that you correctly port-forwarded it with UDP (can be TCP/UDP, doesn't matter.)  

## `Could not load file or assembly...`
Please check that you only have **working** plugins in the `plugins` folder.  
This error can be caused by non-plugin files or plugins that are not working correctly.