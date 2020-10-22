using System;
using System.Collections.Generic;
using System.Text;

namespace Impostor.Benchmarks.Logic.GameOptionsDataLogic
{
    public struct GameOptionsDataStruct_Fields
    {
        public byte Version;
        public byte MaxPlayers;
        public GameKeywords Keywords;
        public byte MapId;
        public float PlayerSpeedMod;
        public float CrewLightMod;
        public float ImpostorLightMod;
        public float KillCooldown;
        public int NumCommonTasks;
        public int NumLongTasks;
        public int NumShortTasks;
        public int NumEmergencyMeetings;
        public int EmergencyCooldown;
        public int NumImpostors;

        public int KillDistance;

        public int DiscussionTime;
        public int VotingTime;
        public bool ConfirmImpostor;
        public bool VisualTasks;
        public bool IsDefaults;

        public GameOptionsDataStruct_Fields(ReadOnlySpan<byte> bytes)
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
