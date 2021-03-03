using System;
using System.Collections.Generic;
using System.Numerics;

namespace Impostor.Api.Innersloth
{
    public class MapSpawn
    {
        private MapSpawn(float spawnRadius, Vector2 initialSpawnCenter, Vector2 meetingSpawnCenter)
        {
            SpawnRadius = spawnRadius;
            InitialSpawnCenter = initialSpawnCenter;
            MeetingSpawnCenter = meetingSpawnCenter;
        }

        public static Dictionary<MapTypes, MapSpawn> Maps { get; } = new Dictionary<MapTypes, MapSpawn>
        {
            [MapTypes.Skeld] = new MapSpawn(1.6f, new Vector2(-0.72f, 0.62f), new Vector2(-0.72f, 0.62f)),
            [MapTypes.MiraHQ] = new MapSpawn(1.55f, new Vector2(-4.4f, 2.2f), new Vector2(24.043f, 1.72f)),
            [MapTypes.Polus] = new MapSpawn(1f, new Vector2(16.64f, -2.46f), new Vector2(17.726f, -16.286f)),
        };

        public float SpawnRadius { get; }

        public Vector2 InitialSpawnCenter { get; }

        public Vector2 MeetingSpawnCenter { get; }

        public Vector2 GetSpawnLocation(int playerId, int numPlayer, bool initialSpawn)
        {
            var vector = new Vector2(0, 1);
            vector = Rotate(vector, (playerId - 1) * (360f / numPlayer));
            vector *= this.SpawnRadius;
            return (initialSpawn ? this.InitialSpawnCenter : this.MeetingSpawnCenter) + vector + new Vector2(0f, 0.3636f);
        }

        private static Vector2 Rotate(Vector2 self, float degrees)
        {
            var f = 0.017453292f * degrees;
            var num = (float)Math.Cos(f);
            var num2 = (float)Math.Sin(f);
            return new Vector2((self.X * num) - (num2 * self.Y), (self.X * num2) + (num * self.Y));
        }
    }
}
