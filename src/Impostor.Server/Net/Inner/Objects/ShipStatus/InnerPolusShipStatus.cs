using System.Collections.Generic;
using System.Numerics;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.Maps;
using Impostor.Api.Net.Inner.Objects.ShipStatus;
using Impostor.Server.Net.Inner.Objects.Systems;
using Impostor.Server.Net.Inner.Objects.Systems.ShipStatus;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner.Objects.ShipStatus
{
    internal class InnerPolusShipStatus : InnerShipStatus, IInnerPolusShipStatus
    {
        public InnerPolusShipStatus(Game game) : base(game)
        {
        }

        public override IMapData Data => IMapData.Maps[MapTypes.Polus];

        public override Dictionary<int, bool> Doors { get; } = new Dictionary<int, bool>(12);

        public override float SpawnRadius => 1f;

        public override Vector2 InitialSpawnCenter { get; } = new Vector2(16.64f, -2.46f);

        public override Vector2 MeetingSpawnCenter { get; } = new Vector2(17.726f, -16.286f);

        public Vector2 MeetingSpawnCenter2 { get; } = new Vector2(-17.7f, -17.5f);

        public override Vector2 GetSpawnLocation(InnerPlayerControl player, int numPlayers, bool initialSpawn)
        {
            if (initialSpawn)
            {
                return base.GetSpawnLocation(player, numPlayers, initialSpawn);
            }

            Vector2 position;
            if (player.PlayerId < 5)
            {
                position = this.MeetingSpawnCenter + (new Vector2(1, 0) * player.PlayerId);
            }
            else
            {
                position = this.MeetingSpawnCenter2 + (new Vector2(1, 0) * (player.PlayerId - 5));
            }

            return position;
        }

        protected override void AddSystems(Dictionary<SystemTypes, ISystemType> systems)
        {
            base.AddSystems(systems);

            systems.Add(SystemTypes.Doors, new DoorsSystemType(Doors));
            systems.Add(SystemTypes.Comms, new HudOverrideSystemType());
            systems.Add(SystemTypes.Security, new SecurityCameraSystemType());
            systems.Add(SystemTypes.Laboratory, new ReactorSystemType());
        }
    }
}
