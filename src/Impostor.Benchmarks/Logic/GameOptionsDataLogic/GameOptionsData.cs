using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;

namespace Impostor.Benchmarks.Logic.GameOptionsDataLogic
{
    public partial class GameOptionsData
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
    }

    public partial class GameOptionsData
    {
        public static GameOptionsData DeserializeNew01(ReadOnlyMemory<byte> bytes)
        {
            //Unsafe.As<>

            var rented = ArrayPool<byte>.Shared.Rent(bytes.Length);
            try
            {
                bytes.CopyTo(rented);
                using var ms = new MemoryStream(rented, 0, bytes.Length, false);
                using var reader = new BinaryReader(ms);

                var result = new GameOptionsData();

                result.Version = reader.ReadByte();
                result.MaxPlayers = reader.ReadByte();
                result.Keywords = (GameKeywords)reader.ReadUInt32();
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
            finally
            {
                ArrayPool<byte>.Shared.Return(rented, false);
            }
        }

        public static GameOptionsData DeserializeNew02(ReadOnlyMemory<byte> memory)
        {
            var bytes = memory.Span;

            var position = 0;

            var result = new GameOptionsData();

            result.Version = bytes[position++];
            result.MaxPlayers = bytes[position++];

            result.Keywords = (GameKeywords)BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(position));
            position += sizeof(GameKeywords);

            result.MapId = bytes[position++];

            result.PlayerSpeedMod = BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian(bytes.Slice(position)));
            position += sizeof(Int32);

            result.CrewLightMod = BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian(bytes.Slice(position)));
            position += sizeof(Int32);

            result.ImpostorLightMod = BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian(bytes.Slice(position)));
            position += sizeof(Int32);

            result.KillCooldown = BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian(bytes.Slice(position)));
            position += sizeof(Int32);

            result.NumCommonTasks = bytes[position++];
            result.NumLongTasks = bytes[position++];
            result.NumShortTasks = bytes[position++];

            result.NumEmergencyMeetings = BinaryPrimitives.ReadInt32LittleEndian(bytes.Slice(position));
            position += sizeof(Int32);

            result.NumImpostors = bytes[position++];
            result.KillDistance = bytes[position++];
            result.DiscussionTime = BinaryPrimitives.ReadInt32LittleEndian(bytes.Slice(position));
            position += sizeof(Int32);

            result.VotingTime = BinaryPrimitives.ReadInt32LittleEndian(bytes.Slice(position));
            position += sizeof(Int32);

            result.IsDefaults = bytes[position++] != 0;

            if (result.Version > 1)
            {
                result.EmergencyCooldown = bytes[position++];
            }

            if (result.Version > 2)
            {
                result.ConfirmImpostor = bytes[position++] != 0;
                result.VisualTasks = bytes[position++] != 0;
            }
            return result;
        }

        public static GameOptionsData DeserializeNew03(ReadOnlyMemory<byte> memory)
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

        public static GameOptionsData DeserializeOld(ReadOnlyMemory<byte> bytes)
        {
            // TODO: Remove memory allocation.

            using (var stream = new MemoryStream(bytes.ToArray()))
            using (var reader = new BinaryReader(stream))
            {
                var result = new GameOptionsData();

                result.Version = reader.ReadByte();
                result.MaxPlayers = reader.ReadByte();
                result.Keywords = (GameKeywords)reader.ReadUInt32();
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