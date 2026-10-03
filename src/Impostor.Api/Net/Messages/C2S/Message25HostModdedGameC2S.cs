using System;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.GameOptions;

namespace Impostor.Api.Net.Messages.C2S
{
    /// <summary>
    /// Special serialization rules for AU MCI HostGame messages.
    /// <see href="https://github.com/Innersloth-LLC/AmongUsModdingInformation?tab=readme-ov-file#all-client-mod-client-identification"/>
    /// for more information.
    /// </summary>
    public static class Message25HostModdedGameC2S
    {
        public static void Serialize(IMessageWriter writer, IGameOptions gameOptions, CrossplayFlags crossplayFlags, GameFilterOptions gameFilterOptions)
        {
            Message00HostGameC2S.Serialize(writer, gameOptions, crossplayFlags, gameFilterOptions);
        }

        public static void Deserialize(IMessageReader reader, out IGameOptions gameOptions, out CrossplayFlags crossplayFlags, out GameFilterOptions gameFilterOptions, out Guid modGuid)
        {
            Message00HostGameC2S.Deserialize(reader, out gameOptions, out crossplayFlags, out gameFilterOptions);
            modGuid = new Guid(reader.ReadBytes(16).Span);
        }
    }
}
