using System.Collections.Generic;
using Impostor.Api.Net.Messages;
using Impostor.Api.Reactor;

namespace Impostor.Server.Net.Reactor
{
    public static class ModList
    {
        public static void Deserialize(IMessageReader reader, out ISet<Mod> mods)
        {
            var length = reader.ReadPackedInt32();

            mods = new HashSet<Mod>(length);

            for (var i = 0; i < length; i++)
            {
                var id = reader.ReadString();
                var version = reader.ReadString();

                mods.Add(new Mod(id, version));
            }
        }
    }
}
