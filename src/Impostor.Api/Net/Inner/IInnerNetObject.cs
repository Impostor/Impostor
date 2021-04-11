using Impostor.Api.Games;

namespace Impostor.Api.Net.Inner
{
    public interface IInnerNetObject
    {
        public uint NetId { get; }

        public int OwnerId { get; }

        public IGame Game { get; }
    }
}
