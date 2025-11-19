# MorePlayers Plugin for Impostor

A plugin that enables Among Us servers to support more than the standard 15 player limit, allowing games with 20-30+ players.

## Features

- **Increased Player Cap**: Support for 20-30+ players per game (configurable)
- **Automatic Game Creation**: Optionally auto-create high-capacity games on startup
- **Smart Impostor Scaling**: Automatically calculate impostor count based on player count
- **Color Sharing Support**: Works with color sharing when more than 18 players join
- **Comprehensive Logging**: Monitor game events, player actions, and color assignments
- **Unmodified Client Compatible**: Works with vanilla Among Us clients

## Installation

1. **Build the Plugin**:
   ```bash
   cd src
   dotnet build Impostor.Plugins.MorePlayers/Impostor.Plugins.MorePlayers.csproj -c Release
   ```

2. **Copy to Plugins Folder**:
   ```bash
   cp Impostor.Plugins.MorePlayers/bin/Release/net8.0/Impostor.Plugins.MorePlayers.dll /path/to/impostor/plugins/
   ```

3. **Configure Server** (CRITICAL - see below)

## Critical Configuration Requirement

⚠️ **IMPORTANT**: For games with more than 18 players to work, you **MUST** disable color limit checks in the server configuration.

Edit your server's `config.json` file and set:

```json
{
  "AntiCheat": {
    "EnableColorLimitChecks": false
  }
}
```

**Why?** Among Us has 18 colors. When 19+ players join, some must share colors. The server's anti-cheat normally rejects duplicate colors, so this must be disabled.

**Security Note**: This only disables the color uniqueness check. All other anti-cheat features remain active.

## Configuration

The plugin can be configured by adding a `MorePlayers` section to your server's configuration file (typically `config.json` or `appsettings.json`):

```json
{
  "MorePlayers": {
    "MaxPlayers": 20,
    "ImpostorRatio": 5,
    "AutoCreateGame": true,
    "GameName": "High Capacity Game",
    "MakeGamePublic": false
  }
}
```

### Configuration Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `MaxPlayers` | byte | 20 | Maximum players allowed (10-255) |
| `ImpostorRatio` | int | 5 | Players per impostor (3-10) |
| `AutoCreateGame` | bool | true | Auto-create a game on startup |
| `GameName` | string | "High Capacity Game" | Display name for auto-created game |
| `MakeGamePublic` | bool | false | Make auto-created game public |

**Calculated Impostors**: The plugin automatically calculates impostor count using: `ceiling(MaxPlayers / ImpostorRatio)`

**Examples**:
- 20 players, ratio 5 → 4 impostors
- 30 players, ratio 5 → 6 impostors
- 25 players, ratio 6 → 5 impostors

## How It Works

### Player Counts 10-18
- Normal Among Us behavior
- Each player gets a unique color
- All standard features work as expected

### Player Counts 19+
- **Color sharing** enabled (multiple players can have the same color)
- Players differentiated by names instead of colors alone
- Server validates that `EnableColorLimitChecks = false` is set
- All other game mechanics work normally

### Systems That Scale Automatically
✅ Voting system - handles any player count
✅ Task assignment - distributes tasks to all players
✅ Role distribution - scales with impostor count
✅ Game ending logic - works with any player/impostor ratio
✅ Meeting mechanics - all players can vote

### Known Limitations

#### Client UI (Cannot be changed without modding clients)
- **Meeting room UI**: May be cramped with 20+ players
- **Player list**: May require scrolling
- **Spawn positions**: Players may spawn close together on some maps

These are client-side limitations and cannot be resolved without client mods. However, **the game remains fully playable**.

## Recommended Settings

### Conservative (Tested/Stable)
```json
{
  "MaxPlayers": 20,
  "ImpostorRatio": 5
}
```
- **20 players**: Good balance, proven stable
- **4 impostors**: Balanced gameplay

### Moderate (Adventurous)
```json
{
  "MaxPlayers": 30,
  "ImpostorRatio": 5
}
```
- **30 players**: More chaotic, fun for large groups
- **6 impostors**: Maintains balance

### Maximum (Experimental)
```json
{
  "MaxPlayers": 50,
  "ImpostorRatio": 6
}
```
- **50 players**: Extreme chaos
- **8-9 impostors**: May need adjustment based on testing
- ⚠️ **Warning**: Client UI will be very cramped, network performance may vary

## Testing Checklist

When testing with increased player counts, verify:

- [ ] All players can join the game
- [ ] All players can select colors (with sharing if >18)
- [ ] Players can spawn without issues
- [ ] Tasks are assigned to all players
- [ ] Meetings can be called and all players shown
- [ ] Voting completes successfully
- [ ] Game ending works correctly (impostor/crew wins)
- [ ] No server crashes or errors
- [ ] Network performance is acceptable

## Troubleshooting

### Players beyond 18th can't select colors
**Solution**: Ensure `AntiCheat.EnableColorLimitChecks = false` in server config.

### Game doesn't auto-create
**Solution**: Check that `AutoCreateGame = true` in plugin config.

### Too many/few impostors
**Solution**: Adjust `ImpostorRatio` setting. Higher = fewer impostors.

### Client UI is cramped
**Expected**: This is a client-side limitation. Game is still playable but UI may require scrolling.

### Network lag with many players
**Solution**:
- Reduce `MaxPlayers` to 20-25
- Ensure server has adequate bandwidth
- Check server hardware can handle load

## Development

### Building
```bash
dotnet build -c Release
```

### Testing
```bash
dotnet test
```

### Structure
```
Impostor.Plugins.MorePlayers/
├── Configuration/
│   └── MorePlayersConfig.cs      # Plugin configuration
├── Handlers/
│   ├── GameEventHandler.cs       # Game-level event monitoring
│   └── PlayerEventHandler.cs     # Player-level event monitoring
├── MorePlayersPlugin.cs           # Main plugin class
├── MorePlayersStartup.cs          # DI registration
└── README.md                      # This file
```

## Version History

### v1.0.0
- Initial release
- Support for 20-30+ players
- Automatic impostor scaling
- Color sharing support
- Comprehensive event logging

## Credits

- Built for [Impostor](https://github.com/Impostor/Impostor)
- Compatible with Among Us (latest version)
- Created as part of the Impostor plugin ecosystem

## License

This plugin follows the same license as the Impostor project (GNU GPLv3).

## Support

For issues, questions, or contributions:
1. Check the [Troubleshooting](#troubleshooting) section
2. Review server logs for errors
3. Open an issue on the Impostor repository
4. Join the Impostor Discord for community support

## Important Notes

⚠️ **Anti-Cheat Configuration**: Remember to set `EnableColorLimitChecks = false`
⚠️ **Client Compatibility**: Requires unmodified Among Us clients (latest version)
⚠️ **Network Performance**: Test with your expected player count before going live
⚠️ **Gameplay Balance**: More players = more chaos. Adjust impostor ratio as needed

Enjoy your high-capacity Among Us games! 🎮🚀
