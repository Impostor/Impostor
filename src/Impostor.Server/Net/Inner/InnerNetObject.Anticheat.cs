using System;
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

        protected async ValueTask<bool> ValidateCanDoTask(CheatContext context, IClientPlayer sender, InnerPlayerInfo? playerInfo, bool value = true)
        {
            if (playerInfo == null)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.InvalidObject, "Couldn't check if can do tasks, playerInfo not set"))
                {
                    return false;
                }
            }
            else if (playerInfo.CanDoTasks != value)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Role, "Failed can do tasks check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateCanKill(CheatContext context, IClientPlayer sender, InnerPlayerInfo? playerInfo, bool value = true)
        {
            if (playerInfo == null)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.InvalidObject, "Couldn't check if can kill, playerInfo not set"))
                {
                    return false;
                }
            }
            else if (playerInfo.CanKill != value)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Role, "Failed can kill check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateCanSabotage(CheatContext context, IClientPlayer sender, InnerPlayerInfo? playerInfo, bool value = true)
        {
            if (playerInfo == null)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.InvalidObject, "Couldn't check if can sabotage, playerInfo not set"))
                {
                    return false;
                }
            }
            else if (playerInfo.CanSabotage != value)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Role, "Failed can sabotage check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateCanShapeshift(CheatContext context, IClientPlayer sender, InnerPlayerInfo? playerInfo, bool value = true)
        {
            if (playerInfo == null)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.InvalidObject, "Couldn't check if can shapeshift, playerInfo not set"))
                {
                    return false;
                }
            }
            else if (playerInfo.CanShapeshift != value)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Role, "Failed can shapeshift check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateCanVanish(CheatContext context, IClientPlayer sender, InnerPlayerInfo? playerInfo, bool value = true)
        {
            if (playerInfo == null)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.InvalidObject, "Couldn't check if can vanish, playerInfo not set"))
                {
                    return false;
                }
            }
            else if (playerInfo.CanVanish != value)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Role, "Failed can vanish check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateCanProtect(CheatContext context, IClientPlayer sender, InnerPlayerInfo? playerInfo, bool value = true)
        {
            if (playerInfo == null)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.InvalidObject, "Couldn't check if can protect, playerInfo not set"))
                {
                    return false;
                }
            }
            else if (playerInfo.CanProtect != value)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Role, "Failed can protect check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateCanOverrule(CheatContext context, IClientPlayer sender, InnerPlayerInfo? playerInfo, bool value = true)
        {
            if (playerInfo == null)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.InvalidObject, "Couldn't check if can overrule, playerInfo not set"))
                {
                    return false;
                }
            }
            else if (playerInfo.CanOverrule != value)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Role, "Failed can overrule check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateCanSendPhoto(CheatContext context, IClientPlayer sender, InnerPlayerInfo? playerInfo, bool value = true)
        {
            if (playerInfo == null)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.InvalidObject, "Couldn't check if can send photo, playerInfo not set"))
                {
                    return false;
                }
            }
            else if (playerInfo.CanSendPhoto != value)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Role, "Failed can send photo check"))
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
