using Impostor.Api.Innersloth.Net;
using Impostor.Api.Innersloth.Net.Objects;

namespace Impostor.Api.Innersloth.Data
{
    /// <summary>
    ///     Holds all data that is serialized over the network through GameData packets.
    /// </summary>
    public class GameNet
    {
        public InnerGameData GameData { get; internal set; }

        internal void OnSpawn(InnerNetObject netObj)
        {
            if (netObj is InnerGameData data)
            {
                GameData = data;
            }
        }

        internal void OnDestroy(InnerNetObject netObj)
        {
            if (netObj is InnerGameData)
            {
                GameData = null;
            }
        }
    }
}