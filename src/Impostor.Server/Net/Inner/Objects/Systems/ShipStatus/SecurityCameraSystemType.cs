using System.Collections.Generic;
using Impostor.Server.Net.State;
using Impostor.Api.Net.Messages;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    internal class SecurityCameraSystemType : ISystemType
    {
        private readonly Game _game;
        public (HashSet<byte> UsingNow, HashSet<byte> StartedWatching, HashSet<byte> StoppedWatching) Players { get; internal set; } = (new (), new (), new ());
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
            startedWatching.ExceptWith(Players.UsingNow);

            var stoppedWatching = Players.UsingNow;
            stoppedWatching.ExceptWith(nowInUseBy);

            Players = (nowInUseBy, startedWatching, stoppedWatching);

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