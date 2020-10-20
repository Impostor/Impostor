using Impostor.Api.Extensions;
using Impostor.Api.Innersloth.Data;

using System;
using System.IO;

namespace Impostor.Api.Innersloth
{
    public class GameOptionsData
    {
        public const int LatestVersion = 2;

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
        public bool GhostsDoTasks { get; set; }
        public int KillDistance { get; set; }
        public int DiscussionTime { get; set; }
        public int VotingTime { get; set; }
        public bool ConfirmImpostor { get; set; }
        public bool VisualTasks { get; set; }
        public bool IsDefaults { get; set; }

        public void Serialize(BinaryWriter writer, byte version)
        {
            writer.Write((byte)version);
            writer.Write((byte)MaxPlayers);
            writer.Write((uint)Keywords);
            writer.Write((byte)MapId);
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
            if (version > 1)
            {
                writer.Write((byte)EmergencyCooldown);
            }

            if (version > 2)
            {
                writer.Write((bool)ConfirmImpostor);
                writer.Write((bool)VisualTasks);
            }
        }

        public static GameOptionsData Deserialize(ReadOnlyMemory<byte> memory)
        {
            var bytes = memory.Span;

            var result = new GameOptionsData();
            result.Version = bytes.ReadByte();
            result.MaxPlayers = bytes.ReadByte();
            result.Keywords = (GameKeywords)bytes.ReadUInt32();
            result.MapId = bytes.ReadByte();
            result.PlayerSpeedMod = bytes.ReadSingle();

            result.CrewLightMod = bytes.ReadSingle();
            result.ImpostorLightMod = bytes.ReadSingle();
            result.KillCooldown = bytes.ReadSingle();

            result.NumCommonTasks = bytes.ReadByte();
            result.NumLongTasks = bytes.ReadByte();
            result.NumShortTasks = bytes.ReadByte();

            result.NumEmergencyMeetings = bytes.ReadInt32();

            result.NumImpostors = bytes.ReadByte();
            result.KillDistance = bytes.ReadByte();
            result.DiscussionTime = bytes.ReadInt32();
            result.VotingTime = bytes.ReadInt32();

            result.IsDefaults = bytes.ReadBoolean();

            if (result.Version > 1)
            {
                result.EmergencyCooldown = bytes.ReadByte();
            }

            if (result.Version > 2)
            {
                result.ConfirmImpostor = bytes.ReadBoolean();
                result.VisualTasks = bytes.ReadBoolean();
            }

            return result;
        }
    }
}