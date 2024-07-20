using System;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc60LobbyTimeExpiring
    {
        public static void Serialize(IMessageWriter writer, int timeRemainingSeconds, bool isExtensionAvailable, int hostId, int extensionId, int extendedTimeSeconds)
        {
            writer.WritePacked(timeRemainingSeconds);
            writer.Write(isExtensionAvailable);
            if (isExtensionAvailable)
            {
                // The following parameters is not used in vanilla code.
                writer.WritePacked(hostId);
                writer.WritePacked(extensionId);
                writer.WritePacked(extendedTimeSeconds);
                throw new NotImplementedException();
            }
        }

        public static void Deserialize(IMessageReader reader, out int timeRemainingSeconds, out bool isExtensionAvailable, out int hostId, out int extensionId, out int extendedTimeSeconds)
        {
            timeRemainingSeconds = reader.ReadPackedInt32();
            isExtensionAvailable = reader.ReadBoolean();

            if (isExtensionAvailable)
            {
                hostId = reader.ReadPackedInt32();
                extensionId = reader.ReadPackedInt32();
                extendedTimeSeconds = reader.ReadPackedInt32();
                throw new NotImplementedException();

                // extensionId is not used in vanilla code and there is no enum matching this "extension Id" in code.
            }
            else
            {
                hostId = 255;
                extensionId = 0;
                extendedTimeSeconds = 0;
            }
        }
    }
}
