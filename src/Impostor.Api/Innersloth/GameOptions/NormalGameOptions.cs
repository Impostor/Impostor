using Impostor.Api.Innersloth.GameOptions.RoleOptions;

namespace Impostor.Api.Innersloth.GameOptions;

public class NormalGameOptions : IGameOptions
{
    public const int LatestVersion = 10;

    public NormalGameOptions(byte version = LatestVersion)
    {
        Version = version;
        IGameOptions.EnsureVersionIsModular<NormalGameOptions>(version);
        RoleOptions = new RoleOptionsCollection(version);
    }

    /// <inheritdoc />
    public byte Version { get; }

    /// <inheritdoc />
    public GameModes GameMode => GameModes.Normal;

    /// <inheritdoc />
    public SpecialGameModes SpecialMode { get; set; } = SpecialGameModes.None;

    /// <inheritdoc />
    public RulesPresets RulesPreset { get; set; } = RulesPresets.Custom;

    /// <inheritdoc />
    public byte MaxPlayers { get; set; } = 10;

    /// <inheritdoc />
    public GameKeywords Keywords { get; set; } = GameKeywords.English;

    /// <inheritdoc />
    public MapTypes Map { get; set; } = MapTypes.Skeld;

    /// <inheritdoc />
    public int NumImpostors { get; set; } = 1;

    /// <inheritdoc />
    public bool IsDefaults { get; set; } = true;

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
    ///     Gets or sets the experience level of people in the lobby: Beginner, Intermediate or Expert.
    /// </summary>
    public GameTags Tag { get; set; } = 0;

    public RoleOptionsCollection RoleOptions { get; set; }

    public static NormalGameOptions Deserialize(IMessageReader reader, byte version)
    {
        var options = new NormalGameOptions(version);
        options.Deserialize(reader);
        return options;
    }

    public void Deserialize(IMessageReader reader)
    {
        if (Version >= 8)
        {
            SpecialMode = (SpecialGameModes)reader.ReadByte();
            RulesPreset = (RulesPresets)reader.ReadByte();
        }

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

        EmergencyCooldown = reader.ReadByte();
        ConfirmImpostor = reader.ReadBoolean();
        VisualTasks = reader.ReadBoolean();
        AnonymousVotes = reader.ReadBoolean();
        TaskBarUpdate = (TaskBarUpdate)reader.ReadByte();

        if (Version >= 9)
        {
            Tag = (GameTags)reader.ReadByte();
        }

        RoleOptions.Deserialize(reader);

        if (Version > LatestVersion)
        {
            IGameOptions.ThrowUnknownVersion<NormalGameOptions>(Version);
        }
    }

    public void Serialize(IMessageWriter writer)
    {
        if (Version >= 8)
        {
            writer.Write((byte)SpecialMode);
            writer.Write((byte)RulesPreset);
        }

        writer.Write(MaxPlayers);
        writer.Write((uint)Keywords);
        writer.Write((byte)Map);
        writer.Write(PlayerSpeedMod);

        writer.Write(CrewLightMod);
        writer.Write(ImpostorLightMod);
        writer.Write(KillCooldown);

        writer.Write((byte)NumCommonTasks);
        writer.Write((byte)NumLongTasks);
        writer.Write((byte)NumShortTasks);

        writer.Write(NumEmergencyMeetings);

        writer.Write((byte)NumImpostors);
        writer.Write((byte)KillDistance);
        writer.Write(DiscussionTime);
        writer.Write(VotingTime);

        writer.Write(IsDefaults);

        writer.Write((byte)EmergencyCooldown);
        writer.Write(ConfirmImpostor);
        writer.Write(VisualTasks);
        writer.Write(AnonymousVotes);
        writer.Write((byte)TaskBarUpdate);

        if (Version >= 9)
        {
            writer.Write((byte)Tag);
        }

        RoleOptions.Serialize(writer);

        if (Version > LatestVersion)
        {
            IGameOptions.ThrowUnknownVersion<NormalGameOptions>(Version);
        }
    }
}
