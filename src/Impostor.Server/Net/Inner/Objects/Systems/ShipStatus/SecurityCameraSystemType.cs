using System.Collections.Generic;
using Impostor.Server.Net.State;
using Impostor.Api.Net.Messages;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    internal class SecurityCameraSystemType : ISystemType
    {
        private readonly Game _game;
        public HashSet<byte> PlayersUsingNow { get; internal set; } = new ();
        public HashSet<byte> PlayersStartedWatching { get; internal set; } = new ();
        public HashSet<byte> PlayersStoppedWatching { get; internal set; } = new ();

        public SecurityCameraSystemType(Game game)
        {
            _game = game;
        }

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(IMessageReader reader, bool initialState)
        {
            var count = reader.ReadPackedInt32();
            var nowInUseBy = new HashSet<byte>();
            for (var i = 0; i < count; i++)
            {
                nowInUseBy.Add(reader.ReadByte());
            }

            var startedWatching = new HashSet<byte>(nowInUseBy);
            startedWatching.ExceptWith(PlayersUsingNow);

            var stoppedWatching = PlayersUsingNow;
            stoppedWatching.ExceptWith(nowInUseBy);

            PlayersUsingNow = nowInUseBy;
            PlayersStartedWatching = startedWatching;
            PlayersStoppedWatching = stoppedWatching;

            foreach (var id in startedWatching)
            {
                if(_game.GameNet.GameData.GetPlayerById(id) is InnerPlayerInfo player)
                {
                    player.Controller.SetActivity(Api.Innersloth.ActivityType.WatchingCamera);
                }
            }

            foreach (var id in stoppedWatching)
            {
                if(_game.GameNet.GameData.GetPlayerById(id) is InnerPlayerInfo player)
                {
                    player.Controller.SetActivity(Api.Innersloth.ActivityType.Default);
                }
            }
        }
    }
}