using System;
using System.IO;
using Impostor.Shared.Innersloth.Data;

namespace Impostor.Shared.Innersloth
{
    public class GameOptionsData
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
        public bool GhostsDoTasks { get; set; }
        public int KillDistance { get; set; }
        public int DiscussionTime { get; set; }
        public int VotingTime { get; set; }
        public bool ConfirmImpostor { get; set; }
        public bool VisualTasks { get; set; }
        public bool IsDefaults { get; set; }
        
        public void Serialize(BinaryWriter writer, byte version)
        {
            writer.Write((byte) version);
            writer.Write((byte) MaxPlayers);
            writer.Write((uint) Keywords);
            writer.Write((byte) MapId);
            writer.Write((float) PlayerSpeedMod);
            writer.Write((float) CrewLightMod);
            writer.Write((float) ImpostorLightMod);
            writer.Write((float) KillCooldown);
            writer.Write((byte) NumCommonTasks);
            writer.Write((byte) NumLongTasks);
            writer.Write((byte) NumShortTasks);
            writer.Write((int) NumEmergencyMeetings);
            writer.Write((byte) NumImpostors);
            writer.Write((byte) KillDistance);
            writer.Write((uint) DiscussionTime);
            writer.Write((uint) VotingTime);
            writer.Write((bool) IsDefaults);
            if (version > 1)
            {
                writer.Write((byte) EmergencyCooldown);
            }

            if (version > 2)
            {
                writer.Write((bool) ConfirmImpostor);
                writer.Write((bool) VisualTasks);
            }
        }

        public static GameOptionsData Deserialize(ReadOnlyMemory<byte> bytes)
        {
            // TODO: Remove memory allocation.

            using (var stream = new MemoryStream(bytes.ToArray()))
            using (var reader = new BinaryReader(stream))
            {
                var result = new GameOptionsData();

                result.Version = reader.ReadByte();
                result.MaxPlayers = reader.ReadByte();
                result.Keywords = (GameKeywords) reader.ReadUInt32();
                result.MapId = reader.ReadByte();
                result.PlayerSpeedMod = reader.ReadSingle();
                result.CrewLightMod = reader.ReadSingle();
                result.ImpostorLightMod = reader.ReadSingle();
                result.KillCooldown = reader.ReadSingle();
                result.NumCommonTasks = reader.ReadByte();
                result.NumLongTasks = reader.ReadByte();
                result.NumShortTasks = reader.ReadByte();
                result.NumEmergencyMeetings = reader.ReadInt32();
                result.NumImpostors = reader.ReadByte();
                result.KillDistance = reader.ReadByte();
                result.DiscussionTime = reader.ReadInt32();
                result.VotingTime = reader.ReadInt32();
                result.IsDefaults = reader.ReadBoolean();

                if (result.Version > 1)
                {
                    result.EmergencyCooldown = reader.ReadByte();
                }

                if (result.Version > 2)
                {
                    result.ConfirmImpostor = reader.ReadBoolean();
                    result.VisualTasks = reader.ReadBoolean();
                }
                
                return result;
            }
        }
    }
}