using System.Collections.Generic;
using Impostor.Api.Net.Messages;

namespace Impostor.Api.Reactor
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
                var pluginSide = (PluginSide)reader.ReadByte();

                mods.Add(new Mod(id, version, pluginSide));
            }
        }
    }
}
