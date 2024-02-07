using System;
using System.Threading.Tasks;
using Impostor.Api.Net;
using Impostor.Api.Net.Custom;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner.Objects
{
    internal class InnerLobbyBehaviour : InnerNetObject, IInnerLobbyBehaviour
    {
        public InnerLobbyBehaviour(ICustomMessageManager<ICustomRpc> customMessageManager, Game game) : base(customMessageManager, game)
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
