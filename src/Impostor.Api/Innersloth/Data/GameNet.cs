using Impostor.Api.Games;
using Impostor.Api.Innersloth.Net;
using Impostor.Api.Innersloth.Net.Objects;
using Impostor.Api.Innersloth.Net.Objects.Components;

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

        public InnerLobbyBehaviour LobbyBehaviour { get; private set; }

        public InnerGameData GameData { get; private set; }

        public InnerVoteBanSystem VoteBan { get; private set; }

        public InnerShipStatus ShipStatus { get; private set; }

        internal void OnSpawn(InnerNetObject netObj)
        {
            switch (netObj)
            {
                case InnerLobbyBehaviour lobby:
                {
                    LobbyBehaviour = lobby;
                    break;
                }

                case InnerGameData data:
                {
                    GameData = data;
                    break;
                }

                case InnerVoteBanSystem voteBan:
                {
                    VoteBan = voteBan;
                    break;
                }

                case InnerShipStatus shipStatus:
                {
                    ShipStatus = shipStatus;
                    break;
                }

                case InnerPlayerControl control:
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

                    break;
                }
            }
        }

        internal void OnDestroy(InnerNetObject netObj)
        {
            switch (netObj)
            {
                case InnerLobbyBehaviour lobby:
                {
                    LobbyBehaviour = null;
                    break;
                }

                case InnerGameData:
                {
                    GameData = null;
                    break;
                }

                case InnerVoteBanSystem:
                {
                    VoteBan = null;
                    break;
                }

                case InnerShipStatus:
                {
                    ShipStatus = null;
                    break;
                }

                case InnerPlayerControl control:
                {
                    // Remove InnerPlayerControl <-> IClientPlayer.
                    var clientPlayer = _game.GetClientPlayer(control.OwnerId);
                    if (clientPlayer != null)
                    {
                        clientPlayer.Character = null;
                    }

                    break;
                }
            }
        }
    }
}