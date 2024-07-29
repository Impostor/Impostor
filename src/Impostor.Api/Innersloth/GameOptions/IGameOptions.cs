using System.Diagnostics.CodeAnalysis;

namespace Impostor.Api.Innersloth.GameOptions;

public interface IGameOptions
{
    /// <summary>
    ///     Gets the version.
    /// </summary>
    public byte Version { get; }

    /// <summary>
    ///     Gets the currently active gamemode. This is currently used for the Normal and HideAndSeek gamemodes.
    /// </summary>
    public GameModes GameMode { get; }

    /// <summary>
    ///    Gets the currently active special gamemode. This is currently used for the AprilFools gamemode.
    /// </summary>
    public SpecialGameModes SpecialMode { get; }

    /// <summary>
    ///    Gets the rule preset.
    /// </summary>
    public RulesPresets RulesPreset { get; }

    /// <summary>
    ///     Gets or sets the maximum amount of players for this lobby.
    /// </summary>
    public byte MaxPlayers { get; set; }

    /// <summary>
    ///     Gets or sets the language of the lobby as per <see cref="GameKeywords" /> enum.
    /// </summary>
    public GameKeywords Keywords { get; set; }

    /// <summary>
    ///     Gets or sets the Map selected for this lobby.
    /// </summary>
    public MapTypes Map { get; set; }

    /// <summary>
    ///     Gets or sets the number of impostors for this lobby.
    /// </summary>
    public int NumImpostors { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the GameOptions are the default ones.
    /// </summary>
    public bool IsDefaults { get; set; }

    public void Serialize(IMessageWriter writer);

    public static void EnsureVersionIsModular<TCaller>(byte version)
    {
        if (version < GameOptionsFactory.ModularOptionsDataVersion)
        {
            throw new ImpostorException($"{typeof(TCaller).Name} didn't exist before version 7, did you mean {nameof(LegacyGameOptionsData)}?");
        }
    }

    [DoesNotReturn]
    public static void ThrowUnknownVersion<TCaller>(byte version)
    {
        throw new ImpostorException($"Unknown {typeof(TCaller).Name} version {version}");
    }
}
