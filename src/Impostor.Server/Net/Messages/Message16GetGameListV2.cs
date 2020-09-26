using System.Collections.Generic;
using Hazel;
using Impostor.Server.Net.State;
using Impostor.Shared.Innersloth;

namespace Impostor.Server.Net.Messages
{
    internal static class Message16GetGameListV2
    {
        public static void Deserialize(MessageReader reader, out GameOptionsData options)
        {
            reader.ReadPackedInt32(); // Hardcoded 0.
            options = GameOptionsData.Deserialize(reader.ReadBytesAndSize());
        }

        public static void Serialize(MessageWriter writer, IEnumerable<Game> games)
        {
            writer.StartMessage(MessageFlags.GetGameListV2);
                
            // Count
            writer.StartMessage(1);
            writer.Write(123); // The Skeld
            writer.Write(456); // Mira HQ
            writer.Write(789); // Polus
            writer.EndMessage();
                
            // Listing
            writer.StartMessage(0);
            foreach (var game in games)
            {
                writer.StartMessage(0);
                writer.Write(game.PublicIp.Address.GetAddressBytes());
                writer.Write((ushort) game.PublicIp.Port);
                writer.Write(game.Code);
                writer.Write(game.Host.Client.Name);
                writer.Write(game.PlayerCount);
                writer.WritePacked(1); // TODO: What does Age do?
                writer.Write((byte) game.Options.MapId);
                writer.Write((byte) game.Options.NumImpostors);
                writer.Write((byte) game.Options.MaxPlayers);
                writer.EndMessage();
            }
            writer.EndMessage();
                
            writer.EndMessage();
        }
    }
}