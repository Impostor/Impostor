using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages;
using Impostor.Api.Net.Messages.S2C;
using Impostor.Api.Reactor;
using Impostor.Hazel;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net
{
    internal abstract class ClientBase : IClient
    {
        protected ClientBase(string name, int gameVersion, IHazelConnection connection, ISet<Mod> mods)
        {
            Name = name;
            GameVersion = gameVersion;
            Connection = connection;
            Mods = mods;
            Items = new ConcurrentDictionary<object, object>();

            ModIdMap = new Dictionary<int, string>();

            var i = -1;

            foreach (var mod in mods.OrderBy(x => x.Id))
            {
                if (mod.Side == PluginSide.Both)
                {
                    ModIdMap[i--] = mod.Id;
                }
            }
        }

        public int Id { get; set; }

        public string Name { get; }

        public int GameVersion { get; }

        public ISet<Mod> Mods { get; }

        public Dictionary<int, string> ModIdMap { get; }

        public IHazelConnection Connection { get; }

        public IDictionary<object, object> Items { get; }

        public ClientPlayer? Player { get; set; }

        IClientPlayer? IClient.Player => Player;

        public virtual ValueTask<bool> ReportCheatAsync(CheatContext context, string message)
        {
            return new ValueTask<bool>(false);
        }

        public abstract ValueTask HandleMessageAsync(IMessageReader message, MessageType messageType);

        public abstract ValueTask HandleDisconnectAsync(string reason);

        public async ValueTask DisconnectAsync(DisconnectReason reason, string? message = null)
        {
            if (!Connection.IsConnected)
            {
                return;
            }

            using var packet = MessageWriter.Get(MessageType.Reliable);
            Message01JoinGameS2C.SerializeError(packet, false, reason, message);

            await Connection.SendAsync(packet);

            // Need this to show the correct message, otherwise it shows a generic disconnect message.
            await Task.Delay(TimeSpan.FromMilliseconds(250));

            await Connection.DisconnectAsync(message ?? reason.ToString());
        }
    }
}
