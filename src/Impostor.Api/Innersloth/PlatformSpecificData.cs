using System;

namespace Impostor.Api.Innersloth
{
    public class PlatformSpecificData
    {
        public PlatformSpecificData(Platforms platform, string platformName, ulong? xboxPlatformId = null, ulong? psnPlatformId = null)
        {
            Platform = platform;
            PlatformName = platformName;
            XboxPlatformId = xboxPlatformId;
            PsnPlatformId = psnPlatformId;
        }

        public PlatformSpecificData(IMessageReader reader)
        {
            Platform = (Platforms)reader.Tag;
            PlatformName = reader.ReadString();

            XboxPlatformId = Platform == Platforms.Xbox ? reader.ReadUInt64() : null;
            PsnPlatformId = Platform == Platforms.Playstation ? reader.ReadUInt64() : null;
        }

        public Platforms Platform { get; }

        public string PlatformName { get; }

        public ulong? XboxPlatformId { get; }

        public ulong? PsnPlatformId { get; }

        public void Serialize(IMessageWriter writer)
        {
            writer.StartMessage((byte)Platform);
            writer.Write(PlatformName);
            switch (Platform)
            {
                case Platforms.Xbox:
                    writer.Write(XboxPlatformId ?? throw new NullReferenceException($"{nameof(XboxPlatformId)} shouldn't be null when {nameof(Platform)} is Xbox"));
                    break;
                case Platforms.Playstation:
                    writer.Write(PsnPlatformId ?? throw new NullReferenceException($"{nameof(PsnPlatformId)} shouldn't be null when {nameof(Platform)} is Playstation"));
                    break;
            }

            writer.EndMessage();
        }
    }
}
