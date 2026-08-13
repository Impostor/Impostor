using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Innersloth;
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
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Ownership, $"Failed ownership check on {GetType().Name}"))
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
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.MustBeHost, "Failed host check"))
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
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Target, "Failed target check"))
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
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Target, "Failed broadcast check"))
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
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Target, "Failed cmd check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateImpostor(CheatContext context, IClientPlayer sender, InnerPlayerInfo? playerInfo, bool value = true)
        {
            if (playerInfo == null)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.InvalidObject, "Couldn't check if Impostor, playerInfo not set"))
                {
                    return false;
                }
            }
            else if (playerInfo.IsImpostor != value)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Role, "Failed impostor check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateCanVent(CheatContext context, IClientPlayer sender, InnerPlayerInfo? playerInfo, bool value = true)
        {
            if (playerInfo == null)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.InvalidObject, "Couldn't check if can vent, playerInfo not set"))
                {
                    return false;
                }
            }
            else if (playerInfo.CanVent != value)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Role, "Failed can vent check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateRole(CheatContext context, IClientPlayer sender, InnerPlayerInfo? playerInfo, RoleTypes role)
        {
            if (playerInfo == null)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.InvalidObject, "Couldn't check role, playerInfo not set"))
                {
                    return false;
                }
            }
            else if (playerInfo.RoleType != role)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Role, $"Failed role = {role} check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> UnregisteredCall(CheatContext context, IClientPlayer sender)
        {
            if (await sender.Client.ReportCheatAsync(context, CheatCategory.ProtocolExtension, "Client sent unregistered call"))
            {
                return false;
            }

            return true;
        }
    }
}
