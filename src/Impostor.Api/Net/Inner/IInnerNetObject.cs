namespace Impostor.Api.Net.Inner
{
    public interface IInnerNetObject
    {
        public uint NetId { get; }

        public int OwnerId { get; }
    }
}
