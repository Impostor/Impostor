using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Net;
using Impostor.Api.Net.Custom;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Messages.Rpcs;
using Impostor.Server.Net.State;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Inner.Objects
{
    internal class InnerLobbyBehaviour : InnerNetObject, IInnerLobbyBehaviour
    {
        private readonly ILogger<InnerLobbyBehaviour> _logger;

        public InnerLobbyBehaviour(ICustomMessageManager<ICustomRpc> customMessageManager, Game game, ILogger<InnerLobbyBehaviour> logger) : base(customMessageManager, game)
        {
            _logger = logger;
            Components.Add(this);
        }

        public override ValueTask<bool> SerializeAsync(IMessageWriter writer, bool initialState)
        {
            return new ValueTask<bool>(false);
        }

        public override ValueTask DeserializeAsync(IClientPlayer sender, IClientPlayer? target, IMessageReader reader, bool initialState)
        {
            return default;
        }

        public override async ValueTask<bool> HandleRpcAsync(ClientPlayer sender, ClientPlayer? target, RpcCalls call, IMessageReader reader)
        {
            switch (call)
            {
                case RpcCalls.LobbyTimeExpiring:
                {
                    if (await sender.Client.ReportCheatAsync(RpcCalls.LobbyTimeExpiring, CheatCategory.ProtocolExtension, "Client should never send LobbyTimeExpiring."))
                    {
                        return false;
                    }

                    if (!await ValidateHost(call, sender))
                    {
                        return false;

                        // Some Host-Only mods is using this Rpc to sync room close time on vanilla server.
                        // It should be a good practice to only allow host to send this rpc if Protocol Extension is enabled.
                    }

                    Rpc60LobbyTimeExpiring.Deserialize(reader, out var timeRemainingSeconds, out _, out _, out _, out _);
                    _logger.LogInformation("{0} - {1} sent Lobby time expiring for {2}s", sender.Game.Code, sender.Client.Id, timeRemainingSeconds);
                    break;
                }

                case RpcCalls.ExtendLobbyTimer:
                {
                    if (await sender.Client.ReportCheatAsync(RpcCalls.ExtendLobbyTimer, CheatCategory.ProtocolExtension, "Client should never send ExtendLobbyTimer."))
                    {
                        return false;
                    }

                    if (!await ValidateHost(call, sender))
                    {
                        return false;
                    }

                    Rpc61ExtendLobbyTimer.Deserialize(reader, out _, out _, out _);
                    _logger.LogInformation("{0} - {1} sent Extend Lobby Timer", sender.Game.Code, sender.Client.Id);
                    break;
                }

                default:
                    return await base.HandleRpcAsync(sender, target, call, reader);
            }

            return true;
        }
    }
}
