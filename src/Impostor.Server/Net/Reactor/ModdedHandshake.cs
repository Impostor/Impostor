using System.Collections.Generic;
using Impostor.Api.Net.Messages;
using Impostor.Api.Reactor;

namespace Impostor.Server.Net.Reactor
{
    public static class ModdedHandshake
    {
        public static void Deserialize(IMessageReader reader, out int clientVersion, out string name, out ISet<Mod>? mods)
        {
            var isModded = false;

            clientVersion = reader.ReadInt32();
            if (clientVersion == -1)
            {
                clientVersion = reader.ReadInt32();
                isModded = true;
            }

            name = reader.ReadString();

            if (isModded)
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
