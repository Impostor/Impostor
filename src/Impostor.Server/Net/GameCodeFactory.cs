using Impostor.Api.Games;

namespace Impostor.Server.Net
{
    public class GameCodeFactory : IGameCodeFactory
    {
        public GameCode Create()
        {
            return GameCode.Create();
        }
    }
}
