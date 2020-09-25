using System.IO;
using Hazel;
using Impostor.Shared.Innersloth;
using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Net.Messages
{
    internal static class Message00HostGame
    {
        public static void Serialize(MessageWriter writer, int gameCode)
        {
            writer.StartMessage(MessageFlags.HostGame);
            writer.Write(gameCode);
            writer.EndMessage();
        }

        public static GameOptionsData Deserialize(MessageReader reader)
        {
            var bytes = reader.ReadBytesAndSize();
            
            using (var stream = new MemoryStream(bytes))
            using (var binary = new BinaryReader(stream))
            {
                var result = new GameOptionsData
                {
                    Version = binary.ReadByte(),
                    MaxPlayers = binary.ReadByte(),
                    Keywords = (GameKeywords) binary.ReadUInt32(),
                    MapId = binary.ReadByte(),
                    PlayerSpeedMod = binary.ReadSingle(),
                    CrewLightMod = binary.ReadSingle(),
                    ImpostorLightMod = binary.ReadSingle(),
                    KillCooldown = binary.ReadSingle(),
                    NumCommonTasks = binary.ReadByte(),
                    NumLongTasks = binary.ReadByte(),
                    NumShortTasks = binary.ReadByte(),
                    NumEmergencyMeetings = binary.ReadInt32(),
                    NumImpostors = binary.ReadByte(),
                    KillDistance = binary.ReadByte(),
                    DiscussionTime = binary.ReadInt32(),
                    VotingTime = binary.ReadInt32(),
                    IsDefaults = binary.ReadBoolean()
                };

                if (result.Version > 1)
                {
                    result.EmergencyCooldown = binary.ReadByte();
                }

                if (result.Version > 2)
                {
                    result.ConfirmImpostor = binary.ReadBoolean();
                    result.VisualTasks = binary.ReadBoolean();
                }
                
                return result;
            }
        }
    }
}