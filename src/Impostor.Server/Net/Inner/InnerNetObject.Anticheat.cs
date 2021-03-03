using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Net;
using Impostor.Server.Net.Inner.Objects;

namespace Impostor.Server.Net.Inner
{
    internal abstract partial class InnerNetObject
    {
        protected async ValueTask<bool> ValidateOwnership(CheatContext context, IClientPlayer sender)
        {
            if (!sender.IsOwner(this))
            {
                if (await sender.Client.ReportCheatAsync(context, $"Failed ownership check on {GetType().Name}"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateHost(CheatContext context, IClientPlayer sender)
        {
            if (!sender.IsHost)
            {
                if (await sender.Client.ReportCheatAsync(context, "Failed host check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateTarget(CheatContext context, IClientPlayer sender, IClientPlayer? target)
        {
            if (target == null)
            {
                if (await sender.Client.ReportCheatAsync(context, "Failed target check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateBroadcast(CheatContext context, IClientPlayer sender, IClientPlayer? target)
        {
            if (target != null)
            {
                if (await sender.Client.ReportCheatAsync(context, "Failed broadcast check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateCmd(CheatContext context, IClientPlayer sender, IClientPlayer? target)
        {
            if (target == null || !target.IsHost)
            {
                if (await sender.Client.ReportCheatAsync(context, "Failed cmd check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateImpostor(CheatContext context, IClientPlayer sender, InnerPlayerInfo playerInfo, bool value = true)
        {
            if (playerInfo.IsImpostor != value)
            {
                if (await sender.Client.ReportCheatAsync(context, "Failed impostor check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> UnregisteredCall(CheatContext context, IClientPlayer sender)
        {
            if (await sender.Client.ReportCheatAsync(context, "Client sent unregistered call"))
            {
                return false;
            }

            return true;
        }
    }
}
