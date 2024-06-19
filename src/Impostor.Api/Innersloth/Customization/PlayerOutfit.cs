namespace Impostor.Api.Innersloth.Customization
{
    public class PlayerOutfit
    {
        public ColorType Color { get; internal set; } = (ColorType)(-1);

        public string HatId { get; internal set; } = "missing";

        public string PetId { get; internal set; } = "missing";

        public string SkinId { get; internal set; } = "missing";

        public string VisorId { get; internal set; } = "missing";

        public string NamePlateId { get; internal set; } = "missing";

        public string PlayerName { get; internal set; } = string.Empty;

        public byte HatSequenceId { get; internal set; } = 0;

        public byte PetSequenceId { get; internal set; } = 0;

        public byte SkinSequenceId { get; internal set; } = 0;

        public byte VisorSequenceId { get; internal set; } = 0;

        public byte NamePlateSequenceId { get; internal set; } = 0;

        public bool IsIncomplete
        {
            get
            {
                if (!string.IsNullOrEmpty(PlayerName) && Color != (ColorType)(-1) && HatId != "missing" && PetId != "missing" && SkinId != "missing" && VisorId != "missing")
                {
                    return NamePlateId == "missing";
                }

                return true;
            }
        }

        public void Serialize(IMessageWriter writer)
        {
            writer.Write(PlayerName);
            writer.WritePacked((int)Color);
            writer.Write(HatId);
            writer.Write(PetId);
            writer.Write(SkinId);
            writer.Write(VisorId);
            writer.Write(NamePlateId);
            writer.Write(HatSequenceId);
            writer.Write(PetSequenceId);
            writer.Write(SkinSequenceId);
            writer.Write(VisorSequenceId);
            writer.Write(NamePlateSequenceId);
        }

        public void Deserialize(IMessageReader reader)
        {
            PlayerName = reader.ReadString();
            Color = (ColorType)reader.ReadPackedInt32();
            HatId = reader.ReadString();
            PetId = reader.ReadString();
            SkinId = reader.ReadString();
            VisorId = reader.ReadString();
            NamePlateId = reader.ReadString();
            HatSequenceId = reader.ReadByte();
            PetSequenceId = reader.ReadByte();
            SkinSequenceId = reader.ReadByte();
            VisorSequenceId = reader.ReadByte();
            NamePlateSequenceId = reader.ReadByte();
        }
    }
}
