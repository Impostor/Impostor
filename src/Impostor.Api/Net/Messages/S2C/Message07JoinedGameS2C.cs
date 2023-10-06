using System;

namespace Impostor.Api.Net.Messages.S2C
{
    public static class Message07JoinedGameS2C
    {
        public static void Serialize(IMessageWriter writer, bool clear, int gameCode, int playerId, int hostId, IClientPlayer[] otherPlayers)
        {
            if (clear)
            {
                writer.Clear(MessageType.Reliable);
            }

            writer.StartMessage(MessageFlags.JoinedGame);
            writer.Write(gameCode);
            writer.Write(playerId);
            writer.Write(hostId);
            writer.WritePacked(otherPlayers.Length);

            foreach (var ply in otherPlayers)
            {
                writer.WritePacked(ply.Client.Id);
                writer.Write(ply.Client.Name);
                ply.Client.PlatformSpecificData.Serialize(writer);
                writer.WritePacked(ply.Character?.PlayerInfo.PlayerLevel ?? 1);

                // ProductUserId and FriendCode are not yet known, so set them to an empty string
                writer.Write(string.Empty);
                writer.Write(string.Empty);
            }

            writer.EndMessage();
        }

        public static void Deserialize(IMessageReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
