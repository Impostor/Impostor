using Impostor.Api.Net.Inner.Objects.ShipSystems;

namespace Impostor.Api.Net.Inner.Objects
{
    public interface IInnerShipStatus : IInnerNetObject
    {
        ICommsSystem GetCommsSystem();

        IReactorSystem GetReactorSystem();
    }
}
