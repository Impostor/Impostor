using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class DeconSystemType : ISystemType
    {
        private const byte HeadUpCommand = 1;
        private const byte HeadDownCommand = 2;
        private const byte HeadUpInsideCommand = 3;
        private const byte HeadDownInsideCommand = 4;

        public enum States : byte
        {
            Idle = 0,
            Enter = 1,
            Closed = 2,
            Exit = 4,
            HeadingUp = 8,
        }

        public byte Timer { get; private set; }

        public States CurState { get; private set; }

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            writer.Write(Timer);
            writer.Write((byte)CurState);
        }

        public void Deserialize(IMessageReader reader, bool initialState)
        {
            Timer = reader.ReadByte();
            CurState = (States)reader.ReadByte();
        }

        public void UpdateSystem(IInnerPlayerControl? playerControl, IMessageReader reader)
        {
            if (CurState != States.Idle)
            {
                return;
            }

            switch (reader.ReadByte())
            {
                case HeadUpCommand:
                    CurState = States.Enter | States.HeadingUp;
                    Timer = 3;
                    break;
                case HeadDownCommand:
                    CurState = States.Enter;
                    Timer = 3;
                    break;
                case HeadUpInsideCommand:
                    CurState = States.Exit | States.HeadingUp;
                    Timer = 3;
                    break;
                case HeadDownInsideCommand:
                    CurState = States.Exit;
                    Timer = 3;
                    break;
            }
        }
    }
}
