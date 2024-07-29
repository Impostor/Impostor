using Impostor.Api.Innersloth.GameOptions.RoleOptions;

namespace Impostor.Api.Innersloth.GameOptions;

public class LegacyGameOptionsData : IGameOptions
{
    /// <summary>
    ///     The latest major version of the game client.
    /// </summary>
    public const int LatestVersion = 5;

    public LegacyGameOptionsData(byte version = LatestVersion)
    {
        Version = version;
    }

    /// <summary>
    ///     Gets or sets host's version of the game.
    /// </summary>
    public byte Version { get; set; }

    public GameModes GameMode => GameModes.Normal;

    /// <inheritdoc />
    public SpecialGameModes SpecialMode { get; } = SpecialGameModes.None;

    /// <inheritdoc />
    public RulesPresets RulesPreset { get; } = RulesPresets.Custom;

    /// <summary>
    ///     Gets or sets the maximum amount of players for this lobby.
    /// </summary>
    public byte MaxPlayers { get; set; } = 10;

    /// <summary>
    ///     Gets or sets the language of the lobby as per <see cref="GameKeywords" /> enum.
    /// </summary>
    public GameKeywords Keywords { get; set; } = GameKeywords.English;

    /// <summary>
    ///     Gets or sets the Map selected for this lobby.
    /// </summary>
    public MapTypes Map { get; set; } = MapTypes.Skeld;

    /// <summary>
    ///     Gets or sets the Player speed modifier.
    /// </summary>
    public float PlayerSpeedMod { get; set; } = 1f;

    /// <summary>
    ///     Gets or sets the Light modifier for the players that are members of the crew as a multiplier value.
    /// </summary>
    public float CrewLightMod { get; set; } = 1f;

    /// <summary>
    ///     Gets or sets the Light modifier for the players that are Impostors as a multiplier value.
    /// </summary>
    public float ImpostorLightMod { get; set; } = 1f;

    /// <summary>
    ///     Gets or sets the Impostor cooldown to kill in seconds.
    /// </summary>
    public float KillCooldown { get; set; } = 15f;

    /// <summary>
    ///     Gets or sets the number of common tasks.
    /// </summary>
    public int NumCommonTasks { get; set; } = 1;

    /// <summary>
    ///     Gets or sets the number of long tasks.
    /// </summary>
    public int NumLongTasks { get; set; } = 1;

    /// <summary>
    ///     Gets or sets the number of short tasks.
    /// </summary>
    public int NumShortTasks { get; set; } = 2;

    /// <summary>
    ///     Gets or sets the maximum amount of emergency meetings each player can call during the game in seconds.
    /// </summary>
    public int NumEmergencyMeetings { get; set; } = 1;

    /// <summary>
    ///     Gets or sets the cooldown between each time any player can call an emergency meeting in seconds.
    /// </summary>
    public int EmergencyCooldown { get; set; } = 15;

    /// <summary>
    ///     Gets or sets the number of impostors for this lobby.
    /// </summary>
    public int NumImpostors { get; set; } = 1;

    /// <summary>
    ///     Gets or sets a value indicating whether ghosts (dead crew members) can do tasks.
    /// </summary>
    public bool GhostsDoTasks { get; set; } = true;

    /// <summary>
    ///     Gets or sets the Kill as per values in <see cref="KillDistances" />.
    /// </summary>
    public KillDistances KillDistance { get; set; } = KillDistances.Normal;

    /// <summary>
    ///     Gets or sets the time for discussion before voting time in seconds.
    /// </summary>
    public int DiscussionTime { get; set; } = 15;

    /// <summary>
    ///     Gets or sets the time for voting in seconds.
    /// </summary>
    public int VotingTime { get; set; } = 120;

