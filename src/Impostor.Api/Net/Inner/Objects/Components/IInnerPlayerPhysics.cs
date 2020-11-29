namespace Impostor.Api.Net.Inner.Objects.Components
{
    public interface IInnerPlayerPhysics : IInnerNetObject
    {
        /// <summary>
        ///     Gets a value indicating whether the player is watching security camera
        /// </summary>
        bool IsWatchingCamera  { get; }
    }
}
