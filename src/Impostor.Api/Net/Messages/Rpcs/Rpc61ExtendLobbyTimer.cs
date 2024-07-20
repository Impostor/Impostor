using System;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc61ExtendLobbyTimer
    {
        public static void Serialize(IMessageWriter writer, int extensionId, bool isSuccess, byte extensionFailureReasons)
        {
            writer.WritePacked(extensionId);
            writer.Write(isSuccess);
            if (!isSuccess)
            {
                writer.Write(extensionFailureReasons);
            }
        }

        public static void Deserialize(IMessageReader reader, out int extensionId, out bool isSuccess, out byte extensionFailureReasons)
        {
            extensionId = reader.ReadPackedInt32();
            isSuccess = reader.ReadBoolean();

            if (!isSuccess)
            {
                extensionFailureReasons = reader.ReadByte();

                // The game currently only logs the failure reason and dont act on it.
                // ExtensionFailureReasons enum exists in code but is not used, not adding it here until InnerSloth put real use to it.
                throw new NotImplementedException();
            }
            else
            {
                extensionFailureReasons = 0;
            }
        }
    }
}
