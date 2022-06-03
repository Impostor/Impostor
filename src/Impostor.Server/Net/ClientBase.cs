using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net
{
    internal abstract class ClientBase : IClient
    {
        protected ClientBase(string name, int gameVersion, Language language, QuickChatModes chatMode, PlatformSpecificData platformSpecificData, IHazelConnection connection)
        {
            Name = name;
            GameVersion = gameVersion;
            Language = language;
            ChatMode = chatMode;
            PlatformSpecificData = platformSpecificData;
            Connection = connection;
            Items = new ConcurrentDictionary<object, object>();
        }

        public int Id { get; set; }

        public string Name { get; }

        public Language Language { get; }

        public QuickChatModes ChatMode { get; }

        public PlatformSpecificData PlatformSpecificData { get; }

        public int GameVersion { get; }

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
            await Connection.CustomDisconnectAsync(reason, message);
        }
    }
}
