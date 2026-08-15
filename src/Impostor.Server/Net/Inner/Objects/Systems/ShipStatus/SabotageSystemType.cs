using System;
using System.Linq;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class SabotageSystemType : ISystemType
    {
        private readonly IActivatable[] _specials;
        private readonly Action<IInnerPlayerControl?, SystemTypes, byte> _updateSystem;

        public SabotageSystemType(IActivatable[] specials, Action<IInnerPlayerControl?, SystemTypes, byte> updateSystem)
        {
            _specials = specials.Where(x => x is not IDoorSystem).ToArray();
            _updateSystem = updateSystem;
        }

        public float Timer { get; private set; }

        public bool AnyActive => _specials.Any(x => x.IsActive);

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            writer.Write(Timer);
        }

        public void Deserialize(IMessageReader reader, bool initialState)
        {
            Timer = reader.ReadSingle();
        }

        public void UpdateSystem(IInnerPlayerControl? playerControl, IMessageReader reader)
        {
            if (Timer > 0f)
            {
                return;
            }

            var systemType = (SystemTypes)reader.ReadByte();
            switch (systemType)
            {
                case SystemTypes.Laboratory:
                case SystemTypes.Reactor:
                case SystemTypes.HeliSabotage:
                case SystemTypes.LifeSupp:
                case SystemTypes.Comms:
                    _updateSystem(playerControl, systemType, 0x80);
                    break;
                case SystemTypes.Electrical:
                    var value = (byte)4;
                    for (var i = 0; i < 5; i++)
                    {
                        if (Random.Shared.Next(2) == 0)
                        {
                            value |= (byte)(1 << i);
                        }
                    }

                    _updateSystem(playerControl, systemType, (byte)(value | 0x80));
                    break;
                case SystemTypes.MushroomMixupSabotage:
                    _updateSystem(playerControl, systemType, 1);
                    break;
            }

            Timer = 30f;
        }
    }
}
