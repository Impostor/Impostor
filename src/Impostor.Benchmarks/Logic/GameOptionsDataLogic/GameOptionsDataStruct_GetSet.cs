using System;

namespace Impostor.Benchmarks.Logic.GameOptionsDataLogic
{
    public struct GameOptionsDataStruct_GetSet
    {
        public byte Version { get; set; }
        public byte MaxPlayers { get; set; }
        public GameKeywords Keywords { get; set; }
        public byte MapId { get; set; }
        public float PlayerSpeedMod { get; set; }
        public float CrewLightMod { get; set; }
        public float ImpostorLightMod { get; set; }
        public float KillCooldown { get; set; }
        public int NumCommonTasks { get; set; }
        public int NumLongTasks { get; set; }
        public int NumShortTasks { get; set; }
        public int NumEmergencyMeetings { get; set; }
        public int EmergencyCooldown { get; set; }
        public int NumImpostors { get; set; }

        public int KillDistance { get; set; }

        public int DiscussionTime { get; set; }
        public int VotingTime { get; set; }
        public bool ConfirmImpostor { get; set; }
        public bool VisualTasks { get; set; }
        public bool IsDefaults { get; set; }

        public GameOptionsDataStruct_GetSet(ReadOnlySpan<byte> bytes)
        {
            Version = bytes.ReadByte();
            MaxPlayers = bytes.ReadByte();
            Keywords = (GameKeywords)bytes.ReadUInt32();
            MapId = bytes.ReadByte();
            PlayerSpeedMod = bytes.ReadSingle();
            CrewLightMod = bytes.ReadSingle();
            ImpostorLightMod = bytes.ReadSingle();
            KillCooldown = bytes.ReadSingle();
            NumCommonTasks = bytes.ReadByte();
            NumLongTasks = bytes.ReadByte();
            NumShortTasks = bytes.ReadByte();
            NumEmergencyMeetings = bytes.ReadInt32();
            NumImpostors = bytes.ReadByte();
            KillDistance = bytes.ReadByte();
            DiscussionTime = bytes.ReadInt32();
            VotingTime = bytes.ReadInt32();
            IsDefaults = bytes.ReadBoolean();

            EmergencyCooldown = default;
            ConfirmImpostor = default;
            VisualTasks = default;

            if (Version > 1)
            {
                EmergencyCooldown = bytes.ReadByte();
            }

            if (Version > 2)
            {
                ConfirmImpostor = bytes.ReadBoolean();
                VisualTasks = bytes.ReadBoolean();
            }
        }
    }
}