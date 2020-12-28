using Impostor.Api.Net.Inner.Objects.ShipSystems;

namespace Impostor.Api.Net.Inner.Objects
{
    public interface IInnerShipStatus : IInnerNetObject
    {
        ICommsSystem CommsSystem { get; }

        IReactorSystem ReactorSystem { get; }

        ISabotageSystem SabotageSystem { get; }

        ISwitchSystem SwitchSystem { get; }

        IOxygenSystem OxygenSystem { get; }
    }
}
