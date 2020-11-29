using Impostor.Api.Net.Inner.Objects.Components;

namespace Impostor.Server.Net.Inner.Objects.Components
{
    internal partial class InnerPlayerPhysics : IInnerPlayerPhysics
    {
        bool IInnerPlayerPhysics.IsWatchingCamera => _isWatchingCamera;
    }
}
