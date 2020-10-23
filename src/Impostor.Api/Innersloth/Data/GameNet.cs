using Impostor.Api.Games;
using Impostor.Api.Innersloth.Net;
using Impostor.Api.Innersloth.Net.Objects;

namespace Impostor.Api.Innersloth.Data
{
    /// <summary>
    ///     Holds all data that is serialized over the network through GameData packets.
    /// </summary>
    public class GameNet
    {
        private readonly IGame _game;

        public GameNet(IGame game)
        {
            _game = game;
        }

        public InnerGameData GameData { get; private set; }

        internal void OnSpawn(InnerNetObject netObj)
        {
            if (netObj is InnerGameData data)
            {
                GameData = data;
            }
            else if (netObj is InnerPlayerControl control)
            {
                // Hook up InnerPlayerControl <-> IClientPlayer.
                var clientPlayer = _game.GetClientPlayer(control.OwnerId);
                if (clientPlayer != null)
                {
                    clientPlayer.Character = control;
                }

                // Hook up InnerPlayerControl <-> InnerPlayerControl.PlayerInfo.
                control.PlayerInfo = GameData.GetPlayerById(control.PlayerId);

                if (control.PlayerInfo == null)
                {
                    GameData.AddPlayer(control);
                }
            }
        }

        internal void OnDestroy(InnerNetObject netObj)
        {
            if (netObj is InnerGameData)
            {
                GameData = null;
            }
            else if (netObj is InnerPlayerControl control)
            {
                // Remove InnerPlayerControl <-> IClientPlayer.
                var clientPlayer = _game.GetClientPlayer(control.OwnerId);
                if (clientPlayer != null)
                {
                    clientPlayer.Character = null;
                }
            }
        }
    }
}