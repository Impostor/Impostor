using System;
using System.Threading.Tasks;
using Impostor.Api.Config;
using Impostor.Api.Net;
using Impostor.Api.Net.Custom;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Server.Net.State;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Net.Inner.Objects
{
    internal class InnerLobbyBehaviour : InnerNetObject, IInnerLobbyBehaviour
    {
        public InnerLobbyBehaviour(ICustomMessageManager<ICustomRpc> customMessageManager, IOptions<AntiCheatConfig> antiCheatConfig, Game game) : base(customMessageManager, antiCheatConfig, game)
        {
            Components.Add(this);
        }

        public override ValueTask<bool> SerializeAsync(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
        }

        public override ValueTask DeserializeAsync(IClientPlayer sender, IClientPlayer? target, IMessageReader reader, bool initialState)
        {
            throw new NotImplementedException();
        }
    }
}
