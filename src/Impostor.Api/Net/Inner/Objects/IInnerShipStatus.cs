using System.Threading.Tasks;
using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Inner.Objects
{
    public interface IInnerShipStatus : IInnerNetObject
    {
        ValueTask Sabotage(SystemTypes systemType);
    }
}