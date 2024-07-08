using System;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc35UpdateSystem
    {
        public static void Serialize(IMessageWriter writer, SystemTypes systemType, IInnerPlayerControl playerControl, ushort sequenceId, byte state, byte ventId)
        {
            // In vanilla game only VentilationSystem uses a messagewriter to pass details.
            if (systemType is not SystemTypes.Ventilation)
            {
                throw new NotImplementedException();
            }

            writer.Write((byte)systemType);
            writer.Write(playerControl);
            writer.Write(sequenceId);
            writer.Write(state);
            writer.Write(ventId);
        }

        public static void Serialize(IMessageWriter writer, SystemTypes systemType, IInnerPlayerControl playerControl, byte amount)
        {
            writer.Write((byte)systemType);
            writer.Write(playerControl);
            writer.Write(amount);
        }

        public static void Deserialize(IMessageReader reader, IGame game, out SystemTypes systemType, out IInnerPlayerControl? playerControl, out byte amount, out ushort sequenceId, out byte state, out byte ventId)
        {
            systemType = (SystemTypes)reader.ReadByte();
            playerControl = reader.ReadNetObject<IInnerPlayerControl>(game);

            if (systemType is SystemTypes.Ventilation)
            {
                amount = 0;
                sequenceId = reader.ReadUInt16();
                state = reader.ReadByte();
                ventId = reader.ReadByte();
            }
            else
            {
                amount = reader.ReadByte();
                sequenceId = 0;
                state = 0;
                ventId = 0;
            }
        }
    }
}
