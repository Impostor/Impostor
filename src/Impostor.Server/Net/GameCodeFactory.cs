using Impostor.Api.Games;
using Impostor.Server.Account;

namespace Impostor.Server.Net
{
    public class GameCodeFactory : IGameCodeFactory
    {
        private readonly GameCodeAuthManager _accounts;

        public GameCodeFactory(GameCodeAuthManager accounts)
        {
            _accounts = accounts;
        }

        public GameCode Create()
        {
            GameCode code;

            do
            {
                code = GameCode.Create();
            }
            while (_accounts.IsAlias(code));

            return code;
        }
    }
}
