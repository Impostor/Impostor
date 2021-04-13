using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.C2S
{
    public static class Message00HostGameC2S
    {
        public static void Serialize(IMessageWriter writer, GameOptionsData gameOptionsData)
        {
            writer.StartMessage(MessageFlags.HostGame);
            gameOptionsData.Serialize(writer);
            writer.EndMessage();
        }

        /// <summary>
        ///     Deserialize a packet.
        /// </summary>
        /// <param name="reader"><see cref="IMessageReader" /> with <see cref="IMessageReader.Tag" /> 0.</param>
        /// <param name="chatMode">The chat type selected in the client of the player.</param>
        /// <returns>Deserialized <see cref="GameOptionsData" />.</returns>
        public static GameOptionsData Deserialize(IMessageReader reader, out QuickChatModes chatMode)
        {
            var gameOptionsData = GameOptionsData.DeserializeCreate(reader);
            chatMode = (QuickChatModes)reader.ReadByte();

            return gameOptionsData;
        }
    }
}
