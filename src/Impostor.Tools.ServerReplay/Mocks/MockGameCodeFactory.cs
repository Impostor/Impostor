using Impostor.Api.Games;

namespace Impostor.Tools.ServerReplay.Mocks
{
    public class MockGameCodeFactory : IGameCodeFactory
    {
        public GameCode Result { get; set; }

        public GameCode Create()
        {
            return Result;
        }
    }
}
