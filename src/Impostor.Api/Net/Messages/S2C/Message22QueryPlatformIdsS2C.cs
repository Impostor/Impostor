using System;
using System.Collections.Generic;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.S2C
{
    public class Message22QueryPlatformIdsS2C
    {
        public static void Serialize(IMessageWriter writer, GameCode gameCode, IEnumerable<PlatformSpecificData> playerSpecificDatas)
        {
            writer.StartMessage(MessageFlags.QueryPlatformIds);
            gameCode.Serialize(writer);
            foreach (var playerSpecificData in playerSpecificDatas)
            {
                playerSpecificData.Serialize(writer);
            }

            writer.EndMessage();
        }

        public static void Deserialize(IMessageReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
