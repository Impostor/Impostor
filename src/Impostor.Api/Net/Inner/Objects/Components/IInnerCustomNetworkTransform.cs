using System.Numerics;
using System.Threading.Tasks;

namespace Impostor.Api.Net.Inner.Objects.Components
{
    public interface IInnerCustomNetworkTransform : IInnerNetObject
    {
        ValueTask SnapTo(Vector2 position);
    }
}
