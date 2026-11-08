using System.Threading.Tasks;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Inner.Objects.ShipStatus;

namespace Impostor.Api.Net.Custom
{
    /// <summary>
    ///     Handles custom <see cref="IInnerShipStatus" /> system state and update payloads.
    /// </summary>
    public interface ICustomSystemType : ICustomMessage
    {
        ValueTask DeserializeAsync(IInnerShipStatus shipStatus, IClientPlayer sender, IClientPlayer? target, IMessageReader reader, bool initialState);

        ValueTask<bool> HandleUpdateSystemAsync(IInnerShipStatus shipStatus, IInnerPlayerControl? playerControl, IClientPlayer sender, IClientPlayer? target, IMessageReader reader);
    }
}