    /// <summary>
    ///     Gets or sets a value indicating whether an ejected player is an impostor or not.
    /// </summary>
    public bool ConfirmImpostor { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether players are able to see tasks being performed by other players.
    /// </summary>
    /// <remarks>
    ///     By being set to true, tasks such as Empty Garbage, Submit Scan, Clear asteroids, Prime shields execution will be visible to other players.
    /// </remarks>
    public bool VisualTasks { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether the vote is anonymous.
    /// </summary>
    public bool AnonymousVotes { get; set; }

    /// <summary>
    ///     Gets or sets the task bar update mode as per values in <see cref="Innersloth.TaskBarUpdate" />.
    /// </summary>
    public TaskBarUpdate TaskBarUpdate { get; set; } = TaskBarUpdate.Always;

    /// <summary>
    ///     Gets or sets role options.
    /// </summary>
    public LegacyRoleOptionsData RoleOptions { get; set; } = new LegacyRoleOptionsData();

    /// <summary>
    ///     Gets or sets a value indicating whether the GameOptions are the default ones.
    /// </summary>
    public bool IsDefaults { get; set; } = true;

    public static LegacyGameOptionsData Deserialize(IMessageReader reader, byte version)
    {
        var options = new LegacyGameOptionsData(version);
        options.Deserialize(reader);
        return options;
    }

    public void Deserialize(IMessageReader reader)
    {
        Version = reader.ReadByte();
        MaxPlayers = reader.ReadByte();
        Keywords = (GameKeywords)reader.ReadUInt32();
        Map = (MapTypes)reader.ReadByte();
        PlayerSpeedMod = reader.ReadSingle();

        CrewLightMod = reader.ReadSingle();
        ImpostorLightMod = reader.ReadSingle();
        KillCooldown = reader.ReadSingle();

        NumCommonTasks = reader.ReadByte();
        NumLongTasks = reader.ReadByte();
        NumShortTasks = reader.ReadByte();

        NumEmergencyMeetings = reader.ReadInt32();

        NumImpostors = reader.ReadByte();
        KillDistance = (KillDistances)reader.ReadByte();
        DiscussionTime = reader.ReadInt32();
        VotingTime = reader.ReadInt32();

        IsDefaults = reader.ReadBoolean();

        if (Version >= 2)
        {
            EmergencyCooldown = reader.ReadByte();
        }

        if (Version >= 3)
        {
            ConfirmImpostor = reader.ReadBoolean();
            VisualTasks = reader.ReadBoolean();
        }

        if (Version >= 4)
        {
            AnonymousVotes = reader.ReadBoolean();
            TaskBarUpdate = (TaskBarUpdate)reader.ReadByte();
        }

        if (Version >= 5)
        {
            RoleOptions = LegacyRoleOptionsData.Deserialize(reader);
        }

        if (Version >= 6)
        {
            // Nothing was changed in V6
        }

        if (Version > 6)
        {
            IGameOptions.ThrowUnknownVersion<LegacyGameOptionsData>(Version);
        }
    }

    /// <summary>
    ///     Serializes this instance of GameOptionsData object to a specified BinaryWriter.
    /// </summary>
    /// <param name="writer">The stream to write the message to.</param>
    public void Serialize(IMessageWriter writer)
    {
        writer.Write((byte)Version);
        writer.Write((byte)MaxPlayers);
        writer.Write((uint)Keywords);
        writer.Write((byte)Map);
        writer.Write((float)PlayerSpeedMod);
        writer.Write((float)CrewLightMod);
        writer.Write((float)ImpostorLightMod);
        writer.Write((float)KillCooldown);
        writer.Write((byte)NumCommonTasks);
        writer.Write((byte)NumLongTasks);
        writer.Write((byte)NumShortTasks);
        writer.Write((int)NumEmergencyMeetings);
        writer.Write((byte)NumImpostors);
        writer.Write((byte)KillDistance);
        writer.Write((uint)DiscussionTime);
        writer.Write((uint)VotingTime);
        writer.Write((bool)IsDefaults);

        if (Version >= 2)
        {
            writer.Write((byte)EmergencyCooldown);
        }

        if (Version >= 3)
        {
            writer.Write((bool)ConfirmImpostor);
            writer.Write((bool)VisualTasks);
        }

        if (Version >= 4)
        {
            writer.Write((bool)AnonymousVotes);
            writer.Write((byte)TaskBarUpdate);
        }

        if (Version >= 5)
        {
            RoleOptions.Serialize(writer);
        }

        if (Version >= 6)
        {
            // Nothing was changed in V6
        }

        if (Version > 6)
        {
            throw new ImpostorException($"Unknown {nameof(LegacyGameOptionsData)} version {Version}");
        }
    }
}
