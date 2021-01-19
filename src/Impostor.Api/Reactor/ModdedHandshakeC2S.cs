using System.Collections.Generic;
using Impostor.Api.Net.Messages;

namespace Impostor.Api.Reactor
{
    public static class ModdedHandshakeC2S
    {
        public static void Deserialize(IMessageReader reader, out int clientVersion, out string name, out ISet<Mod>? mods)
        {
            clientVersion = reader.ReadInt32();
            name = reader.ReadString();

            if (reader.Length > reader.Position)
            {
                ModList.Deserialize(reader, out mods);
            }
            else
            {
                mods = null;
            }
        }
    }
}
