using System;
using System.IO;
using Impostor.Api.Innersloth.Data;
using Impostor.Api.Net.Messages;

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

            if (Version > 3)
            {
                throw new ImpostorException($"Unknown GameOptionsData version {Version}.");
            }
        }

        public void Deserialize(ReadOnlyMemory<byte> memory)
        {
            var bytes = memory.Span;

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

            if (Version > 1)
            {
                EmergencyCooldown = bytes.ReadByte();
            }

            if (Version > 2)
            {
                ConfirmImpostor = bytes.ReadBoolean();
                VisualTasks = bytes.ReadBoolean();
            }

            if (Version > 3)
            {
                throw new ImpostorException($"Unknown GameOptionsData version {Version}.");
            }
        }

        public static GameOptionsData DeserializeCreate(IMessageReader reader)
        {
            var options = new GameOptionsData();
            options.Deserialize(reader.ReadBytesAndSize());
            return options;
        }
    }
}